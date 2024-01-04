using System;
using System.Collections.Generic;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RL.Algorithms;
using RL.Box2D;
using RL.Environments;
using RL.MDArrays;

namespace RL.Runner;

public class EnvironmentWindow<TE, TO, TA>
    where TE : IEnvironment<TO, TA>
{
    private readonly Dictionary<Keys, IAgent<TO, TA>> _agents = [];

    private IAgent<TO, TA>? _agent;
    private bool _needReset = true;
    private IAgent<TO, TA>? _nextAgent;

    public EnvironmentWindow(TE environment, int width, int height)
    {
        Environment = environment;
        var window = new Window(environment.Name, width, height);
        window.UpdateFrame += OnUpdateFrame;
        window.Unload += OnUnload;
        Window = window;
    }

    public TE Environment { get; }
    public Window Window { get; }
    public float TotalReward { get; private set; }
    public int Episode { get; private set; } = -1;

    public event Action<EnvironmentWindow<TE, TO, TA>>? LoadEvent;
    public event Action<EnvironmentWindow<TE, TO, TA>>? UpdateEvent;
    public event Action<EnvironmentWindow<TE, TO, TA>>? StepEvent;
    public event Action<EnvironmentWindow<TE, TO, TA>>? CompleteEvent;
    public event Action<EnvironmentWindow<TE, TO, TA>>? UnloadEvent;

    public void AddAgent(Keys key, IAgent<TO, TA> agent)
    {
        if (_agents.Count == 0)
            _agent = agent;
        _agents.Add(key, agent);
        _nextAgent = agent;
    }

    public void Run()
    {
        LoadEvent?.Invoke(this);
        Window.VSync = VSyncMode.Off;
        Window.Run();
    }

    private void Reset()
    {
        if (_nextAgent != null)
        {
            _agent = _nextAgent;
            _nextAgent = null;
        }

        _needReset = false;
        Episode++;
        Environment.Reset();
        TotalReward = 0.0f;
    }

    private void OnUpdateFrame(FrameEventArgs args)
    {
        if (_needReset || Window.KeyboardState.IsKeyReleased(Keys.R))
            Reset();

        foreach (var agent in _agents)
            if (Window.KeyboardState.IsKeyReleased(agent.Key))
                _agent = agent.Value;

        if (_agent != null && !Window.Paused)
        {
            var action = _agent.Predict(Environment.State);
            var transition = Environment.Step(action);
            TotalReward += transition.Reward;

            StepEvent?.Invoke(this);

            if (transition.Terminated || transition.Truncated)
            {
                CompleteEvent?.Invoke(this);
                _needReset = true;
            }
        }

        UpdateEvent?.Invoke(this);
    }

    private void OnUnload()
    {
        UnloadEvent?.Invoke(this);
        UnloadEvent = null;
        Window.UpdateFrame += OnUpdateFrame;
        Window.Unload += OnUnload;
    }
}

public static class EnvironmentExtensions
{
    public static EnvironmentWindow<LunarLanderEnvironment, Array1D<float>, int>
        Window(this LunarLanderEnvironment environment, int width = 1280, int height = 720) =>
        Window<LunarLanderEnvironment, Array1D<float>, int>(environment, width, height);

    public static EnvironmentWindow<IEnvironment<TO, TA>, TO, TA>
        Window<TO, TA>(this IEnvironment<TO, TA> environment, int width = 1280, int height = 720) =>
        Window<IEnvironment<TO, TA>, TO, TA>(environment, width, height);

    public static EnvironmentWindow<TE, TO, TA>
        Window<TE, TO, TA>(this TE environment, int width = 1280, int height = 720)
        where TE : IEnvironment<TO, TA> =>
        new(environment, width, height);
}
