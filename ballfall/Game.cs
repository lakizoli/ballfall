using System;
using OpenTK;
using OpenTK.Graphics.ES11;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ballfall {
    public class Game {
        #region Static data
        int width = 0;
        int height = 0;

        int[] gl_buffers = null;

        float[] square_vertices = {
            -0.5f, -0.5f,
            0.5f, -0.5f,
            -0.5f, 0.5f,
            0.5f, 0.5f,
        };

        byte[] square_colors = {
            255, 255,   0, 255,
            0,   255, 255, 255,
            0,     0,    0,  0,
            255,   0,  255, 255,
        };
        #endregion

        #region Dynamic data
        float _rotZ = 0;
        Vector2D _touchPos = null;
        Vector2D _touchMove = null;
        #endregion

        #region Interface
        public void Init (int width, int height) {
            InitProjection (width, height);

            gl_buffers = new int[] { 0, 0 };
            GL.GenBuffers (2, gl_buffers);

            GL.BindBuffer (All.ArrayBuffer, gl_buffers[0]);
            GL.BufferData<float> (All.ArrayBuffer, (IntPtr)(square_vertices.Length * sizeof (float)), square_vertices, All.StaticDraw);

            GL.BindBuffer (All.ArrayBuffer, gl_buffers[1]);
            GL.BufferData<byte> (All.ArrayBuffer, (IntPtr)(square_colors.Length * sizeof (byte)), square_colors, All.StaticDraw);

            //...
        }

        public void Shutdown () {
            GL.DeleteBuffers (2, gl_buffers);
            gl_buffers = null;
        }

        /// <summary>
        /// Resize game's surface.
        /// </summary>
        /// <param name="oldWidth">The old surface width in pixels.</param>
        /// <param name="oldHeight">The old surface height in pixels.</param>
        /// <param name="newWidth">The new surface width in pixels.</param>
        /// <param name="newHeight">The new surface height in pixels.</param>
        public void Resize (int oldWidth, int oldHeight, int newWidth, int newHeight) {
            InitProjection (newWidth, newHeight);
        }

        /// <summary>
        /// The update step of the game.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time from the last update in seconds.</param>
        public void Update (double elapsedTime) {
            _rotZ += 1;
        }

        public void Render () {
            GL.ClearColor (0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear ((uint)All.ColorBufferBit);

            GL.MatrixMode (All.Modelview);
            GL.LoadIdentity ();

            GL.PushMatrix ();
            if (_touchMove != null) {
                GL.Translate (_touchMove.X, _touchMove.Y, 0.0f);
            }
            GL.Rotate ((_rotZ / 1.0f) % 360.0f, 0.0f, 0.0f, 1.0f);

            GL.BindBuffer (All.ArrayBuffer, gl_buffers[0]);
            GL.VertexPointer (2, All.Float, 0, IntPtr.Zero);
            GL.EnableClientState (All.VertexArray);

            GL.BindBuffer (All.ArrayBuffer, gl_buffers[1]);
            GL.ColorPointer (4, All.UnsignedByte, 0, IntPtr.Zero);
            GL.EnableClientState (All.ColorArray);

            GL.DrawArrays (All.TriangleStrip, 0, 4);

            GL.PopMatrix ();
        }
        #endregion

        #region Input handlers
        public void TouchDown (int fingerID, float x, float y) {
            if (fingerID == 0 && _touchPos == null) {
                _touchPos = ToLocal (x, y);
                _touchMove = new Vector2D (0, 0);
            }
        }

        public void TouchUp (int fingerID, float x, float y) {
            if (fingerID == 0 && _touchPos != null) {
                _touchPos = null;
                _touchMove = null;
            }
        }

        public void TouchMove (int fingerID, float x, float y) {
            if (fingerID == 0 && _touchPos != null) {
                _touchMove = ToLocal (x, y) - _touchPos;
            }
        }
        #endregion

        #region Inner methods
        private void InitProjection (int width, int height) {
            this.width = width;
            this.height = height;

            GL.MatrixMode (All.Projection);
            GL.LoadIdentity ();

            float min = Math.Min (width, height);
            float max = Math.Max (width, height);
            float aspect = max / min;
            if (width <= height) {
                GL.Ortho (0, 1.0f, aspect, 0, -1.0f, 1.0f);
            } else {
                GL.Ortho (0, aspect, 1.0f, 0, -1.0f, 1.0f);
            }
        }

        private Vector2D ToLocal (float x, float y) {
            float min = Math.Min (width, height);
            float max = Math.Max (width, height);
            float aspect = max / min;
            if (width <= height) {
                return new Vector2D (x / (float)width, aspect * y / (float)height);
            }

            return new Vector2D (aspect * x / (float)width, y / (float)height);
        }

        #endregion
    }
}
