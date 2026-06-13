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

using Plane_stress_analyzer_PSL.Resources;


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

                // Skip invalid glyphs with zero size (avoids potential issues)
                if (bitmap.Width <= 0 || bitmap.Rows <= 0)
                {
                    // Still add a placeholder character
                    Glyphs[(char)c] = CreatePlaceholderCharacter(xOffset, atlasWidth, atlasHeight);
                    continue;
                }

                // Upload glyph bitmap into the atlas texture
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, xOffset, 0,
                                 bitmap.Width, bitmap.Rows,
                                 PixelFormat.Red, PixelType.UnsignedByte,
                                 bitmap.Buffer);

                // Create glyph character info safely
                Character ch = CreateSafeCharacter(glyph, bitmap, xOffset, atlasWidth, atlasHeight);
                Glyphs[(char)c] = ch;

                xOffset += bitmap.Width;
            }

            // Console.WriteLine($"✅ Font atlas created ({atlasWidth}x{atlasHeight})");
        }

        // Helper method to create character safely
        private Character CreateSafeCharacter(GlyphSlot glyph, FTBitmap bitmap,
                                              int xOffset, int atlasWidth, int atlasHeight)
        {
            // Safely get the advance value
            int safeAdvance = GetSafeAdvance(glyph);

            // Validate bitmap dimensions (clamp to reasonable values)
            float safeWidth = Math.Max(0, bitmap.Width);
            float safeRows = Math.Max(0, bitmap.Rows);

            // Validate bearing values
            float safeBitmapLeft = ValidateBearingValue(glyph.BitmapLeft);
            float safeBitmapTop = ValidateBearingValue(glyph.BitmapTop);

            return new Character
            {
                Size = new Vector2(safeWidth, safeRows),
                Bearing = new Vector2(safeBitmapLeft, safeBitmapTop),
                Advance = safeAdvance,
                TopLeft = new Vector2((float)xOffset / atlasWidth, 0f),
                BottomRight = new Vector2((float)(xOffset + safeWidth) / atlasWidth,
                                          safeRows / atlasHeight)
            };
        }

        // Safely extract advance value from glyph
        private int GetSafeAdvance(GlyphSlot glyph)
        {
            try
            {
                // glyph.Advance.X is a Fixed26Dot6 value (1/64th of a pixel)
                // Try to get the value safely
                int rawValue = (int)glyph.Advance.X;

                // Sanity check - most glyph advances are between 0 and 5000
                // (5000/64 ≈ 78 pixels, reasonable for a 64px font)
                if (rawValue < 0 || rawValue > 20000)
                {
                    // Log if you want to debug: Console.WriteLine($"Suspicious advance for char: {rawValue}, using default");
                    return 64; // Return 1 pixel as default (64 in 26.6 format)
                }

                return rawValue;
            }
            catch (OverflowException)
            {
                // Overflow occurred - provide a reasonable default
                // For a 64px font, typical advance is around 64-128 (1-2 pixels)
                return 64;
            }
        }

        // Validate bearing values (they can be negative and should be reasonable)
        private float ValidateBearingValue(int value)
        {
            // Clamp to reasonable range for a 64px font
            // Bearings typically range from -100 to +100 in pixels
            if (value < -500 || value > 500)
            {
                return 0; // Return safe default
            }
            return value;
        }

        // Create a placeholder for invalid glyphs
        private Character CreatePlaceholderCharacter(int xOffset, int atlasWidth, int atlasHeight)
        {
            return new Character
            {
                Size = new Vector2(1, 1),
                Bearing = new Vector2(0, 0),
                Advance = 64, // 1 pixel in 26.6 format
                TopLeft = new Vector2((float)xOffset / atlasWidth, 0f),
                BottomRight = new Vector2((float)(xOffset + 1) / atlasWidth, 1f / atlasHeight)
            };
        }



        //public void CreateAtlas()
        //{

        //    // Set the freetype texture
        //    // initialize library
        //    Library lib = new Library();
        //    uint pixelHeight = 64;

        //    // Get the FreeSans.ttf from the resource
        //    byte[] res_ttf = Resource_font.HyperFont;
        //    MemoryStream ms = new MemoryStream(res_ttf);


        //    // Load font face
        //    ftFace = new Face(lib, ms.ToArray(), 0);
        //    ftFace.SetPixelSizes(0, (uint)pixelHeight);

        //    GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

        //    // Measure atlas dimensions
        //    int atlasWidth = 0;
        //    int atlasHeight = 0;

        //    for (uint c = 0; c < 128; c++)
        //    {
        //        ftFace.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
        //        atlasWidth += ftFace.Glyph.Bitmap.Width;
        //        atlasHeight = Math.Max(atlasHeight, ftFace.Glyph.Bitmap.Rows);
        //    }

        //    TextureWidth = atlasWidth;
        //    TextureHeight = atlasHeight;

        //    // Create OpenGL texture
        //    TextureID = GL.GenTexture();
        //    GL.BindTexture(TextureTarget.Texture2D, TextureID);
        //    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8,
        //                  atlasWidth, atlasHeight, 0,
        //                  PixelFormat.Red, PixelType.UnsignedByte, IntPtr.Zero);

        //    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        //    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        //    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        //    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        //    // Fill texture with glyph bitmaps
        //    int xOffset = 0;

        //    for (uint c = 0; c < 128; c++)
        //    {
        //        // Load glyph
        //        ftFace.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);

        //        GlyphSlot glyph = ftFace.Glyph;
        //        FTBitmap bitmap = glyph.Bitmap;

        //        // Upload glyph bitmap into the atlas texture
        //        GL.TexSubImage2D(TextureTarget.Texture2D, 0, xOffset, 0,
        //                         bitmap.Width, bitmap.Rows,
        //                         PixelFormat.Red, PixelType.UnsignedByte,
        //                         bitmap.Buffer);

        //        // Create glyph character info
        //        Character ch = new Character
        //        {
        //            Size = new Vector2(bitmap.Width, bitmap.Rows),
        //            Bearing = new Vector2(glyph.BitmapLeft, glyph.BitmapTop),
        //            Advance = (int)glyph.Advance.X,
        //            TopLeft = new Vector2((float)xOffset / atlasWidth, 0f),
        //            BottomRight = new Vector2((float)(xOffset + bitmap.Width) / atlasWidth,
        //                                      (float)bitmap.Rows / atlasHeight)
        //        };

        //        Glyphs[(char)c] = ch;
        //        xOffset += bitmap.Width;
        //    }

        //    // Console.WriteLine($"✅ Font atlas created ({atlasWidth}x{atlasHeight})");

        //}

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
