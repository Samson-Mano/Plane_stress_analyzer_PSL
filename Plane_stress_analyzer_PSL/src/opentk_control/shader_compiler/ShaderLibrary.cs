using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plane_stress_analyzer_PSL.src.opentk_control.shader_compiler
{
    public class ShaderLibrary
    {

        public enum ShaderType
        {
            MeshShader,
            TextShader,
            ConstraintShader,
            LoadShader,
            RsltMeshShader,
            RsltWireframeShader,
            SelectionShader,
            DrawingAxisShader,
        }


        #region "Mesh Shaders"

        private static string mesh_vert_shader()
        {
            return @"

            #version 330 core

            // Pre-computed MVP matrix on CPU for better performance
            uniform mat4 uMVP;           // Model-View-Projection matrix
            uniform vec4 vertexColor;
                    
            layout(location = 0) in vec2 aPosition;
                    

            out vec4 vColor;
                    
            void main()
            {
                gl_Position = uMVP * vec4(aPosition, 0.0, 1.0);
                vColor = vertexColor;
            }


                    ";

        }




        private static string mesh_frag_shader()
        {

            return @"

            #version 330 core

            in vec4 vColor;
            out vec4 fColor;
    
            void main()
            {
                // Simple color output without lighting
                fColor = vColor;
            }


                    ";

        }

        #endregion




        #region "Result Mesh Shaders"

        private static string rslt_mesh_vert_shader()
        {
            return @"

            #version 330 core

            // Pre-computed MVP matrix on CPU for better performance
            uniform mat4 uMVP;           // Model-View-Projection matrix
            uniform float geomscale = 1.0f; // Geometry scale factor
            uniform float sinevalue = 1.0f;                    
            uniform float modelpercent = 0.01; // default 1 % scale factor             
            

            layout(location = 0) in vec2 aPosition;
            layout(location = 1) in vec2 aDisplacement;
            layout(location = 2) in float aScalarValue;
                    
            out float v_deflscale;
                    
            void main()
            {
                float scalevalue = geomscale * modelpercent * aScalarValue;
                vec2 scaledDisplacement = aDisplacement * scalevalue * sinevalue;

                gl_Position = uMVP * vec4(aPosition + scaledDisplacement, 0.0, 1.0);
                v_deflscale = aScalarValue * sinevalue;
            }


                    ";

        }




        private static string rslt_mesh_frag_shader()
        {

            return @"

            #version 330 core
            
            uniform float vertexTransparency; // Transparency of the mesh
            in float v_deflscale;

            out vec4 fColor;
    

            vec3 jetHeatmap(float value) 
            {
                float t = (value + 1.0) * 0.5;
                return clamp(vec3(1.5) - abs(4.0 * vec3(t) + vec3(-3, -2, -1)), vec3(0), vec3(1));
            }


            void main()
            {
                // Simple color output without lighting
                fColor = vec4(jetHeatmap(v_deflscale), vertexTransparency);
            }


                    ";

        }

        #endregion




        #region "Result WireFrame Mesh Shaders"

        private static string rslt_wireframe_vert_shader()
        {
            return @"
            #version 330 core

            uniform mat4 uMVP;
            uniform float geomscale = 1.0f;
            uniform float sinevalue = 1.0f;                    
            uniform float modelpercent = 0.01;
    
            layout(location = 0) in vec2 aPosition;
            layout(location = 1) in vec2 aDisplacement;
            layout(location = 2) in float aScalarValue;
    
            out float v_deflscale;
        
            void main()
            {
                float scalevalue = geomscale * modelpercent * aScalarValue;
                vec2 scaledDisplacement = aDisplacement * scalevalue * sinevalue;
                gl_Position = uMVP * vec4(aPosition + scaledDisplacement, 0.0, 1.0);
                v_deflscale = aScalarValue * sinevalue;
            }
            ";
        }


        private static string rslt_wireframe_frag_shader()
        {
            return @"
            #version 330 core
    
            uniform float wireframeAlpha;
    
            in float v_deflscale;
    
            out vec4 fColor;
    
            vec3 jetHeatmap(float value) 
            {
                float t = (value + 1.0) * 0.5;
                return clamp(vec3(1.5) - abs(4.0 * vec3(t) + vec3(-3, -2, -1)), vec3(0), vec3(1));
            }
    
            void main()
            {
                vec3 contourColor = jetHeatmap(v_deflscale);
                vec3 finalColor;
        
         
                // Use complementary color with some brightness enhancement
                finalColor = vec3(1.0) - contourColor;

                // Make it brighter for visibility
                finalColor = mix(finalColor, vec3(1.0), 0.3);
              
        
                fColor = vec4(finalColor, wireframeAlpha);
            }
            ";
        }

        #endregion


        #region "Text shaders"

        public static string text_vert_shader()
        {
            return @"

            #version 330 core

            uniform mat4 uMVP;           // Model-View-Projection matrix
            uniform float zoomscale = 1.0f;

            uniform float vertexTransparency = 1.0f; // Transparency of the mesh

            layout(location = 0) in vec2 position;
            layout(location = 1) in vec2 origin;
            layout(location = 2) in vec2 textureCoord;
            layout(location = 3) in vec3 textColor;

            out vec4 v_textureColor;
            out vec2 v_textureCoord;

            void main()
            {

	            // apply Translation to the final position 
	            vec4 finalPosition =  uMVP * vec4(position,0.0f,1.0f);

	            // apply Translation to the text origin
	            vec4 finalTextorigin =  uMVP * vec4(origin,0.0f,1.0f);
    
	            // Remove the zoom scale
	            vec2 scaled_pt = vec2(finalPosition.x - finalTextorigin.x,finalPosition.y - finalTextorigin.y) / zoomscale;
		
	            // Set the final position of the vertex
	            gl_Position = vec4(scaled_pt.x + finalTextorigin.x, scaled_pt.y + finalTextorigin.y, 0.0f, 1.0f);

	            // Calculate texture coordinates for the glyph
	            v_textureCoord = textureCoord;
	
	            // Pass the texture color to the fragment shader
	            v_textureColor = vec4(textColor, vertexTransparency);
            }

                    ";

        }


        public static string text_frag_shader()
        {
            return @"

            #version 330 core
            uniform sampler2D u_Texture;

            in vec4 v_textureColor;
            in vec2 v_textureCoord;

            out vec4 f_Color; // fragment's final color (out to the fragment shader)

            void main()
            {
	            vec4 texColor = vec4(1.0, 1.0, 1.0, texture(u_Texture, v_textureCoord).r);
	            f_Color = v_textureColor * texColor;
            }

                    ";

        }

        #endregion


        #region "Constraint Shader"

        public static string constraint_vert_shader()
        {
            return @"

            #version 330 core

            uniform mat4 uMVP;           // Model-View-Projection matrix
            uniform float zoomscale = 1.0f;

            uniform vec4 vertexColor;

            layout(location = 0) in vec2 position;
            layout(location = 1) in vec2 origin;
            layout(location = 2) in vec2 textureCoord;
            layout(location = 3) in float textureType;


            flat out uint v_textureType;
            out vec2 v_textureCoord;
            out vec4 v_textureColor;

            void main()
            {

	            // apply Translation to the final position 
	            vec4 finalPosition = uMVP * vec4(position, 0.0f, 1.0f);

	            // apply Translation to the text origin
	            vec4 finalTextorigin = uMVP * vec4(origin, 0.0f, 1.0f);
    

	            // Remove the zoom scale
	            vec2 scaled_pt = vec2(finalPosition.x - finalTextorigin.x, finalPosition.y - finalTextorigin.y) / zoomscale;
		
	            // Set the final position of the vertex
	            gl_Position = vec4(scaled_pt.x + finalTextorigin.x, scaled_pt.y + finalTextorigin.y, 0.0f, 1.0f);


	            // update the texture type
	            v_textureType = uint(textureType);
	            v_textureCoord = textureCoord;
	            v_textureColor = vertexColor;

            }

                    ";

        }


        public static string constraint_frag_shader()
        {
            return @"

            #version 330 core
            // uniform float transparency;
            uniform sampler2D u_TexturePin;    // Pin support texture
            uniform sampler2D u_TextureRoller; // Roller support texture

            flat in uint v_textureType;  // 0 = Pin, 1 = Roller
            in vec2 v_textureCoord;
            in vec4 v_textureColor;

            out vec4 f_Color; // fragment's final color (out to the fragment shader)

            void main()
            {
                vec4 texColor;
        
                // Select which texture to sample based on v_textureType
                if (v_textureType == 0u)
                    texColor = texture(u_TexturePin, v_textureCoord);
                else
                    texColor = texture(u_TextureRoller, v_textureCoord);
        
                f_Color = v_textureColor * texColor;
            }

                    ";

        }



        #endregion


        #region "Load Shader"

        public static string load_vert_shader()
        {
            return @"

            #version 330 core

            uniform mat4 uMVP;
            uniform vec4 vertexColor;
            uniform float zoomscale = 1.0f;
    
            layout(location = 0) in vec2 aPosition;
            layout(location = 1) in vec2 aOrigin;
    
            out vec4 vColor;
    
            void main()
            {
                // Transform to clip space
                vec4 clipPos = uMVP * vec4(aPosition, 0.0, 1.0);
                vec4 clipOrigin = uMVP * vec4(aOrigin, 0.0, 1.0);
        
                // Calculate NDC coordinates
                vec3 ndcPos = clipPos.xyz / clipPos.w;
                vec3 ndcOrigin = clipOrigin.xyz / clipOrigin.w;
        
                // Scale offset in NDC space
                vec2 scaledOffset = (ndcPos.xy - ndcOrigin.xy) / zoomscale;
        
                // Final position (back to clip space)
                gl_Position = vec4(ndcOrigin.xy + scaledOffset, 0.0, 1.0);
        
                vColor = vertexColor;
            }

                    ";

        }


        public static string load_frag_shader()
        {
            return @"

            #version 330 core

            in vec4 vColor;
            out vec4 fColor;
    
            void main()
            {
                // Simple color output without lighting
                fColor = vColor;
            }

                    ";

        }



        #endregion


        #region "Selection Shader"

        private static string selrect_vert_shader()
        {
            return @"

            #version 330 core

            layout(location = 0) in vec2 node_position;

            out vec4 v_Color;

            void main()
            {
	            v_Color = vec4(0.8039f,0.3608f,0.3608f,0.5f);

	            // Final position passed to fragment shader
	            gl_Position = vec4(node_position,0.0f,1.0f);
            }

                    ";

        }



        private static string selrect_frag_shader()
        {
            return @"

            #version 330 core

            in vec4 v_Color;

            out vec4 f_Color; // fragment's final color (out to the fragment shader)

            void main()
            {
	            f_Color = v_Color;
            }

                    ";

        }


        #endregion




        #region "Drawing Axis Shader"

        private static string drawingaxis_vert_shader()
        {
            return @"

            #version 330 core

            layout(location = 0) in vec2 node_position;
            layout(location = 1) in vec3 node_color;

            out vec4 v_Color;

            void main()
            {
	            v_Color = vec4(node_color, 1.0f);

	            // Final position passed to fragment shader
	            gl_Position = vec4(node_position,0.0f,1.0f);
            }

                    ";

        }



        private static string drawingaxis_frag_shader()
        {
            return @"

            #version 330 core

            in vec4 v_Color;

            out vec4 f_Color; // fragment's final color (out to the fragment shader)

            void main()
            {
	            f_Color = v_Color;
            }

                    ";

        }


        #endregion



        public static string get_vertex_shader(ShaderType type)
        {
            // Returns the vertex shader
            switch (type)
            {
                case ShaderType.MeshShader:
                    return mesh_vert_shader();
                case ShaderType.RsltMeshShader:
                    return rslt_mesh_vert_shader();
                case ShaderType.RsltWireframeShader:
                    return rslt_wireframe_vert_shader();
                case ShaderType.SelectionShader:
                    return selrect_vert_shader();
                case ShaderType.ConstraintShader:
                    return constraint_vert_shader();
                case ShaderType.LoadShader:
                    return load_vert_shader();
                case ShaderType.TextShader:
                    return text_vert_shader();
                case ShaderType.DrawingAxisShader:
                    return drawingaxis_vert_shader();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), "Unknown shader type");

            }
        }

        public static string get_fragment_shader(ShaderType type)
        {
            // Returns the fragment shader
            switch (type)
            {
                case ShaderType.MeshShader:
                    return mesh_frag_shader();
                case ShaderType.RsltMeshShader:
                    return rslt_mesh_frag_shader();
                case ShaderType.RsltWireframeShader:
                    return rslt_wireframe_frag_shader();
                case ShaderType.SelectionShader:
                    return selrect_frag_shader();
                case ShaderType.ConstraintShader:
                    return constraint_frag_shader();
                case ShaderType.LoadShader:
                    return load_frag_shader();
                case ShaderType.TextShader:
                    return text_frag_shader();
                case ShaderType.DrawingAxisShader:
                    return drawingaxis_frag_shader();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), "Unknown shader type");

            }
        }

        //___________________

    }
}
