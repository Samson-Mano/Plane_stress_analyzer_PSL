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
            RsltPSLShader,
            SelectionShader,
            DrawingAxisShader,
            ContourBarShader
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
            uniform float rsltoption = 0.0; 

            layout(location = 0) in vec2 aPosition;
            layout(location = 1) in vec2 aDisplacement;
            layout(location = 2) in float aDisplacementMagnitude;
            layout(location = 3) in float aScalarValue;
                    
            out float v_deflscale;
                    
            void main()
            {
                float scalevalue = geomscale * modelpercent * aDisplacementMagnitude;
                vec2 scaledDisplacement = aDisplacement * scalevalue * sinevalue;

                gl_Position = uMVP * vec4(aPosition + scaledDisplacement, 0.0, 1.0);
                
                float contourcolor = aScalarValue * sinevalue;  

                if(rsltoption != 0)
                    contourcolor = (contourcolor + 1.0) * 0.5; // Normalize to [0,1] if option is set

                v_deflscale = contourcolor;

            }


                    ";

        }

        private static string rslt_mesh_frag_shader()
        {

            return @"

            #version 330 core

            uniform float vertexTransparency; // Transparency of the mesh
            uniform float uNumContours = 10.0;      // number of contour bands
            uniform float uLineWidth = 1.0;         // contour line thickness (in pixels, roughly)
            uniform vec3  uLineColor = vec3(0.0);   // contour line color (black by default)
            uniform float uLineOpacity = 1.0;       // how strongly lines blend over the heatmap
            uniform float uMinContourValue = 0.01;  // minimum value for contour lines (smooth falloff)

            in float v_deflscale;

            out vec4 fColor;

            vec3 jetHeatmap(float value) 
            {
                // values between 0.0 and 1.0 are mapped to the jet colormap
                float t = value;
                return clamp(vec3(1.5) - abs(4.0 * vec3(t) + vec3(-3, -2, -1)), vec3(0), vec3(1));
            }


            // Returns 1.0 exactly on a contour line, fading to 0.0 away from it
            float contourLines(float value, float numContours, float lineWidth)
            {
                float scaled = value * numContours;
                float distToLine = abs(fract(scaled + 0.5) - 0.5); // distance to nearest integer
                float aa = fwidth(scaled) * lineWidth;              // pixel-based line width
                return 1.0 - smoothstep(0.0, aa, distToLine);
            }


            void main()
            {
                vec3 baseColor = vec3(0.0); // jetHeatmap(v_deflscale);
                
                float line = 0.2f;

                // Contour bands
                if (v_deflscale < 0.0f)
                {
                    baseColor = vec3(0.4f, 0.4f, 0.4f); // Dark gray for negative values
                    line = 0.0f; // Disable contour lines for negative values
                }
                else if (v_deflscale > 1.0f)
                {
                    baseColor = vec3(0.8f, 0.8f, 0.8f); // Light gray for above 1.0 values
                    line = 0.0f; // Disable contour lines for values above 1.0
                }
                else
                {
                    baseColor = jetHeatmap(v_deflscale);
                }


                if (uLineOpacity > 0.1 && line > 0.1f)
                {
                    line = contourLines(v_deflscale, uNumContours, uLineWidth);
            
                    // Smoothly fade contour lines as value approaches zero
                    float valueScale = abs(v_deflscale);
                    float falloff = smoothstep(0.0, uMinContourValue, valueScale);
                    line *= falloff;
                }

                vec3 finalColor = mix(baseColor, uLineColor, line * uLineOpacity);

                fColor = vec4(finalColor, vertexTransparency);
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
            layout(location = 2) in float aDisplacementMagnitude;
            layout(location = 3) in float aScalarValue;
    
            out float v_deflscale;
        
            void main()
            {
                float scalevalue = geomscale * modelpercent * aDisplacementMagnitude;
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
                float t = value; // (value + 1.0) * 0.5;
                return clamp(vec3(1.5) - abs(4.0 * vec3(t) + vec3(-3, -2, -1)), vec3(0), vec3(1));
            }
    
            void main()
            {
                vec3 contourColor = jetHeatmap(v_deflscale);
                vec3 finalColor;
        
         
                // Use complementary color with some brightness enhancement
                finalColor = vec3(1.0) - contourColor;

                // Make it brighter for visibility
                finalColor = mix(finalColor, vec3(1.0), 0.1);
              
        
                fColor = vec4(finalColor, wireframeAlpha);
            }
            ";
        }

        #endregion


        #region "Result Plane stress line shader"


        private static string rslt_psline_mesh_vert_shader()
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
            layout(location = 2) in float aDisplacementMagnitude;
            layout(location = 3) in float aPrincipalAngle; // Principal stress angle in radians
            layout(location = 4) in float aSigma1; // Principal stress value 1
            layout(location = 5) in float aSigma2; // Principal stress value 2
            layout(location = 6) in vec2 aDirection1; // Direction of principal stress 1 x 
            layout(location = 7) in vec2 aDirection2; // Direction of principal stress 2 x 
            
            out vec2 v_worldPos;         // World position for distance calculations
            out float v_sigma1;          // Pass sigma1 to fragment
            out float v_sigma2;          // Pass sigma2 to fragment
            // out float v_principalAngle;  // Pass angle to fragment
            out vec2 v_direction1;       // Pass direction1 to fragment
            out vec2 v_direction2;       // Pass direction2 to fragment                    
            // out float v_deflscale;
                    
            void main()
            {
                float scalevalue = geomscale * modelpercent * aDisplacementMagnitude;
                vec2 scaledDisplacement = aDisplacement * scalevalue * sinevalue;

                gl_Position = uMVP * vec4(aPosition + scaledDisplacement, 0.0, 1.0);
                
                float contourcolor = aDisplacementMagnitude * sinevalue;  


                // if(rsltoption != 0)
                //     contourcolor = (contourcolor + 1.0) * 0.5; // Normalize to [0,1] if option is set
                
                // Pass the principal stress values and angle to the fragment shader
                v_worldPos = aPosition + scaledDisplacement;
                // v_deflscale = contourcolor;
                v_sigma1 = aSigma1;
                v_sigma2 = aSigma2;
                // v_principalAngle = aPrincipalAngle;
                v_direction1 = aDirection1;
                v_direction2 = aDirection2;

            }


                    ";

        }



        private static string rslt_psline_mesh_frag_shader()
        {

            return @"
            
            #version 330 core

            uniform float uLineWidth = 2.0;
            uniform float uContourDensity = 0.01;
            uniform float uMinStressMagnitude = 0.0;

            uniform bool uShowTension = true;
            uniform bool uShowCompression = true;
            uniform vec3 uTensionColor = vec3(1.0, 0.0, 0.0);
            uniform vec3 uCompressionColor = vec3(0.0, 0.0, 1.0);

            in vec2  v_worldPos;
            in float v_sigma1;
            in float v_sigma2;
            in vec2  v_direction1;
            in vec2  v_direction2;

            out vec4 fragColor;

            // Repurposed contour function for trajectory lines
            float trajectoryLine(float value, float density, float width)
            {
                float scaled = value * density;
                float distToLine = abs(fract(scaled + 0.5) - 0.5);
                float aa = fwidth(scaled) * width;
                return 1.0 - smoothstep(0.0, aa, distToLine);
            }

            void main()
            {
                vec3 color = vec3(0.0); // Black/transparent background
    
                // Get stress info
                float mag1 = abs(v_sigma1);
                float mag2 = abs(v_sigma2);
                vec2 dir1 = normalize(v_direction1);
                vec2 dir2 = normalize(v_direction2);
    
                // Draw major principal stress lines
                if (mag1 > uMinStressMagnitude && 
                    ((v_sigma1 > 0.0 && uShowTension) || (v_sigma1 < 0.0 && uShowCompression)))
                {
                    // Project position along direction field
                    float val = dot(v_worldPos, dir1);
                    float line = trajectoryLine(val, uContourDensity, uLineWidth);
        
                    if (line > 0.0)
                    {
                        vec3 lineColor = (v_sigma1 > 0.0) ? uTensionColor : uCompressionColor;
                        color = mix(color, lineColor, line);
                    }
                }
    
                // Draw minor principal stress lines (perpendicular)
                if (mag2 > uMinStressMagnitude && 
                    ((v_sigma2 > 0.0 && uShowTension) || (v_sigma2 < 0.0 && uShowCompression)))
                {
                    float val = dot(v_worldPos, dir2);
                    float line = trajectoryLine(val, uContourDensity, uLineWidth);
        
                    if (line > 0.0)
                    {
                        vec3 lineColor = (v_sigma2 > 0.0) ? uTensionColor : uCompressionColor;
                        // If both lines overlap, use max intensity
                        color = max(color, lineColor * line);
                    }
                }
    
                fragColor = vec4(color, 1.0);
            }



";
}

        private static string rslt_psline_mesh_frag_shader_r0()
        {

            return @"


            # version 330 core

            uniform float vertexTransparency;

            uniform float uNumContours = 10.0;
            uniform float uLineWidth = 1.0;
            uniform vec3  uLineColor = vec3(0.0);
            uniform float uLineOpacity = 1.0;
            uniform float uMinContourValue = 0.01;

            // Controls density of principal stress lines.
            uniform float uStressLineDensity = 25.0;

            in float v_sigma1;
            in float v_sigma2;
            in float v_principalAngle;
            in vec2  v_direction1;
            in vec2  v_direction2;
            in float v_deflscale;

            out vec4 fColor;


            // ------------------------------------------------------------
            // Heatmap
            // ------------------------------------------------------------

            vec3 jetHeatmap(float value)
            {
                float t = clamp(value, 0.0, 1.0);

                return clamp(
                    vec3(1.5) -
                    abs(4.0 * vec3(t) + vec3(-3.0, -2.0, -1.0)),
                    vec3(0.0),
                    vec3(1.0)
                );
            }


            // ------------------------------------------------------------
            // Ordinary scalar contour
            // ------------------------------------------------------------

            float scalarContour(float value, float numContours, float lineWidth)
            {
                float scaled = value * numContours;

                float distToLine =
                    abs(fract(scaled + 0.5) - 0.5);

                float aa = max(fwidth(scaled) * lineWidth, 1e-5);

                return 1.0 -
                       smoothstep(0.0, aa, distToLine);
            }


            // ------------------------------------------------------------
            // Directional stripe pattern
            //
            // The stripes are perpendicular to dir.
            //
            // NOTE:
            // This is an approximation. It does NOT integrate the
            // direction field, so strongly curved stress trajectories
            // will not be represented correctly.
            // ------------------------------------------------------------

            float principalStripe(vec2 direction, float density, float width)
            {
                direction = normalize(direction);

                // Pixel coordinates.
                vec2 p = gl_FragCoord.xy;

                // Coordinate perpendicular to the principal direction.
                vec2 normal = vec2(-direction.y, direction.x);

                float coordinate = dot(p, normal);

                float phase = coordinate * density / 100.0;

                float distToLine =
                    abs(fract(phase + 0.5) - 0.5);

                float aa = max(fwidth(phase), 1e-5);

                return 1.0 -
                       smoothstep(
                           width * aa,
                           (width + 1.0) * aa,
                           distToLine
                       );
            }


            // ------------------------------------------------------------
            // Stress information
            // ------------------------------------------------------------

            void getStressAtFragment(out float sigma1, out float sigma2, out vec2 dir1,
                out vec2 dir2, out float angle)
            {
                sigma1 = v_sigma1;
                sigma2 = v_sigma2;

                dir1 = normalize(v_direction1);
                dir2 = normalize(v_direction2);

                angle = v_principalAngle;
            }


            // ------------------------------------------------------------
            // Main
            // ------------------------------------------------------------

            void main()
            {

                float sigma1;
                float sigma2;
                vec2 dir1;
                vec2 dir2;
                float angle;

                getStressAtFragment(
                    sigma1,
                    sigma2,
                    dir1,
                    dir2,
                    angle
                );


                // --------------------------------------------------------
                // Heatmap value
                //
                // Replace this with whatever stress normalization you use.
                // --------------------------------------------------------

                float stressValue = clamp(abs(sigma1), 0.0, 1.0);

                vec3 heatColor = jetHeatmap(stressValue);


                // --------------------------------------------------------
                // Principal stress lines
                // --------------------------------------------------------

                float line1 = principalStripe(dir1, uStressLineDensity, uLineWidth);

                float line2 = principalStripe(dir2, uStressLineDensity, uLineWidth);

                // Choose either family:
                float principalLines = line1;

                // Or both families:
                // float principalLines = max(line1, line2);


                // --------------------------------------------------------
                // Suppress lines when stress magnitude is very small
                // --------------------------------------------------------

                float magnitude = max(abs(sigma1), abs(sigma2));

                float stressMask =
                    smoothstep(0.0, uMinContourValue, magnitude);

                principalLines *= stressMask;


                // --------------------------------------------------------
                // Blend lines over heatmap
                // --------------------------------------------------------

                vec3 finalColor = mix(
                    heatColor,
                    uLineColor,
                    principalLines * uLineOpacity
                );


                float alpha = vertexTransparency;

                fColor = vec4(finalColor, alpha);
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



        #region "Contour Bar Shader"

        private static string contourbar_vert_shader()
        {
            return @"

            #version 330 core

            layout(location = 0) in vec2 node_position;
            layout(location = 1) in float node_value; // Value for the contour level between 0.0 and 1.0

            out float v_node_value;

            void main()
            {
	            // Map the node_value to a color (e.g., from blue to red)
	            v_node_value = node_value;

	            // Final position passed to fragment shader
	            gl_Position = vec4(node_position,0.0f,1.0f);
            }

                    ";

        }



        private static string contourbar_frag_shader()
        {
            return @"

            #version 330 core

            in float v_node_value;

            out vec4 f_Color; // fragment's final color (out to the fragment shader)
            
             vec3 jetHeatmap(float value) 
            {
                float t = value; // (value + 1.0) * 0.5;
                return clamp(vec3(1.5) - abs(4.0 * vec3(t) + vec3(-3, -2, -1)), vec3(0), vec3(1));
            }


            void main()
            {
                vec3 contourColor = vec3(0.0); 
                
                if(v_node_value < 0.0f)
                    contourColor = vec3(0.4f, 0.4f, 0.4f); // Dark gray for negative values
                else if(v_node_value > 1.0f)
                    contourColor = vec3(0.8f, 0.8f, 0.8f); // Light gray for values greater than 1
                else
                    contourColor = jetHeatmap(v_node_value); // Use the heatmap for values between 0 and 1


	            f_Color = vec4(contourColor, 1.0f);
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
                case ShaderType.RsltPSLShader:
                    return rslt_psline_mesh_vert_shader();
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
                case ShaderType.ContourBarShader:
                    return contourbar_vert_shader();
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
                case ShaderType.RsltPSLShader:
                    return rslt_psline_mesh_frag_shader();
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
                case ShaderType.ContourBarShader:
                    return contourbar_frag_shader();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), "Unknown shader type");

            }
        }

        //___________________

    }
}
