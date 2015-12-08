using OpenTK.Graphics.ES11;
using System;
using System.Runtime.InteropServices;

namespace ballfall {
    abstract class Mesh2D {
        #region Transformation
        public Vector2D Pos { get; set; }

        public float Rotation { get; set; }

        public Vector2D Scale { get; set; }
        #endregion

        #region Init methods
        public virtual void Init (ISystem sys) {
            Scale = new Vector2D (1, 1);
        }

        public virtual void Shutdown (ISystem sys) {
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
        protected int LoadTextureFromAsset (IContentManager contentManager, string asset) {
            int texID = 0;
            GL.GenTextures (1, ref texID);
            GL.BindTexture (All.Texture2D, texID);

            using (var image = contentManager.LoadImage (asset)) {
                IntPtr pixels = contentManager.LockPixels (image);
                GL.TexImage2D (All.Texture2D, 0, (int)All.Rgba, contentManager.GetWidth (image), contentManager.GetHeight (image), 0, All.Rgba, All.UnsignedByte, pixels);
                contentManager.UnlockPixels (image);
            }

            GL.TexParameter (All.Texture2D, All.TextureMinFilter, (int)All.Linear);
            GL.TexParameter (All.Texture2D, All.TextureMagFilter, (int)All.Linear);

            return texID;
        }

        protected int NewVBO<T> (T[] data) where T : struct  {
            int vboID = 0;
            GL.GenBuffers (1, ref vboID);

            GL.BindBuffer (All.ArrayBuffer, vboID);
            GL.BufferData<T> (All.ArrayBuffer, (IntPtr)(data.Length * Marshal.SizeOf (typeof (T))), data, All.StaticDraw);

            return vboID;
        }

        protected int[] NewTexturedVBO (int texID, float[] vertices = null, float[] texCoords = null) {
            return new int[] {
                NewVBO<float> (vertices == null ? new float[] {
                    -1.0f, -1.0f,
                    1.0f, -1.0f,
                    -1.0f, 1.0f,
                    1.0f, 1.0f
                } : vertices),
                NewVBO<float> (texCoords == null ? new float[] {
                    0.0f, 0.0f,
                    1.0f, 0.0f,
                    0.0f, 1.0f,
                    1.0f, 1.0f
                } : texCoords)
            };
        }

        protected void RenderTexturedVBO (int texID, int vertCoordID, int texCoordID, All mode = All.TriangleStrip, int vertexCount = 4) {
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

    class Ball : Mesh2D {
        public enum Color {
            Red,
            Green,
            Blue,
            Magic
        }

        private Color _color;
        private int _tex;
        private int[] _vbo;

        public Ball (Color color) {
            _color = color;
        }

        public override void Init (ISystem sys) {
            base.Init (sys);

            string asset;
            switch (_color) {
                case Color.Red: asset = "ball_red.png"; break;
                case Color.Green: asset = "ball_green.png"; break;
                case Color.Blue: asset = "ball_blue.png"; break;
                case Color.Magic: asset = "ball_magic.png"; break; //TODO: magic-rõl az árnyék leszedése
                default:
                    throw new NotImplementedException ();
            }

            _tex = LoadTextureFromAsset (sys.ContentManager, asset);
            _vbo = NewTexturedVBO (_tex);
        }

        public override void Shutdown (ISystem sys) {
            GL.DeleteBuffers (_vbo.Length, _vbo);
            _vbo = null;

            GL.DeleteTextures (1, ref _tex);
            _tex = 0;

            base.Shutdown (sys);
        }

        protected override void RenderMesh () {
            RenderTexturedVBO (_tex, _vbo[0], _vbo[1]);
        }
    }
}