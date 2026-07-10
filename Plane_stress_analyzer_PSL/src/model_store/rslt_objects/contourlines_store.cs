using Plane_stress_analyzer_PSL.src.opentk_control.opentk_buffer;
using Plane_stress_analyzer_PSL.src.opentk_control.shader_compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.model_store.rslt_objects
{
    public class contourlines_store
    {
        private struct contourlinepoint_store
        {
            public int point_id;
            public float point_x;
            public float point_y;

            public float result_magnitude;
        }


        private struct contourline_store
        {
            public int line_id;
            public int line_start_id;
            public int line_end_id;
        }


        private struct ContourLevelData
        {
            public int NumLevels;
            public List<contourline_store> Lines;
            private VertexArray contourline_vao;
            public IndexBuffer contourline_ibo;
            public VertexBuffer contourline_vbo;

        }


         private Shader contourlinesShader;


        public contourlines_store() 
        {
            InitializeShader();

        }


        private void InitializeShader()
        {
            // Create Shader
            contourlinesShader = new Shader(
                ShaderLibrary.get_vertex_shader(ShaderLibrary.ShaderType.RsltMeshShader),
                ShaderLibrary.get_fragment_shader(ShaderLibrary.ShaderType.RsltMeshShader)
                );

        }






    }
}
