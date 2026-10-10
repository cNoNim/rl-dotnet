using System.Drawing;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace RL.Draw;

public partial class Drawer
{
    private class Lines
    {
        private const int MaxVertices = 2 * 512;
        private readonly GL _gl;
        private readonly IProjectionMatrixBuilder _projectionMatrixBuilder;
        private readonly Vector4D<Half>[] _colors = new Vector4D<Half>[MaxVertices];
        private readonly int _projectionUniform;
        private readonly uint[] _vboIds = new uint[2];
        private readonly Vector2D<float>[] _vertices = new Vector2D<float>[MaxVertices];

        private uint _count;
        private uint _programId;

        private uint _vaoId;

        public unsafe Lines(GL gl, IProjectionMatrixBuilder projectionMatrixBuilder)
        {
            _gl = gl;
            _projectionMatrixBuilder = projectionMatrixBuilder;

            const uint vertexAttribute = 0;
            const uint colorAttribute = 1;

            //language=shader
            const string vs =
                """
                #version 330 core
                uniform mat4 projectionMatrix;
                layout(location = 0) in vec2 v_position;
                layout(location = 1) in vec4 v_color;
                out vec4 f_color;

                void main(void)
                {
                    f_color = v_color;
                    gl_Position = projectionMatrix * vec4(v_position, 0.0f, 1.0f);
                }

                """;

            //language=shader
            const string fs =
                """
                #version 330 core
                in vec4 f_color;
                out vec4 color;

                void main(void)
                {
                    color = f_color;
                }

                """;

            _programId = CreateShaderProgram(gl, vs, fs);

            _projectionUniform = _gl.GetUniformLocation(_programId, "projectionMatrix");

            // Generate
            _vaoId = gl.GenVertexArray();
            gl.GenBuffers((uint)_vboIds.Length, _vboIds);

            gl.BindVertexArray(_vaoId);
            gl.EnableVertexAttribArray(vertexAttribute);
            gl.EnableVertexAttribArray(colorAttribute);

            // Vertex buffer
            gl.BindBuffer(GLEnum.ArrayBuffer, _vboIds[0]);
            gl.VertexAttribPointer(vertexAttribute, 2, VertexAttribPointerType.Float, false, 0, null);
            gl.BufferData<Vector2D<float>>(GLEnum.ArrayBuffer, _vertices, GLEnum.DynamicDraw);

            gl.BindBuffer(GLEnum.ArrayBuffer, _vboIds[1]);
            gl.VertexAttribPointer(colorAttribute, 4, VertexAttribPointerType.HalfFloat, false, 0, null);
            gl.BufferData<Vector4D<Half>>(GLEnum.ArrayBuffer, _colors, GLEnum.DynamicDraw);

            CheckError(gl);

            // Cleanup
            gl.BindBuffer(GLEnum.ArrayBuffer, 0);
            gl.BindVertexArray(0);

            _count = 0;
        }

        public void Destroy()
        {
            if (_vaoId != 0)
            {
                _gl.DeleteVertexArray(_vaoId);
                _gl.DeleteBuffers((uint)_vboIds.Length, _vboIds);
                _vaoId = 0;
            }

            if (_programId != 0)
            {
                _gl.DeleteProgram(_programId);
                _programId = 0;
            }
        }

        public void Add(System.Numerics.Vector2 v1, System.Numerics.Vector2 v2, Color color)
        {
            if (_count == MaxVertices)
                Flush();

            var c = ConvertColor(color);
            _vertices[_count] = ConvertVector(v1);
            _colors[_count] = c;
            _count++;
            _vertices[_count] = ConvertVector(v2);
            _colors[_count] = c;
            _count++;
        }

        public void Flush()
        {
            if (_count == 0)
                return;

            _gl.UseProgram(_programId);

            Span<float> matrix = stackalloc float[16];
            _projectionMatrixBuilder.BuildProjectionMatrix(matrix, 0.1f);
            _gl.UniformMatrix4(_projectionUniform, 1, false, matrix);

            _gl.BindVertexArray(_vaoId);

            _gl.BindBuffer(GLEnum.ArrayBuffer, _vboIds[0]);
            _gl.BufferSubData<Vector2D<float>>(GLEnum.ArrayBuffer, 0, _vertices);

            _gl.BindBuffer(GLEnum.ArrayBuffer, _vboIds[1]);
            _gl.BufferSubData<Vector4D<Half>>(GLEnum.ArrayBuffer, 0, _colors);

            _gl.DrawArrays(PrimitiveType.Lines, 0, _count);

            CheckError(_gl);

            _gl.BindBuffer(GLEnum.ArrayBuffer, 0);
            _gl.BindVertexArray(0);
            _gl.UseProgram(0);

            _count = 0;
        }
    }
}
