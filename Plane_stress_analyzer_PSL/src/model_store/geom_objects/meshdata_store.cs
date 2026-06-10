using Plane_stress_analyzer_PSL.src.model_store.fe_objects;
using Plane_stress_analyzer_PSL.src.opentk_control.opentk_buffer;
using Plane_stress_analyzer_PSL.src.opentk_control.shader_compiler;
using SharpFont.Cache;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.LinkLabel;

namespace Plane_stress_analyzer_PSL.src.model_store.geom_objects
{

    public class meshdata_store
    {

        private struct point_store
        {
           public int point_id;
            public float x_coord;
            public float y_coord;
        }

        private struct line_store
        {
            public int line_id;
            public int line_start_id;
            public int line_end_id;    
        }

        private struct tri_store
        {
            public int tri_id;
            public int pt_id1;
            public int pt_id2;
            public int pt_id3;
        }

        private struct quad_store
        {
            public int quad_id;
            public int pt_id1;
            public int pt_id2;
            public int pt_id3;
            public int pt_id4;
        }

        private struct mat_mesh_store
        {
            public int material_id;

            // Index buffer for  triangles and quadrilaterals (EBO)
            public IndexBuffer triangle_ibo;
            public IndexBuffer quadrilateral_ibo;

            public List<tri_store> tris;
            public List<quad_store> quads;
        }

        private const int FLOATS_PER_VERTEX = 2;

        List <point_store> points;
        List<line_store> wireframe_lines;
        Dictionary<int, mat_mesh_store> mat_mesh_data;

        // Geometry data for OpenGL
        Dictionary<int, int> pointIDToIndex;

        Shader meshShader;

        // Vertex Buffer object and Vertex Array object 
        VertexBuffer point_vbo;
        VertexArray point_vao;

        // Index buffer for the points, lines and triangles, quadrilaterals (EBO)
        IndexBuffer point_ibo;
        IndexBuffer selected_point_ibo;
        IndexBuffer wireframe_ibo;


        public meshdata_store()
        {
            InitializeShader();
        }


        public void add_point(int point_id, float x, float y)
        {
            points.Add(new point_store() 
            { 
                point_id = point_id, 
                x_coord = x, 
                y_coord = y 
            });
        }

        public void add_tri(int tri_id, int pt_id1, int pt_id2, int pt_id3, int mat_id)
        {
            if(mat_mesh_data.ContainsKey(mat_id) ==  false)
            {
                mat_mesh_store temp_mesh = new mat_mesh_store();
                temp_mesh.material_id = mat_id;
                temp_mesh.tris.Add(new tri_store() 
                { 
                    tri_id = tri_id, 
                    pt_id1 = pt_id1, 
                    pt_id2 = pt_id2, 
                    pt_id3 = pt_id3 
                });

                mat_mesh_data.Add(mat_id, temp_mesh);
            }
            else
            {
                mat_mesh_data[mat_id].tris.Add(new tri_store()
                {
                    tri_id = tri_id,
                    pt_id1 = pt_id1,
                    pt_id2 = pt_id2,
                    pt_id3 = pt_id3
                });
            }

        }

        public void add_quad(int quad_id, int pt_id1, int pt_id2, int pt_id3, int pt_id4, int mat_id)
        {
            if (mat_mesh_data.ContainsKey(mat_id) == false)
            {
                mat_mesh_store temp_mesh = new mat_mesh_store();
                temp_mesh.material_id = mat_id;
                temp_mesh.quads.Add(new quad_store()
                {
                    quad_id = quad_id,
                    pt_id1 = pt_id1,
                    pt_id2 = pt_id2,
                    pt_id3 = pt_id3,
                    pt_id4 = pt_id4
                });

                mat_mesh_data.Add(mat_id, temp_mesh);
            }
            else
            {
                mat_mesh_data[mat_id].quads.Add(new quad_store()
                {
                    quad_id = quad_id,
                    pt_id1 = pt_id1,
                    pt_id2 = pt_id2,
                    pt_id3 = pt_id3,
                    pt_id4 = pt_id4
                });
            }
        }


        private void InitializeShader()
        {
            // Create Shader
            meshShader = new Shader(ShaderLibrary.get_vertex_shader(ShaderLibrary.ShaderType.MeshShader),
                ShaderLibrary.get_fragment_shader(ShaderLibrary.ShaderType.MeshShader));

        }

   
        public void paint_mesh()
        {

        }


        public void paint_mesh_wireframe()
        {


        }


        public void paint_mesh_points()
        {


        }


        public void paint_selected_mesh_points()
        {


        }


        public void add_selected_points(List<int> selected_point_ids)
        {


        }


        public void create_wireframe()
        {

        }


        public void create_buffer_data()
        {
            // Create the OpenGL buffer for the mesh
            pointIDToIndex.Clear();


            List<int> pointIndexData = new List<int>();

            for (int i = 0; i < points.Count; i++)
            {
                pointIDToIndex[points[i].point_id] = i;

                pointIndexData.Add(i);
            }

            //_______________________________________________________________
            // prepare the Vertex data for openGL
            List<float> vertexData = new List<float>();

            foreach(point_store pt in points)
            {
                vertexData.Add(pt.x_coord);
                vertexData.Add(pt.y_coord);
            }


            point_vao = new VertexArray();
            point_vbo = new VertexBuffer(10);
            point_ibo = new IndexBuffer(10);

            var pointLayout = new VertexBufferLayout();
            pointLayout.AddFloat(FLOATS_PER_VERTEX);
            point_vao.Add_vertexBuffer(point_vbo, pointLayout);


            point_vbo.AppendVertexBuffer(vertexData.ToArray());
            point_ibo.AppendIndexBuffer(pointIndexData.ToArray());

            //_______________________________________________________________
            // prepare wireframe index data for openGL
            List<int> wireframeIndexData = new List<int>();

            foreach (line_store ln in wireframe_lines)
            {
                int start_idx = pointIDToIndex[ln.line_start_id];
                int end_idx = pointIDToIndex[ln.line_end_id];

                wireframeIndexData.Add(start_idx);
                wireframeIndexData.Add(end_idx);

            }


            wireframe_ibo = new IndexBuffer(10);
            wireframe_ibo.AppendIndexBuffer(wireframeIndexData.ToArray());

            //_______________________________________________________________
            // prepare triangle index data for openGL

            for (int i = 0; i < mat_mesh_data.Count; i++)
            {


            }


            foreach(int mat_mesh_key in mat_mesh_data.Keys)
            {
                mat_mesh_data[mat_mesh_key].triangle_ibo = new IndexBuffer(10);
                mat_mesh_data[mat_mesh_key].quadrilateral_ibo = new IndexBuffer(10);




            }



            this->triangleIndexData.clear();

            for (const elementtri_store&tri : this->tris)
	{
                auto it_nd1 = this->pointIdToIndex.find(tri.nd1_id);
                auto it_nd2 = this->pointIdToIndex.find(tri.nd2_id);
                auto it_nd3 = this->pointIdToIndex.find(tri.nd3_id);

                if (it_nd1 != this->pointIdToIndex.end() && it_nd2 != this->pointIdToIndex.end() &&
                    it_nd3 != this->pointIdToIndex.end())
                {
                    this->triangleIndexData.push_back(it_nd1->second);
                    this->triangleIndexData.push_back(it_nd2->second);
                    this->triangleIndexData.push_back(it_nd3->second);
                }
            }

            //_______________________________________________________________
            // prepare quadrilateral index data for openGL
            this->quadrilateralIndexData.clear();

            for (const elementquad_store&quad : this->quads)
	{
                auto it_nd1 = this->pointIdToIndex.find(quad.nd1_id);
                auto it_nd2 = this->pointIdToIndex.find(quad.nd2_id);
                auto it_nd3 = this->pointIdToIndex.find(quad.nd3_id);
                auto it_nd4 = this->pointIdToIndex.find(quad.nd4_id);


                if (it_nd1 != this->pointIdToIndex.end() && it_nd2 != this->pointIdToIndex.end() &&
                    it_nd3 != this->pointIdToIndex.end() && it_nd4 != this->pointIdToIndex.end())
                {
                    // Make two triangles from the quad (pt1, pt2, pt3) and (pt1, pt3, pt4)
                    // Triangle 1 (nd1, nd2, nd3)
                    this->quadrilateralIndexData.push_back(it_nd1->second);
                    this->quadrilateralIndexData.push_back(it_nd2->second);
                    this->quadrilateralIndexData.push_back(it_nd3->second);

                    // Triangle 2 (nd1, nd3, nd4)
                    this->quadrilateralIndexData.push_back(it_nd1->second);
                    this->quadrilateralIndexData.push_back(it_nd3->second);
                    this->quadrilateralIndexData.push_back(it_nd4->second);
                }
            }

            //_______________________________________________________________
            // Prepare vertex normal data for OpenGL
            create_vertex_normals(vnormals);
            //



        }


        public void update_openTK_uniforms()
        {


        }


    }
}
