using System;
using System.Collections.Generic;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RL.Algorithms;
using RL.Box2D;
using RL.Environments;
using RL.MDArrays;

namespace RL.Runner;

public class EnvironmentWindow<TE, TO, TA>(TE environment, int width, int height)
    where TE : IEnvironment<TO, TA>
{
    private readonly Dictionary<Keys, IAgent<TO, TA>> _agents = [];

    private bool _needReset = true;
    private IAgent<TO, TA>? _nextAgent;
    private Window? _window;

    public TE Environment => environment;
    public Window Window => _window ?? throw new InvalidOperationException();
    public IAgent<TO, TA>? Agent { get; private set; }
    public float TotalReward { get; private set; }
    public int Episode { get; private set; } = -1;

    public event Action<EnvironmentWindow<TE, TO, TA>>? LoadEvent;
    public event Action<EnvironmentWindow<TE, TO, TA>>? UpdateEvent;
    public event Action<EnvironmentWindow<TE, TO, TA>>? StepEvent;
    public event Action<EnvironmentWindow<TE, TO, TA>>? CompleteEvent;
    public event Action<EnvironmentWindow<TE, TO, TA>>? UnloadEvent;

    public void AddAgent(Keys key, IAgent<TO, TA> agent)
    {
        _nextAgent = agent;
        _agents.Add(key, agent);
    }

    public void Run()
    {
        using var window = new Window(Environment.Name, width, height);
        window.Load += OnLoad;
        window.UpdateFrame += OnUpdateFrame;
        window.Unload += OnUnload;
        window.VSync = VSyncMode.Off;
        _window = window;
        window.Run();
        _window = null;
    }

    private void OnLoad() =>
        LoadEvent?.Invoke(this);

    private void Reset()
    {
        if (_nextAgent != null)
        {
            Agent = _nextAgent;
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
                Agent = agent.Value;

        if (Agent != null && !Window.Paused)
        {
            var action = Agent.Predict(Environment.State);
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
        foreach (var agent in _agents)
        {
            if (agent.Value is IDisposable d)
                d.Dispose();
        }

        _agents.Clear();
        if (_nextAgent is IDisposable disposable)
            disposable.Dispose();
        _nextAgent = null;
        Agent = null;

        UnloadEvent?.Invoke(this);
        UnloadEvent = null;
        Window.UpdateFrame += OnUpdateFrame;
        Window.Unload += OnUnload;
    }

    public void Dispose()
    {
        Window.Dispose();
        GC.SuppressFinalize(this);
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
