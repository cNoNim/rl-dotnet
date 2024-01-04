using System.Drawing;
using System.Numerics;

namespace RL.Environments;

public interface IDebugDrawer
{
    public void DrawPoint(Vector2 position, float size, Color color);

    public void DrawPolygon(Vector2[] vertices, int vertexCount, Color color);

    public void DrawSolidPolygon(Vector2[] vertices, int vertexCount, Color color);

    public void DrawCircle(Vector2 center, float radius, Color color);

    public void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color);

    public void DrawSegment(Vector2 p1, Vector2 p2, Color color);

    public void DrawXForm(Vector2 p, float m11, float m12, float m21, float m22);
}