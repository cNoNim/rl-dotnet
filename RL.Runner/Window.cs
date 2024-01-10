using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Runtime.InteropServices;
using Box2D.NetStandard.Dynamics.Bodies;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RL.Draw;
using RL.Environments;
using Vector2 = System.Numerics.Vector2;

namespace RL.Runner;

public class Window(string title, int width, int height, bool debugContext = false) :
    GameWindow(
        new GameWindowSettings
        {
            UpdateFrequency = 50
        },
        new NativeWindowSettings
        {
            Title = title,
            ClientSize = new Vector2i(width, height),
            Profile = ContextProfile.Core,
            Flags = ContextFlags.ForwardCompatible | (debugContext ? ContextFlags.Debug : ContextFlags.Default)
        }
    ),
    IDebugDrawer
{
    private static readonly DebugProc DebugMessageDelegate = OnDebugMessage;

    private readonly ConcurrentQueue<Action> _drawActions = new();
    private Drawer? _drawer;
    private Body? _focusBody;
    public bool Paused;

    void IDebugDrawer.DrawPoint(Vector2 position, float size, Color color) =>
        _drawActions.Enqueue(() => _drawer?.Add(position, color, size));

    void IDebugDrawer.DrawPolygon(Vector2[] vertices, int vertexCount, Color color) =>
        _drawActions.Enqueue(() => DrawPolygon(vertices, vertexCount, color));

    void IDebugDrawer.DrawSolidPolygon(Vector2[] vertices, int vertexCount, Color color) =>
        _drawActions.Enqueue(() =>
        {
            if (_drawer == null)
                return;

            var fillColor = Color.FromArgb(color.A / 2, color.R / 2, color.G / 2, color.B / 2);

            for (var i = 1; i < vertexCount - 1; ++i)
                _drawer.Add(vertices[0], vertices[i], vertices[i + 1], fillColor);

            DrawPolygon(vertices, vertexCount, color);
        });

    void IDebugDrawer.DrawCircle(Vector2 center, float radius, Color color) =>
        _drawActions.Enqueue(() =>
        {
            if (_drawer == null)
                return;

            const float kSegments = 16.0f;
            const float kIncrement = 2 * MathF.PI / kSegments;
            var (sinInc, cosInc) = MathF.SinCos(kIncrement);

            var r1 = new Vector2(1.0f, 0.0f);
            var v1 = center + radius * r1;
            for (var i = 0; i < kSegments; ++i)
            {
                var r2 = new Vector2(cosInc * r1.X - sinInc * r1.Y, sinInc * r1.X + cosInc * r1.Y);
                var v2 = center + radius * r2;
                _drawer.Add(v1, v2, color);
                r1 = r2;
                v1 = v2;
            }
        });

    void IDebugDrawer.DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color) =>
        _drawActions.Enqueue(() =>
        {
            if (_drawer == null)
                return;

            var fillColor = Color.FromArgb(color.A / 2, color.R / 2, color.G / 2, color.B / 2);

            const float kSegments = 16.0f;
            const float kIncrement = 2 * MathF.PI / kSegments;
            var (sinInc, cosInc) = MathF.SinCos(kIncrement);
            var r1 = new Vector2(cosInc, sinInc);
            var v1 = center + radius * r1;
            for (var i = 0; i < kSegments; ++i)
            {
                var r2 = new Vector2(cosInc * r1.X - sinInc * r1.Y, sinInc * r1.X + cosInc * r1.Y);
                var v2 = center + radius * r2;
                _drawer.Add(center, v1, v2, fillColor);
                r1 = r2;
                v1 = v2;
            }

            r1 = new Vector2(1.0f, 0.0f);
            v1 = center + radius * r1;
            for (var i = 0; i < kSegments; ++i)
            {
                var r2 = new Vector2(cosInc * r1.X - sinInc * r1.Y, sinInc * r1.X + cosInc * r1.Y);
                var v2 = center + radius * r2;
                _drawer.Add(v1, v2, color);
                r1 = r2;
                v1 = v2;
            }

            var p = center + radius * axis;
            _drawer.Add(center, p, color);
        });

    void IDebugDrawer.DrawSegment(Vector2 p1, Vector2 p2, Color color) =>
        _drawActions.Enqueue(() => _drawer?.Add(p1, p2, color));

    void IDebugDrawer.DrawXForm(Vector2 p, float m11, float m12, float m21, float m22) =>
        _drawActions.Enqueue(() =>
        {
            if (_drawer == null)
                return;

            const float kAxisScale = 0.4f;

            var ex = new Vector2(m11, m21);
            var b = p + kAxisScale * ex;

            _drawer.Add(p, b, Color.Red);

            var ey = new Vector2(m12, m22);
            b = p + kAxisScale * ey;

            _drawer.Add(p, b, Color.Green);
        });

    public void SetBody(Body body) =>
        _focusBody = body;

    protected override void OnLoad()
    {
        base.OnLoad();

        if ((Flags & ContextFlags.Debug) != 0)
        {
            GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);
        }

        GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);

        _drawer = new Drawer(new Camera());
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        _drawer?.Dispose();
        _drawer = null;
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs args)
    {
        base.OnKeyDown(args);

        switch (args.Key)
        {
            case Keys.Escape:
                Close();
                break;
            case Keys.P:
                Paused = !Paused;
                break;
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        var drawer = _drawer;

        if (drawer != null)
        {
            if (_focusBody != null)
                drawer.Camera.Center = _focusBody.GetPosition();
            drawer.Camera.Zoom = 0.5f;
            drawer.Camera.Width = ClientSize.X;
            drawer.Camera.Height = ClientSize.Y;
        }

        while (_drawActions.TryDequeue(out var action))
            action.Invoke();

        drawer?.Flush();

        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs args)
    {
        base.OnResize(args);

        GL.Viewport(0, 0, args.Width, args.Height);
    }

    private void DrawPolygon(Vector2[] vertices, int vertexCount, Color color) =>
        DrawPolygon(new ReadOnlySpan<Vector2>(vertices, 0, vertexCount), color);

    private void DrawPolygon(ReadOnlySpan<Vector2> vertices, Color color)
    {
        if (_drawer == null)
            return;

        var p1 = vertices[^1];
        foreach (var p2 in vertices)
        {
            _drawer.Add(p1, p2, color);
            p1 = p2;
        }
    }

    private static void OnDebugMessage(
        DebugSource source,
        DebugType type,
        int id,
        DebugSeverity severity,
        int length,
        IntPtr pMessage,
        IntPtr pUserParam
    )
    {
        var message = Marshal.PtrToStringAnsi(pMessage, length);
        Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);
        if (type == DebugType.DebugTypeError)
            throw new Exception(message);
    }
}