using OpenTK.Graphics.ES11;
using System;
using System.Runtime.InteropServices;
using game.management;

namespace game.content {
    public abstract class Mesh2D {
        #region Transformation
        public Vector2D Pos { get; set; }

        public float Rotation { get; set; }

        public Vector2D Scale { get; set; }
        #endregion

        #region Data
        public Rect2D BoundingBox { get; set; }

        public Rect2D TransformedBoundingBox { get { return BoundingBox.Scale (Scale).Offset (Pos); } }
        #endregion

        #region Init methods
        public virtual void Init () {
            Scale = new Vector2D (1, 1);
        }

        public virtual void Shutdown () {
            Pos = Vector2D.Zero;
            Rotation = 0;
            Scale = Vector2D.Zero;
        }
        #endregion

        #region Rendering methods
        public void Render () {
            GL.PushMatrix ();

            GL.Translate (Pos.X, Pos.Y, 0.0f);
            GL.Rotate (Rotation, 0.0f, 0.0f, 1.0f);
            GL.Scale (Scale.X, Scale.Y, 1);

            RenderMesh ();

            GL.PopMatrix ();
        }

        protected abstract void RenderMesh ();
        #endregion

        #region Helper methods
        protected int LoadTextureFromAsset (string asset) {
            int texID = 0;
            GL.GenTextures (1, ref texID);
            GL.BindTexture (All.Texture2D, texID);

            using (var image = Game.ContentManager.LoadImage (asset)) {
                IntPtr pixels = Game.ContentManager.LockPixels (image);
                GL.TexImage2D (All.Texture2D, 0, (int)All.Rgba, Game.ContentManager.GetWidth (image), Game.ContentManager.GetHeight (image), 0, All.Rgba, All.UnsignedByte, pixels);
                Game.ContentManager.UnlockPixels (image);
            }

            GL.TexParameter (All.Texture2D, All.TextureMinFilter, (int)All.Linear);
            GL.TexParameter (All.Texture2D, All.TextureMagFilter, (int)All.Linear);

            return texID;
        }

        protected Rect2D CalculateBoundingBox (float[] vertices) {
            Vector2D min = new Vector2D (float.MaxValue, float.MaxValue);
            Vector2D max = new Vector2D (float.MinValue, float.MinValue);

            for (int i = 0; i < vertices.Length; i += 2) {
                Vector2D pt = new Vector2D (vertices[i], vertices[i+1]);
                if (pt < min)
                    min = pt;

                if (pt > max)
                    max = pt;
            }

            return new Rect2D (min, max);
        }

        protected int NewVBO<T> (T[] data) where T : struct  {
            int vboID = 0;
            GL.GenBuffers (1, ref vboID);

            GL.BindBuffer (All.ArrayBuffer, vboID);
            GL.BufferData<T> (All.ArrayBuffer, (IntPtr)(data.Length * Marshal.SizeOf (typeof (T))), data, All.StaticDraw);

            return vboID;
        }

        protected int[] NewTexturedVBO (int texID, float[] vertices = null, float[] texCoords = null) {
            if (vertices == null) {
                vertices = new float[] {
                    -1.0f, -1.0f,
                    1.0f, -1.0f,
                    -1.0f, 1.0f,
                    1.0f, 1.0f
                };
            }

            BoundingBox = CalculateBoundingBox (vertices);

            return new int[] {
                NewVBO<float> (vertices),
                NewVBO<float> (texCoords == null ? new float[] {
                    0.0f, 0.0f,
                    1.0f, 0.0f,
                    0.0f, 1.0f,
                    1.0f, 1.0f
                } : texCoords)
            };
        }

        protected void RenderTexturedVBO (int texID, int vertCoordID, int texCoordID, All mode = All.TriangleStrip, int vertexCount = 4) {
            GL.Enable (All.Blend);
            GL.BlendFunc (All.SrcAlpha, All.OneMinusSrcAlpha);

            GL.Enable (All.Texture2D);
            GL.BindTexture (All.Texture2D, texID);

            GL.BindBuffer (All.ArrayBuffer, vertCoordID);
            GL.VertexPointer (2, All.Float, 0, IntPtr.Zero);
            GL.EnableClientState (All.VertexArray);

            GL.BindBuffer (All.ArrayBuffer, texCoordID);
            GL.TexCoordPointer (2, All.Float, 0, IntPtr.Zero);
            GL.EnableClientState (All.TextureCoordArray);

            GL.DrawArrays (mode, 0, vertexCount);
        }
        #endregion
    }
}