using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using Plane_stress_analyzer_PSL.src.model_store.geom_objects;
using Plane_stress_analyzer_PSL.src.opentk_control.opentk_buffer;
using Plane_stress_analyzer_PSL.src.opentk_control.shader_compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace src.model_store.geom_objects
{
    public class contourlevelbar_store : IDisposable
    {
        // XY labels
        private label_list_store contourResultLabel;

        // Contour level label data
        private label_list_store contourLevelLabel;


        // Rectangle
        private Vector2 _RightBottomCornerPoint = Vector2.Zero;
        private Vector2 _RightTopCornerPoint = Vector2.Zero;

        const float CONTOUR_LEVELBAR_WIDTH = 0.04f; // Width of the contour level bar in pixels
        const float CONTOUR_LEVELTRI_WIDTH = 0.038f; // Width of the contour level triangle in pixels
        const int CONTOUR_LEVELS = 10;

        // Rendering resources
        private VertexArray _contourbarVAO;
        private VertexBuffer _contourbarVBO;
        private IndexBuffer _contourbarIBO;


        private VertexArray _contourtriVAO;
        private VertexBuffer _contourtriVBO;
        private IndexBuffer _contourtriIBO;


        private Shader _contourbarshader;
        private bool _disposed = false;

        private bool IsInitialized = false;



        public contourlevelbar_store()
        {
            // Empty constructor
        }



        public void InitializeContourLevelBarData(int window_width, int window_height)
        {
            // Initialize contour level bar data here
            InitializeShader();
            InitializeBuffers();
            SetupIndexBuffers();

            IsInitialized = true;

            contourResultLabel = new label_list_store();
            contourLevelLabel = new label_list_store();

            UpdateContourLevelBarPosition(window_width, window_height, 0.0f, 1.0f);
        }


        private void InitializeShader()
        {
            // Create Shader
            _contourbarshader = new Shader(ShaderLibrary.get_vertex_shader(ShaderLibrary.ShaderType.ContourBarShader),
                ShaderLibrary.get_fragment_shader(ShaderLibrary.ShaderType.ContourBarShader));


        }


        private void InitializeBuffers()
        {
            // Contour level bar
            _contourbarVAO = new VertexArray();
            _contourbarVBO = new VertexBuffer(CONTOUR_LEVELS * 2 * 3); // 2 points per level, 3 floats per point (x, y, colorValue)
            _contourbarIBO = new IndexBuffer(CONTOUR_LEVELS * 2); // 8 indices to form a rectangle (4 lines)


            _contourtriVAO = new VertexArray();
            _contourtriVBO = new VertexBuffer(CONTOUR_LEVELS * 2 * 3); // 2 points per level, 3 floats per point (x, y, colorValue)
            _contourtriIBO = new IndexBuffer(CONTOUR_LEVELS * 6); // 6 indices to form a rectangle (2 triangles)

            var contourbarLayout = new VertexBufferLayout();
            contourbarLayout.AddFloat(2);
            contourbarLayout.AddFloat(1);

            _contourbarVAO.Add_vertexBuffer(_contourbarVBO, contourbarLayout);
            _contourtriVAO.Add_vertexBuffer(_contourtriVBO, contourbarLayout);

        }


        private void SetupIndexBuffers()
        {
            // Setup contour level bar indices
            int[] lineIndices =  new int[CONTOUR_LEVELS * 2];

            lineIndices[0] = 0;
            lineIndices[1] = 1;

            for (int i = 1; i < CONTOUR_LEVELS; i++)
            {
                lineIndices[i * 2] = (i * 2);
                lineIndices[(i * 2) + 1] = (i * 2) + 1;

            }
            _contourbarIBO.AppendIndexBuffer(lineIndices);


            int[] triIndices = new int[CONTOUR_LEVELS * 6];
            
            for (int i = 0; i < CONTOUR_LEVELS - 1; i++)
            {
                int baseIndex = i * 6;
                int vertexIndex = i * 2;
                triIndices[baseIndex + 0] = vertexIndex + 0;
                triIndices[baseIndex + 1] = vertexIndex + 1;
                triIndices[baseIndex + 2] = vertexIndex + 2;
                triIndices[baseIndex + 3] = vertexIndex + 1;
                triIndices[baseIndex + 4] = vertexIndex + 3;
                triIndices[baseIndex + 5] = vertexIndex + 2;
            }

            _contourtriIBO.AppendIndexBuffer(triIndices);
        }


        public void UpdateContourLevelBarPosition(int window_width, int window_height, float contour_min, float contour_max)
        {
            if (!IsInitialized)
                return;


            // 1. Determine max dimension
            int max_drawing_area_size = Math.Max(window_width, window_height);

            int drawing_area_center_x = (int)((window_width - max_drawing_area_size) * 0.5f);
            int drawing_area_center_y = (int)((window_height - max_drawing_area_size) * 0.5f);

            // 2. Normalize screen dimensions
            double normalizedScreenWidth = 2.0d * ((double)window_width / (double)max_drawing_area_size);
            double normalizedScreenHeight = 2.0d * ((double)window_height / (double)max_drawing_area_size);

            float rightBotX = (float)normalizedScreenWidth * 0.5f;
            float rightBotY = -(float)normalizedScreenHeight * 0.5f;

            float rightTopX = (float)normalizedScreenWidth * 0.5f;
            float rightTopY = (float)normalizedScreenHeight * 0.5f;


            // Assign to private rectangle points
            this._RightBottomCornerPoint = new Vector2(rightBotX, rightBotY);
            this._RightTopCornerPoint = new Vector2(rightTopX, rightTopY);

            UpdateVertexBuffers(contour_min, contour_max);

            // Update the label shader uniforms for the new window size
            Matrix4 uMVP = Matrix4.Identity;

            contourResultLabel.update_openTK_uniforms(uMVP, 1.0f, 1.0f);
            contourLevelLabel.update_openTK_uniforms(uMVP, 1.0f, 1.0f);

        }



        private void UpdateVertexBuffers(float contour_min, float contour_max)
        {
            // Define the 4 corners of the rectangle
            float[] vertices = new float[CONTOUR_LEVELS * 2 * 3];

            float drawing_area_height = _RightTopCornerPoint.Y - _RightBottomCornerPoint.Y;
            float contour_bar_height = (float)(drawing_area_height * 0.8f); // 80% of the drawing area height

            float OriginX = _RightBottomCornerPoint.X - 0.1f;
            float OriginY = _RightBottomCornerPoint.Y + (float)(drawing_area_height * 0.1f);


            // Bottop point of the contour bar
            vertices[0] = OriginX;
            vertices[1] = OriginY;
            vertices[2] = 0.0f; 

            vertices[3] = OriginX - CONTOUR_LEVELBAR_WIDTH;
            vertices[4] = OriginY;
            vertices[5] = 0.0f; 

            for (int i = 1; i< CONTOUR_LEVELS; i++)
            {
                float levelY = ((contour_bar_height * i) / (CONTOUR_LEVELS - 1));
                float colorValue = (float)i / (CONTOUR_LEVELS - 1);

                vertices[(i*6) + 0] = OriginX;
                vertices[(i*6) + 1] = OriginY + levelY;
                vertices[(i*6) + 2] = colorValue;

                vertices[(i*6) + 3] = OriginX - CONTOUR_LEVELBAR_WIDTH;
                vertices[(i*6) + 4] = OriginY + levelY;
                vertices[(i*6) + 5] = colorValue;

            }

            // Update both VBOs with the same vertices
            _contourbarVBO.updateVertexBuffer(vertices);


            // Bottop point of the contour bar
            vertices[0] = OriginX;
            vertices[1] = OriginY;
            vertices[2] = 0.0f;

            vertices[3] = OriginX - CONTOUR_LEVELTRI_WIDTH;
            vertices[4] = OriginY;
            vertices[5] = 0.0f;

            for (int i = 1; i < CONTOUR_LEVELS; i++)
            {
                float levelY = ((contour_bar_height * i) / (CONTOUR_LEVELS - 1));
                float colorValue = (float)i / (CONTOUR_LEVELS - 1);

                vertices[(i * 6) + 0] = OriginX;
                vertices[(i * 6) + 1] = OriginY + levelY;
                vertices[(i * 6) + 2] = colorValue;

                vertices[(i * 6) + 3] = OriginX - CONTOUR_LEVELTRI_WIDTH;
                vertices[(i * 6) + 4] = OriginY + levelY;
                vertices[(i * 6) + 5] = colorValue;

            }

            _contourtriVBO.updateVertexBuffer(vertices);


            // Add the labels for the contour levels
            contourLevelLabel.clear_labels();

            for (int i = 0; i < CONTOUR_LEVELS; i++)
            {
                float levelX = OriginX - CONTOUR_LEVELBAR_WIDTH; // - 0.07f;
                float levelY = OriginY + ((contour_bar_height * i) / (CONTOUR_LEVELS - 1)) + 0.015f;

                float colorValue = (float)i / (float)(CONTOUR_LEVELS - 1);

                // Get vector3 color from colorValue using a colormap (e.g., Jet colormap)
                Vector3 color = new Vector3(colorValue, 0.0f, 1.0f - colorValue); // Example: simple gradient from blue to red

                float contour_value = contour_min + (contour_max - contour_min) * colorValue; // Example: normalized contour value
                float string_width = contour_value.ToString("F6").Length * 0.01f; // Estimate string width based on character count

                contourLevelLabel.add_label(i, contour_value.ToString("F6"), 
                    new Vector2(levelX - string_width, levelY), color);

            }

            contourLevelLabel.update_buffer(1.1f);


        }


        public void draw_contour_bar()
        {

            _contourbarshader.Bind();

            //______________________________________________________________
            _contourbarVAO.Bind();
            _contourbarIBO.Bind();

            GL.LineWidth(3.0f);
            GL.DrawElements(PrimitiveType.Lines, CONTOUR_LEVELS * 2,
                           DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.LineWidth(1.0f);

            // Cleanup 1
            _contourbarIBO.UnBind();
            _contourbarVAO.UnBind();


            //______________________________________________________________
            _contourtriVAO.Bind();
            _contourtriIBO.Bind();

            GL.DrawElements(PrimitiveType.Triangles, CONTOUR_LEVELS * 6,
                           DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Cleanup 2
            _contourtriIBO.UnBind();
            _contourtriVAO.UnBind();

            
        
             _contourbarshader.UnBind();

            // Paint the contour level labels
            contourLevelLabel.paint_static_labels();

            
        }



        public void Dispose()
        {
            if (!_disposed)
            {
                _contourbarVAO?.Dispose();
                _contourbarVBO?.Dispose();
                _contourbarIBO?.Dispose();
                _contourtriVAO?.Dispose();
                _contourtriVBO?.Dispose();
                _contourtriIBO?.Dispose();
                // _shader?.Dispose();
                _disposed = true;
            }
        }




    }
}
