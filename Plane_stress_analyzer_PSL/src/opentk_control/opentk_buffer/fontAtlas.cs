// OpenTK library
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using SharpFont;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Plane_stress_analyzer_PSL.src.opentk_control.opentk_buffer
{
    /// <summary>
    /// Holds glyph metrics and atlas coordinates for one character.
    /// </summary>
    public class Character
    {
        public Vector2 Size;        // Glyph width & height
        public Vector2 Bearing;     // Offset from baseline to left/top
        public int Advance;         // Horizontal advance (in 1/64 pixels)
        public Vector2 TopLeft;     // Top-left texture coordinate
        public Vector2 BottomRight; // Bottom-right texture coordinate
    }

    /// <summary>
    /// Builds a font atlas texture for ASCII glyphs using FreeType.
    /// </summary>
    public class fontAtlas : IDisposable
    {
        public int TextureID { get; private set; } = 0;
        public int TextureWidth { get; private set; } = 0;
        public int TextureHeight { get; private set; } = 0;
        public Dictionary<char, Character> Glyphs { get; private set; } = new Dictionary<char, Character>();

        private Library ftLibrary;
        private Face ftFace;

        public fontAtlas()
        {
            ftLibrary = new Library();
        }

        public void CreateAtlas()
        {

            // Set the freetype texture
            // initialize library
            Library lib = new Library();
            uint pixelHeight = 64;

            // Get the FreeSans.ttf from the resource
            byte[] res_ttf = Resource_font.HyperFont;
            MemoryStream ms = new MemoryStream(res_ttf);


            // Load font face
            ftFace = new Face(lib, ms.ToArray(), 0);
            ftFace.SetPixelSizes(0, (uint)pixelHeight);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            // Measure atlas dimensions
            int atlasWidth = 0;
            int atlasHeight = 0;

            for (uint c = 0; c < 128; c++)
            {
                ftFace.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
                atlasWidth += ftFace.Glyph.Bitmap.Width;
                atlasHeight = Math.Max(atlasHeight, ftFace.Glyph.Bitmap.Rows);
            }

            TextureWidth = atlasWidth;
            TextureHeight = atlasHeight;

            // Create OpenGL texture
            TextureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8,
                          atlasWidth, atlasHeight, 0,
                          PixelFormat.Red, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Fill texture with glyph bitmaps
            int xOffset = 0;

            for (uint c = 0; c < 128; c++)
            {
                // Load glyph
                ftFace.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);

                GlyphSlot glyph = ftFace.Glyph;
                FTBitmap bitmap = glyph.Bitmap;

                // Upload glyph bitmap into the atlas texture
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, xOffset, 0,
                                 bitmap.Width, bitmap.Rows,
                                 PixelFormat.Red, PixelType.UnsignedByte,
                                 bitmap.Buffer);

                // Create glyph character info
                Character ch = new Character
                {
                    Size = new Vector2(bitmap.Width, bitmap.Rows),
                    Bearing = new Vector2(glyph.BitmapLeft, glyph.BitmapTop),
                    Advance = (int)glyph.Advance.X,
                    TopLeft = new Vector2((float)xOffset / atlasWidth, 0f),
                    BottomRight = new Vector2((float)(xOffset + bitmap.Width) / atlasWidth,
                                              (float)bitmap.Rows / atlasHeight)
                };

                Glyphs[(char)c] = ch;
                xOffset += bitmap.Width;
            }

            // Console.WriteLine($"✅ Font atlas created ({atlasWidth}x{atlasHeight})");

        }

        public void Dispose()
        {
            if (ftFace != null)
            {
                ftFace.Dispose();
                ftFace = null;
            }

            if (ftLibrary != null)
            {
                ftLibrary.Dispose();
                ftLibrary = null;
            }

            if (TextureID != 0)
            {
                GL.DeleteTexture(TextureID);
                TextureID = 0;
            }
        }

    }


}
