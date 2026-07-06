using Plane_stress_analyzer_PSL.src.opentk_control.opentk_buffer;
using Plane_stress_analyzer_PSL.src.opentk_control.shader_compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;


namespace src.model_store.geom_objects
{
    public class axisdata_store : IDisposable
    {

        // Constants
        private const int VERTEX_COUNT = 4;
        private const int LINE_INDEX_COUNT = 8;  // 4 lines * 2 indices
        private const int FLOATS_PER_VERTEX = 2;

        // Rectangle state
        private Vector2 _leftCornerPoint = Vector2.Zero;
        private Vector2 _rightCornerPoint = Vector2.Zero;


        // Rendering resources
        private VertexArray _boundaryVAO;
        private VertexBuffer _boundaryVBO;
        private IndexBuffer _boundaryIBO;

        private Shader _shader;
        private bool _disposed = false;

        public axisdata_store()
        {
            // Initialize axis data here
            InitializeShader();
            InitializeBuffers();
            SetupIndexBuffers();
        }

        private void InitializeShader()
        {
            // Create Shader
            _shader = new Shader(ShaderLibrary.get_vertex_shader(ShaderLibrary.ShaderType.SelectionShader),
                ShaderLibrary.get_fragment_shader(ShaderLibrary.ShaderType.SelectionShader));


        }


        private void InitializeBuffers()
        {
            // Boundary (wireframe)
            _boundaryVAO = new VertexArray();
            _boundaryVBO = new VertexBuffer(VERTEX_COUNT * FLOATS_PER_VERTEX);
            _boundaryIBO = new IndexBuffer(LINE_INDEX_COUNT);

            var boundaryLayout = new VertexBufferLayout();
            boundaryLayout.AddFloat(FLOATS_PER_VERTEX);

            _boundaryVAO.Add_vertexBuffer(_boundaryVBO, boundaryLayout);
        }


        private void SetupIndexBuffers()
        {
            // Setup boundary indices (line loop)
            int[] lineIndices = {
            0, 1,  // Bottom edge
            1, 2,  // Right edge
            2, 3,  // Top edge
            3, 0   // Left edge
        };
            _boundaryIBO.AppendIndexBuffer(lineIndices);
        }


        public void UpdateAxisBoundaryRectangle(int window_width, int window_height)
        {
            // 1. Determine max dimension
            int max_drawing_area_size = Math.Max(window_width, window_height);

            int drawing_area_center_x = (int)((window_width - max_drawing_area_size) * 0.5f);
            int drawing_area_center_y = (int)((window_height - max_drawing_area_size) * 0.5f);


            // 2. Normalize screen dimensions
            double normalizedScreenWidth = 2.0d * ((double)window_width / (double)max_drawing_area_size);
            double normalizedScreenHeight = 2.0d * ((double)window_height / (double)max_drawing_area_size);

            float leftX = -(float)normalizedScreenWidth * 0.5f;
            float leftY = -(float)normalizedScreenHeight * 0.5f;
                    
            float rightX = leftX + 0.1f;
            float rightY = leftY + 0.1f;

            // Assign to private rectangle points
            this._leftCornerPoint = new Vector2(leftX, leftY);
            this._rightCornerPoint = new Vector2(rightX, rightY);

            UpdateVertexBuffers();

        }


        private void UpdateVertexBuffers()
        {
            // Define the 4 corners of the rectangle
            float[] vertices = new float[VERTEX_COUNT * FLOATS_PER_VERTEX];

            // Bottom-left
            vertices[0] = _leftCornerPoint.X;
            vertices[1] = _leftCornerPoint.Y;

            // Bottom-right
            vertices[2] = _rightCornerPoint.X;
            vertices[3] = _leftCornerPoint.Y;

            // Top-right
            vertices[4] = _rightCornerPoint.X;
            vertices[5] = _rightCornerPoint.Y;

            // Top-left
            vertices[6] = _leftCornerPoint.X;
            vertices[7] = _rightCornerPoint.Y;

            // Update both VBOs with the same vertices
            _boundaryVBO.updateVertexBuffer(vertices);

        }



        public void draw_axis_rectangle()
        {
            
            _shader.Bind();

            // Draw boundary (wireframe)
            _boundaryVAO.Bind();
            _boundaryIBO.Bind();
            GL.DrawElements(PrimitiveType.Lines, LINE_INDEX_COUNT,
                           DrawElementsType.UnsignedInt, IntPtr.Zero);

            
            // Cleanup
            _boundaryVAO.UnBind();
            _boundaryIBO.UnBind();
            _shader.UnBind();

        }



        public void Dispose()
        {
            if (!_disposed)
            {
                _boundaryVAO?.Dispose();
                _boundaryVBO?.Dispose();
                _boundaryIBO?.Dispose();
                // _shader?.Dispose();
                _disposed = true;
            }
        }



    }
}
