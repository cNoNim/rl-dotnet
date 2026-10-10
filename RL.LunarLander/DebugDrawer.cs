using System;
using System.Collections.Concurrent;
using System.Drawing;
using RL.Draw;
using RL.Environments;
using Vector2 = System.Numerics.Vector2;

namespace RL.LunarLander;

public class DebugDrawer : IDebugDrawer
{
    private readonly ConcurrentQueue<Action<Drawer>> _drawActions = new();

    public void Draw(Drawer drawer)
    {
        while (_drawActions.TryDequeue(out var action))
            action.Invoke(drawer);
        drawer.Flush();
    }

    void IDebugDrawer.DrawPoint(Vector2 position, float size, Color color) =>
        _drawActions.Enqueue(drawer => drawer.Add(position, color, size));

    void IDebugDrawer.DrawPolygon(Vector2[] vertices, int vertexCount, Color color) =>
        _drawActions.Enqueue(drawer => DrawPolygon(drawer, vertices, vertexCount, color));

    void IDebugDrawer.DrawSolidPolygon(Vector2[] vertices, int vertexCount, Color color) =>
        _drawActions.Enqueue(drawer =>
        {
            var fillColor = Color.FromArgb(color.A / 2, color.R / 2, color.G / 2, color.B / 2);

            for (var i = 1; i < vertexCount - 1; ++i)
                drawer.Add(vertices[0], vertices[i], vertices[i + 1], fillColor);

            DrawPolygon(drawer, vertices, vertexCount, color);
        });

    void IDebugDrawer.DrawCircle(Vector2 center, float radius, Color color) =>
        _drawActions.Enqueue(drawer =>
        {
            const float kSegments = 16.0f;
            const float kIncrement = 2 * MathF.PI / kSegments;
            var (sinInc, cosInc) = MathF.SinCos(kIncrement);

            var r1 = new Vector2(1.0f, 0.0f);
            var v1 = center + radius * r1;
            for (var i = 0; i < kSegments; ++i)
            {
                var r2 = new Vector2(cosInc * r1.X - sinInc * r1.Y, sinInc * r1.X + cosInc * r1.Y);
                var v2 = center + radius * r2;
                drawer.Add(v1, v2, color);
                r1 = r2;
                v1 = v2;
            }
        });

    void IDebugDrawer.DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color) =>
        _drawActions.Enqueue(drawer =>
        {
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
                drawer.Add(center, v1, v2, fillColor);
                r1 = r2;
                v1 = v2;
            }

            r1 = new Vector2(1.0f, 0.0f);
            v1 = center + radius * r1;
            for (var i = 0; i < kSegments; ++i)
            {
                var r2 = new Vector2(cosInc * r1.X - sinInc * r1.Y, sinInc * r1.X + cosInc * r1.Y);
                var v2 = center + radius * r2;
                drawer.Add(v1, v2, color);
                r1 = r2;
                v1 = v2;
            }

            var p = center + radius * axis;
            drawer.Add(center, p, color);
        });

    void IDebugDrawer.DrawSegment(Vector2 p1, Vector2 p2, Color color) =>
        _drawActions.Enqueue(drawer => drawer.Add(p1, p2, color));

    void IDebugDrawer.DrawXForm(Vector2 p, float m11, float m12, float m21, float m22) =>
        _drawActions.Enqueue(drawer =>
        {
            const float kAxisScale = 0.4f;

            var ex = new Vector2(m11, m21);
            var b = p + kAxisScale * ex;

            drawer.Add(p, b, Color.Red);

            var ey = new Vector2(m12, m22);
            b = p + kAxisScale * ey;

            drawer.Add(p, b, Color.Green);
        });

    private void DrawPolygon(Drawer drawer, Vector2[] vertices, int vertexCount, Color color) =>
        DrawPolygon(drawer, new ReadOnlySpan<Vector2>(vertices, 0, vertexCount), color);

    private void DrawPolygon(Drawer drawer, ReadOnlySpan<Vector2> vertices, Color color)
    {
        var p1 = vertices[^1];
        foreach (var p2 in vertices)
        {
            drawer.Add(p1, p2, color);
            p1 = p2;
        }
    }
}
