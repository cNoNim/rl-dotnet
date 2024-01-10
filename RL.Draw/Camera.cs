using System.Numerics;

namespace RL.Draw;

public class Camera
{
    public Vector2 Center { get; set; }
    public float Zoom { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public void BuildProjectionMatrix(Span<float> m, float zBias)
    {
        float w = Width;
        float h = Height;
        var ratio = w / h;
        var extents = new Vector2(ratio * 25.0f, 25.0f);
        extents *= Zoom;

        var lower = Center - extents;
        var upper = Center + extents;

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
        m[15] = 1.0f;
    }
}
