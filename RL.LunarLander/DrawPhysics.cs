using System;
using System.Linq;
using System.Numerics;
using Box2D.NetStandard.Common;
using Box2D.NetStandard.Dynamics.World;
using Box2D.NetStandard.Dynamics.World.Callbacks;
using RL.Environments;

namespace RL.LunarLander;

public class DrawPhysics(IDebugDrawer drawer) : DebugDraw
{
    public override void DrawTransform(in Transform xf) =>
        drawer.DrawXForm(xf.p, xf.q.M11, xf.q.M12, xf.q.M21, xf.q.M22);

    public override void DrawPoint(in Vector2 position, float size, in Color color) =>
        drawer.DrawPoint(position, size, ConvertColor(color));

    [Obsolete("Look out for new calls using Vector2")]
    public override void DrawPolygon(in Vec2[] vertices, int vertexCount, in Color color) =>
        drawer.DrawPolygon(vertices.Select(v => (Vector2)v).ToArray(), vertexCount, ConvertColor(color));

    [Obsolete("Look out for new calls using Vector2")]
    public override void DrawSolidPolygon(in Vec2[] vertices, int vertexCount, in Color color) =>
        drawer.DrawSolidPolygon(vertices.Select(v => (Vector2)v).ToArray(), vertexCount, ConvertColor(color));

    [Obsolete("Look out for new calls using Vector2")]
    public override void DrawCircle(in Vec2 center, float radius, in Color color) =>
        drawer.DrawCircle(center, radius, ConvertColor(color));

    [Obsolete("Look out for new calls using Vector2")]
    public override void DrawSolidCircle(in Vec2 center, float radius, in Vec2 axis, in Color color) =>
        drawer.DrawSolidCircle(center, radius, axis, ConvertColor(color));

    [Obsolete("Look out for new calls using Vector2")]
    public override void DrawSegment(in Vec2 p1, in Vec2 p2, in Color color) =>
        drawer.DrawSegment(p1, p2, ConvertColor(color));

    private static System.Drawing.Color ConvertColor(Color color) =>
        System.Drawing.Color.FromArgb(
            (int)(color.A * 255),
            (int)(color.R * 255),
            (int)(color.G * 255),
            (int)(color.B * 255)
        );
}
