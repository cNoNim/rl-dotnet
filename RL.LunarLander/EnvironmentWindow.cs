using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Box2D.NetStandard.Dynamics.Bodies;
using RL.Algorithms;
using RL.Draw;
using RL.Environments;
using RL.Plot;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace RL.LunarLander;

public class EnvironmentWindow<TE, TO, TA>(int width = 1280, int height = 720) :
    IDisposable,
    IProjectionMatrixBuilder
    where TE : IEnvironment<TO, TA>
{
    private readonly List<ISubView> _subViews = [];
    private readonly DebugDrawer _debugDrawer = new();

    private bool _needReset = true;
    private IAgent<TO, TA>? _nextAgent;
    private IAgent<TO, TA>? _agent;
    private TE? _environment;
    private IWindow? _window;
    private GL? _gl;
    private Drawer? _drawer;
    private Body? _focusBody;

    public TE Environment => _environment ?? throw new InvalidOperationException();
    public IDebugDrawer DebugDrawer => _debugDrawer;
    public event Action<EnvironmentWindow<TE, TO, TA>>? LoadEvent;
    public event Action<EnvironmentWindow<TE, TO, TA>>? UnloadEvent;
    public event Action<EnvironmentWindow<TE, TO, TA>>? UpdateEvent;

    public void AddSubView(ISubView subView) =>
        _subViews.Add(subView);

    public void NextAgent(IAgent<TO, TA> agent) =>
        _nextAgent = agent;

    public void Run(TE environment, bool debug = false)
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(width, height);
        options.Title = environment.Name;
        var api = GraphicsAPI.Default;
        if (debug)
            api.Flags |= ContextFlags.Debug;
        options.API = api;

        using var window = Window.Create(options);
        try
        {
            _environment = environment;
            _window = window;
            window.Load += OnLoad;
            window.Closing += OnClosing;
            window.FramebufferResize += OnFramebufferResize;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.Run();
        }
        finally
        {
            _environment = default;
            _window = null;
        }
    }

    public void SetBody(Body body) =>
        _focusBody = body;

    private void OnLoad()
    {
        if (_window == null)
            throw new InvalidCastException();

        _gl = GL.GetApi(_window);

        if ((_window.API.Flags & ContextFlags.Debug) != 0)
        {
            _gl.Enable(EnableCap.DebugOutput);
            _gl.Enable(EnableCap.DebugOutputSynchronous);
            _gl.DebugMessageCallback(OnDebugMessage, IntPtr.Zero);
        }

        _gl.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);

        _drawer = new Drawer(_gl, this);

        foreach (var subView in _subViews)
            subView.OnLoad();

        LoadEvent?.Invoke(this);

        OnFramebufferResize(_window.FramebufferSize);
    }

    private void OnClosing()
    {
        foreach (var subView in _subViews)
            subView.OnClosing();

        _drawer?.Dispose();
        _drawer = null;

        UnloadEvent?.Invoke(this);
        UnloadEvent = null;
    }

    private void OnFramebufferResize(Vector2D<int> framebufferSize)
    {
        if (_gl == null)
            throw new InvalidOperationException();
        _gl.Viewport(framebufferSize);

        foreach (var subView in _subViews)
            subView.OnFramebufferResize(framebufferSize);
    }

    private void OnUpdate(double delta)
    {
        foreach (var subView in _subViews)
            subView.OnUpdate();

        if (_needReset)
            Reset();

        if (_environment != null)
        {
            var action = _agent != null
                ? _agent.Predict(_environment.Observation)
                : _environment.SampleAction(_environment.Random);
            var transition = _environment.Step(action);
            if (transition.Terminated || transition.Truncated)
                _needReset = true;
        }

        UpdateEvent?.Invoke(this);
    }

    private void OnRender(double delta)
    {
        if (_window == null || _gl == null)
            throw new InvalidOperationException();

        _gl.Clear(ClearBufferMask.ColorBufferBit);

        var drawer = _drawer;
        if (drawer != null)
            _debugDrawer.Draw(drawer);

        foreach (var subView in _subViews)
        {
            try
            {
                subView.OnRender();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private void Reset()
    {
        if (_nextAgent != null)
        {
            _agent = _nextAgent;
            _nextAgent = null;
        }

        _needReset = false;
        _environment?.Reset();
    }

    public void Dispose()
    {
        Release(ref _nextAgent);
        Release(ref _agent);
        GC.SuppressFinalize(this);
    }

    private static void Release<T>(ref T? agent) where T : class
    {
        if (agent is IDisposable disposable)
            disposable.Dispose();
        agent = null;
    }

    public void BuildProjectionMatrix(Span<float> m, float zBias)
    {
        if (_window == null)
            throw new NotImplementedException();

        float w = _window.FramebufferSize.X;
        float h = _window.FramebufferSize.Y;
        var ratio = w / h;
        var extents = new Vector2D<float>(ratio * 25.0f, 25.0f);
        extents *= 0.5f;

        var center = Vector2D<float>.Zero;
        if (_focusBody != null)
        {
            var position = _focusBody.GetPosition();
            center.X = position.X;
            center.Y = position.Y;
        }
        var lower = center - extents;
        var upper = center + extents;

        m[0] = 2.0f / (upper.X - lower.X);
        m[1] = 0.0f;
        m[2] = 0.0f;
        m[3] = 0.0f;

        m[4] = 0.0f;
        m[5] = 2.0f / (upper.Y - lower.Y);
        m[6] = 0.0f;
        m[7] = 0.0f;

        m[8] = 0.0f;
        m[9] = 0.0f;
        m[10] = 1.0f;
        m[11] = 0.0f;

        m[12] = -(upper.X + lower.X) / (upper.X - lower.X);
        m[13] = -(upper.Y + lower.Y) / (upper.Y - lower.Y);
        m[14] = zBias;
        m[15] = 1.0f;    }

    private static void OnDebugMessage(
        GLEnum source,
        GLEnum type,
        int id,
        GLEnum severity,
        int length,
        IntPtr pMessage,
        IntPtr pUserParam
    )
    {
        var message = Marshal.PtrToStringAnsi(pMessage, length);
        Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);
        if (type == (GLEnum)DebugType.DebugTypeError)
            throw new Exception(message);
    }
}
