using System.Drawing;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Half = System.Half;

namespace RL.Draw;

public partial class Drawer
{
    private class Points
    {
        private const int MaxVertices = 512;
        private readonly Camera _camera;
        private readonly Vector4h[] _colors = new Vector4h[MaxVertices];
        private readonly int _projectionUniform;
        private readonly Half[] _sizes = new Half[MaxVertices];
        private readonly int[] _vboIds = new int[3];
        private readonly Vector2[] _vertices = new Vector2[MaxVertices];

        private int _count;
        private int _programId;

        private int _vaoId;

        public Points(Camera camera)
        {
            _camera = camera;

            const int vertexAttribute = 0;
            const int colorAttribute = 1;
            const int sizeAttribute = 2;

            //language=shader
            const string vs =
                """
                #version 330 core
                uniform mat4 projectionMatrix;
                layout(location = 0) in vec2 v_position;
                layout(location = 1) in half4 v_color;
                layout(location = 2) in half v_size;
                out half4 f_color;

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
                in half4 f_color;
                out half4 color;

                void main()
                {
                    color = f_color;
                }

                """;

            _programId = CreateShaderProgram(vs, fs);

            _projectionUniform = GL.GetUniformLocation(_programId, "projectionMatrix");

            // Generate
            _vaoId = GL.GenVertexArray();
            GL.GenBuffers(_vboIds.Length, _vboIds);

            GL.BindVertexArray(_vaoId);
            GL.EnableVertexAttribArray(vertexAttribute);
            GL.EnableVertexAttribArray(colorAttribute);
            GL.EnableVertexAttribArray(sizeAttribute);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboIds[0]);
            GL.VertexAttribPointer(vertexAttribute, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * Vector2.SizeInBytes, _vertices,
                BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboIds[1]);
            GL.VertexAttribPointer(colorAttribute, 4, VertexAttribPointerType.HalfFloat, false, 0, 0);
            GL.BufferData(BufferTarget.ArrayBuffer, _colors.Length * Vector4h.SizeInBytes, _colors,
                BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboIds[2]);
            GL.VertexAttribPointer(sizeAttribute, 1, VertexAttribPointerType.HalfFloat, false, 0, 0);
            GL.BufferData(BufferTarget.ArrayBuffer, _sizes.Length * Unsafe.SizeOf<Half>(), _sizes,
                BufferUsageHint.DynamicDraw);

            CheckError();

            // Cleanup
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            _count = 0;
        }

        public void Destroy()
        {
            if (_vaoId != 0)
            {
                GL.DeleteVertexArray(_vaoId);
                GL.DeleteBuffers(_vboIds.Length, _vboIds);
                _vaoId = 0;
            }

            if (_programId != 0)
            {
                GL.DeleteProgram(_programId);
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

            GL.UseProgram(_programId);

            Span<float> matrix = stackalloc float[16];
            _camera.BuildProjectionMatrix(matrix, 0.0f);
            GL.UniformMatrix4(_projectionUniform, 1, false, ref matrix.GetPinnableReference());

            GL.BindVertexArray(_vaoId);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboIds[0]);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, _count * Vector2.SizeInBytes, _vertices);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboIds[1]);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, _count * Vector4h.SizeInBytes, _colors);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboIds[2]);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, _count * Unsafe.SizeOf<Half>(), _sizes);

            GL.Enable(EnableCap.ProgramPointSize);
            GL.DrawArrays(PrimitiveType.Points, 0, _count);
            GL.Disable(EnableCap.ProgramPointSize);

            CheckError();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            _count = 0;
        }
    }
}
