using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace RL.Plot;

public class SimpleWindow
{
    private readonly WindowOptions _options;
    private readonly CancellationToken _token;
    private readonly List<ISubView> _subViews = [];

    private IWindow? _window;
    private GL? _gl;

    public SimpleWindow(
        string? title = default,
        int width = 1280,
        int height = 720,
        CancellationToken token = default
    )
    {
        _options = WindowOptions.Default;
        _options.Size = new Vector2D<int>(width, height);
        if (title != null)
            _options.Title = title;
        _token = token;
    }

    public void AddSubView(ISubView subView) =>
        _subViews.Add(subView);

    public void Run()
    {
        using var window = Window.Create(_options);
        try
        {
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
            _window = null;
        }
    }

    private void OnLoad()
    {
        if (_window == null)
            throw new InvalidOperationException();

        _gl = GL.GetApi(_window);
        _gl.ClearColor(Color.White);

        foreach (var subView in _subViews)
            subView.OnLoad();

        OnFramebufferResize(_window.FramebufferSize);
    }

    private void OnClosing()
    {
        foreach (var subView in _subViews)
            subView.OnClosing();
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

        if (_token.IsCancellationRequested)
            _window?.Close();
    }

    private void OnRender(double delta)
    {
        if (_gl == null)
            throw new InvalidOperationException();

        _gl.Clear(ClearBufferMask.ColorBufferBit);

        foreach (var subView in _subViews)
            subView.OnRender();
    }
}
