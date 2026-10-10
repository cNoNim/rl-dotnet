using System.Drawing;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace RL.Draw;

public partial class Drawer
{
    private class Points
    {
        private const int MaxVertices = 512;
        private readonly GL _gl;
        private readonly IProjectionMatrixBuilder _projectionMatrixBuilder;
        private readonly Vector4D<Half>[] _colors = new Vector4D<Half>[MaxVertices];
        private readonly int _projectionUniform;
        private readonly Half[] _sizes = new Half[MaxVertices];
        private readonly uint[] _vboIds = new uint[3];
        private readonly Vector2D<float>[] _vertices = new Vector2D<float>[MaxVertices];

        private uint _count;
        private uint _programId;

        private uint _vaoId;

        public unsafe Points(GL gl, IProjectionMatrixBuilder projectionMatrixBuilder)
        {
            _gl = gl;
            _projectionMatrixBuilder = projectionMatrixBuilder;

            const int vertexAttribute = 0;
            const int colorAttribute = 1;
            const int sizeAttribute = 2;

            //language=shader
            const string vs =
                """
                #version 330 core
                uniform mat4 projectionMatrix;
                layout(location = 0) in vec2 v_position;
                layout(location = 1) in vec4 v_color;
                layout(location = 2) in float v_size;
                out vec4 f_color;

                void main(void)
                {
                    f_color = v_color;
                    gl_Position = projectionMatrix * vec4(v_position, 0.0f, 1.0f);
                    gl_PointSize = v_size;
                }

                """;


            //language=shader
            const string fs =
                """
                #version 330 core
                in vec4 f_color;
                out vec4 color;

                void main()
                {
                    color = f_color;
                }

                """;

            _programId = CreateShaderProgram(gl, vs, fs);

            _projectionUniform = gl.GetUniformLocation(_programId, "projectionMatrix");

            // Generate
            _vaoId = gl.GenVertexArray();
            gl.GenBuffers((uint)_vboIds.Length, _vboIds);

            gl.BindVertexArray(_vaoId);
            gl.EnableVertexAttribArray(vertexAttribute);
            gl.EnableVertexAttribArray(colorAttribute);
            gl.EnableVertexAttribArray(sizeAttribute);

            gl.BindBuffer(GLEnum.ArrayBuffer, _vboIds[0]);
            gl.VertexAttribPointer(vertexAttribute, 2, VertexAttribPointerType.Float, false, 0, null);
            gl.BufferData<Vector2D<float>>(GLEnum.ArrayBuffer, _vertices, GLEnum.DynamicDraw);

            gl.BindBuffer(GLEnum.ArrayBuffer, _vboIds[1]);
            gl.VertexAttribPointer(colorAttribute, 4, VertexAttribPointerType.HalfFloat, false, 0, null);
            gl.BufferData<Vector4D<Half>>(GLEnum.ArrayBuffer, _colors, GLEnum.DynamicDraw);

            gl.BindBuffer(GLEnum.ArrayBuffer, _vboIds[2]);
            gl.VertexAttribPointer(sizeAttribute, 1, VertexAttribPointerType.HalfFloat, false, 0, null);
            gl.BufferData<Half>(GLEnum.ArrayBuffer, _sizes, GLEnum.DynamicDraw);

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

        public void Add(System.Numerics.Vector2 vertex, Color color, float size)
        {
            if (_count == MaxVertices)
                Flush();

            _vertices[_count] = ConvertVector(vertex);
            _colors[_count] = ConvertColor(color);
            _sizes[_count] = (Half)size;
            _count++;
        }

        public void Flush()
        {
            if (_count == 0)
                return;

            _gl.UseProgram(_programId);

            Span<float> matrix = stackalloc float[16];
            _projectionMatrixBuilder.BuildProjectionMatrix(matrix, 0.0f);
            _gl.UniformMatrix4(_projectionUniform, 1, false, matrix);

            _gl.BindVertexArray(_vaoId);

            _gl.BindBuffer(GLEnum.ArrayBuffer, _vboIds[0]);
            _gl.BufferSubData<Vector2D<float>>(GLEnum.ArrayBuffer, 0, _vertices);

            _gl.BindBuffer(GLEnum.ArrayBuffer, _vboIds[1]);
            _gl.BufferSubData<Vector4D<Half>>(GLEnum.ArrayBuffer, 0, _colors);

            _gl.BindBuffer(GLEnum.ArrayBuffer, _vboIds[2]);
            _gl.BufferSubData<Half>(GLEnum.ArrayBuffer, 0, _sizes);

            _gl.Enable(EnableCap.ProgramPointSize);
            _gl.DrawArrays(PrimitiveType.Points, 0, _count);
            _gl.Disable(EnableCap.ProgramPointSize);

            CheckError(_gl);

            _gl.BindBuffer(GLEnum.ArrayBuffer, 0);
            _gl.BindVertexArray(0);
            _gl.UseProgram(0);

            _count = 0;
        }
    }
}
