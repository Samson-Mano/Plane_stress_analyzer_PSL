// OpenTK library
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using SharpFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Plane_stress_analyzer_PSL.src.opentk_control.opentk_buffer
{
    public class VertexBuffer : IDisposable
    {
        private int _rendererId;
        private int _capacity;  // Current capacity in bytes
        private int _size;      // Current used size in bytes
        private bool _disposed = false;

        public int Size => _size;
        public int Capacity => _capacity;



        public VertexBuffer(int vertexbuffer_count = 10)  // Note: Data count is the number of float count
        {
            // Main Constructor
            _rendererId = GL.GenBuffer();
            _capacity = vertexbuffer_count * sizeof(float);
            _size = 0;

            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, _capacity, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            UnBind();
        }

        public void AppendVertexBuffer(float[] vertexbuffer_data)
        {
            int vertexbuffer_size = vertexbuffer_data.Length * sizeof(float);

            Bind();

            // Grow buffer if needed
            if (_size + vertexbuffer_size > _capacity)
            {
                Grow(Math.Max(_capacity * 2, _size + vertexbuffer_size));
            }

            // Append data at the current size position
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)_size, vertexbuffer_size, vertexbuffer_data);
            _size += vertexbuffer_size;

            UnBind();
        }


        public void updateVertexBuffer(float[] vertexbuffer_data)
        {
            int vertexbuffer_size = vertexbuffer_data.Length * sizeof(float);

            // Important!! Call only in Dynamic Buffer case
            // Update the vertex data
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._rendererId);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertexbuffer_size, vertexbuffer_data);

        }



        private void Grow(int newCapacity)
        {
            // Create new buffer with larger capacity
            int newBufferId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, newBufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, newCapacity, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            // Copy existing data
            GL.BindBuffer(BufferTarget.CopyReadBuffer, _rendererId);
            GL.BindBuffer(BufferTarget.CopyWriteBuffer, newBufferId);
            GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer,
                                IntPtr.Zero, IntPtr.Zero, _size);

            // Replace old buffer
            GL.DeleteBuffer(_rendererId);
            _rendererId = newBufferId;
            _capacity = newCapacity;

            UnBind();
        }

        public void ClearVertexBuffer()
        {
            _size = 0;
            // Optional: Reset GPU memory (clear to zeros)
            Bind();
            IntPtr zero = IntPtr.Zero;
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, _capacity, ref zero);
            UnBind();
        }

        public void Bind() => GL.BindBuffer(BufferTarget.ArrayBuffer, _rendererId);
        public void UnBind() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        public void Dispose()
        {
            if (!_disposed)
            {
                GL.DeleteBuffer(_rendererId);
                _disposed = true;
            }
        }

    }
}
