using System.Drawing;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Vector2 = System.Numerics.Vector2;

namespace RL.Draw;

public partial class Drawer(Camera camera) : IDisposable
{
    private readonly Lines _lines = new(camera);
    private readonly Points _points = new(camera);
    private readonly Triangles _triangles = new(camera);
    private bool _disposed;

    public Camera Camera => camera;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposing || _disposed)
            return;
        _points.Destroy();
        _lines.Destroy();
        _triangles.Destroy();
        _disposed = true;
    }

    ~Drawer() => Dispose(false);

    public void Flush()
    {
        _triangles.Flush();
        _lines.Flush();
        _points.Flush();
    }

    public void Add(Vector2 vertex, Color color, float size) =>
        _points.Add(vertex, color, size);

    public void Add(Vector2 v1, Vector2 v2, Color color) =>
        _lines.Add(v1, v2, color);

    public void Add(Vector2 v1, Vector2 v2, Vector2 v3, Color color) =>
        _triangles.Add(v1, v2, v3, color);

    private static void CheckError()
    {
        var errorCode = GL.GetError();
        if (errorCode != ErrorCode.NoError)
            throw new Exception($"{errorCode}");
    }

    private static int CreateShaderFromString(string source, ShaderType type)
    {
        var shaderId = GL.CreateShader(type);
        GL.ShaderSource(shaderId, source);
        GL.CompileShader(shaderId);
        GL.GetShader(shaderId, ShaderParameter.CompileStatus, out var compileStatus);
        if (compileStatus != 0)
            return shaderId;
        var infoLog = GL.GetShaderInfoLog(shaderId);
        GL.DeleteShader(shaderId);
        if (infoLog != null)
            throw new Exception($"Error compiling shader of type {type}: {infoLog}");
        throw new Exception($"Error compiling shader of type {type}!");
    }

    private static int CreateShaderProgram(string vs, string fs)
    {
        var vsId = CreateShaderFromString(vs, ShaderType.VertexShader);
        var fsId = CreateShaderFromString(fs, ShaderType.FragmentShader);

        var programId = GL.CreateProgram();
        GL.AttachShader(programId, vsId);
        GL.AttachShader(programId, fsId);
        GL.BindFragDataLocation(programId, 0, "color");
        GL.LinkProgram(programId);

        GL.DeleteShader(vsId);
        GL.DeleteShader(fsId);

        GL.GetProgram(programId, GetProgramParameterName.LinkStatus, out var linkStatus);
        if (linkStatus != 0)
            return programId;
        var infoLog = GL.GetProgramInfoLog(programId);
        GL.DeleteProgram(programId);
        if (infoLog != null)
            throw new Exception($"Error linking program: {infoLog}");
        throw new Exception($"Error linking program!");
    }

    private static Vector4h ConvertColor(Color color) =>
        new(
            color.R / (float)byte.MaxValue,
            color.G / (float)byte.MaxValue,
            color.B / (float)byte.MaxValue,
            color.A / (float)byte.MaxValue
        );

    private static OpenTK.Mathematics.Vector2 ConvertVector(Vector2 position) =>
        Unsafe.As<Vector2, OpenTK.Mathematics.Vector2>(ref position);
}
