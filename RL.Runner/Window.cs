using System;
using System.Collections.Concurrent;
using System.Drawing;
using Box2D.NetStandard.Common;
using Box2D.NetStandard.Dynamics.Bodies;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RL.Environments;
using Vector2 = System.Numerics.Vector2;

namespace RL.Runner;

public class Window(string title, int width, int height, Body? focusBody = null) :
    GameWindow(
        new GameWindowSettings
        {
            UpdateFrequency = 50
        },
        new NativeWindowSettings
        {
            Title = title,
            ClientSize = new Vector2i(width, height),
            Profile = ContextProfile.Compatability
        }
    ),
    IDebugDrawer
{
    private readonly ConcurrentQueue<Action> _drawActions = new();
    public bool Paused;

    void IDebugDrawer.DrawPoint(Vector2 position, float size, Color color) =>
        _drawActions.Enqueue(() =>
        {
            GL.Color4(color.R, color.G, color.B, (byte)128);
            GL.Begin(PrimitiveType.Points);
            GL.Vertex2(position.X, position.Y);
            GL.End();
        });

    void IDebugDrawer.DrawPolygon(Vector2[] vertices, int vertexCount, Color color) =>
        _drawActions.Enqueue(() =>
        {
            GL.Color4(color.R, color.G, color.B, (byte)128);
            GL.Begin(PrimitiveType.LineLoop);

            for (var i = 0; i < vertexCount; i++)
            {
                var vertex = vertices[i];
                GL.Vertex2(vertex.X, vertex.Y);
            }

            GL.End();
        });

    void IDebugDrawer.DrawSolidPolygon(Vector2[] vertices, int vertexCount, Color color) =>
        _drawActions.Enqueue(() =>
        {
            GL.Color4(color.R, color.G, color.B, (byte)128);
            GL.Begin(PrimitiveType.TriangleFan);

            for (var i = 0; i < vertexCount; i++)
            {
                var vertex = vertices[i];
                GL.Vertex2(vertex.X, vertex.Y);
            }

            GL.End();
        });

    void IDebugDrawer.DrawCircle(Vector2 center, float radius, Color color) =>
        _drawActions.Enqueue(() =>
        {
            const float kSegments = 16.0f;
            const int vertexCount = 16;
            const float kIncrement = Settings.Tau / kSegments;
            var theta = 0.0f;

            GL.Color4(color.R, color.G, color.B, (byte)128);
            GL.Begin(PrimitiveType.LineLoop);
            GL.VertexPointer(vertexCount * 2, VertexPointerType.Float, 0, IntPtr.Zero);

            for (var i = 0; i < kSegments; ++i)
            {
                var x = (float)Math.Cos(theta);
                var y = (float)Math.Sin(theta);
                var vertex = center + radius * new Vector2(x, y);

                GL.Vertex2(vertex.X, vertex.Y);

                theta += kIncrement;
            }

            GL.End();
        });

    void IDebugDrawer.DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color) =>
        _drawActions.Enqueue(() =>
        {
            const float kSegments = 16.0f;
            const int vertexCount = 16;
            const float kIncrement = Settings.Tau / kSegments;

            var theta = 0.0f;

            GL.Color4(color.R, color.G, color.B, (byte)128);
            GL.Begin(PrimitiveType.TriangleFan);
            GL.VertexPointer(vertexCount * 2, VertexPointerType.Float, 0, IntPtr.Zero);

            for (var i = 0; i < kSegments; ++i)
            {
                var x = (float)Math.Cos(theta);
                var y = (float)Math.Sin(theta);
                var vertex = center + radius * new Vector2(x, y);

                GL.Vertex2(vertex.X, vertex.Y);

                theta += kIncrement;
            }

            GL.End();

            DrawSegment(center, center + radius * axis, color);
        });

    void IDebugDrawer.DrawSegment(Vector2 p1, Vector2 p2, Color color) =>
        _drawActions.Enqueue(() => DrawSegment(p1, p2, color));

    void IDebugDrawer.DrawXForm(Vector2 p, float m11, float m12, float m21, float m22) =>
        _drawActions.Enqueue(() =>
        {
            const float kAxisScale = 0.4f;

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex2(p.X, p.Y);

            var ex = new Vector2(m11, m21);
            var b = p + kAxisScale * ex;

            GL.Vertex2(b.X, b.Y);
            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Vertex2(p.X, p.Y);

            var ey = new Vector2(m12, m22);
            b = p + kAxisScale * ey;

            GL.Vertex2(b.X, b.Y);
            GL.End();
        });

    public void SetBody(Body body) =>
        focusBody = body;

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

        GL.ClearColor(Color.CornflowerBlue);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.LoadIdentity();
        var transform = Matrix4.Identity;
        if (focusBody != null)
        {
            var position = focusBody.GetPosition();
            transform = Matrix4.Mult(transform, Matrix4.CreateTranslation(-position.X, -position.Y, 0));
            var ratio = ClientSize.X / (float)ClientSize.Y;
            const float zoom = 0.05f;
            transform = Matrix4.Mult(transform, Matrix4.CreateScale(zoom, zoom * ratio, 1.0f));
        }

        GL.MultMatrix(ref transform);

        while (_drawActions.TryDequeue(out var action))
            action.Invoke();

        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs args)
    {
        base.OnResize(args);

        GL.Viewport(0, 0, args.Width, args.Height);
    }

    private static void DrawSegment(Vector2 p1, Vector2 p2, Color color)
    {
        GL.Color4(color.R, color.G, color.B, (byte)255);
        GL.Begin(PrimitiveType.Lines);
        GL.Vertex2(p1.X, p1.Y);
        GL.Vertex2(p2.X, p2.Y);
        GL.End();
    }
}