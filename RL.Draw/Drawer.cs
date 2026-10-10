using System.Drawing;
using System.Runtime.CompilerServices;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Vector2 = System.Numerics.Vector2;

namespace RL.Draw;

public partial class Drawer : IDisposable
{
    private readonly Points _points;
    private readonly Lines _lines;
    private readonly Triangles _triangles;
    private bool _disposed;

    public Drawer(GL gl, IProjectionMatrixBuilder projectionMatrixBuilder)
    {
        _points = new Points(gl, projectionMatrixBuilder);
        _lines = new Lines(gl, projectionMatrixBuilder);
        _triangles = new Triangles(gl, projectionMatrixBuilder);
    }

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

    private static void CheckError(GL gl)
    {
        var errorCode = gl.GetError();
        if (errorCode != GLEnum.NoError)
            throw new Exception($"{errorCode}");
    }

    private static uint CreateShaderFromString(GL gl, string source, ShaderType type)
    {
        var shaderId = gl.CreateShader(type);
        gl.ShaderSource(shaderId, source);
        gl.CompileShader(shaderId);
        gl.GetShader(shaderId, GLEnum.CompileStatus, out var compileStatus);
        if (compileStatus != 0)
            return shaderId;
        var infoLog = gl.GetShaderInfoLog(shaderId);
        gl.DeleteShader(shaderId);
        if (infoLog != null)
            throw new Exception($"Error compiling shader of type {type}: {infoLog}");
        throw new Exception($"Error compiling shader of type {type}!");
    }

    private static uint CreateShaderProgram(GL gl, string vs, string fs)
    {
        var vsId = CreateShaderFromString(gl, vs, ShaderType.VertexShader);
        var fsId = CreateShaderFromString(gl, fs, ShaderType.FragmentShader);

        var programId = gl.CreateProgram();
        gl.AttachShader(programId, vsId);
        gl.AttachShader(programId, fsId);
        gl.BindFragDataLocation(programId, 0, "color");
        gl.LinkProgram(programId);

        gl.DeleteShader(vsId);
        gl.DeleteShader(fsId);

        gl.GetProgram(programId, GLEnum.LinkStatus, out var linkStatus);
        if (linkStatus != 0)
            return programId;
        var infoLog = gl.GetProgramInfoLog(programId);
        gl.DeleteProgram(programId);
        if (infoLog != null)
            throw new Exception($"Error linking program: {infoLog}");
        throw new Exception($"Error linking program!");
    }

    private static Vector4D<Half> ConvertColor(Color color) =>
        new(
            color.R / (Half)byte.MaxValue,
            color.G / (Half)byte.MaxValue,
            color.B / (Half)byte.MaxValue,
            color.A / (Half)byte.MaxValue
        );

    private static Vector2D<float> ConvertVector(Vector2 position) =>
        Unsafe.As<Vector2, Vector2D<float>>(ref position);
}
