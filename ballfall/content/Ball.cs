using System;
using OpenTK.Graphics.ES11;
using game.management;
using game.content;

namespace ballfall.content {
    public class Ball : Mesh2D {
        public enum Color {
            Red,
            Green,
            Blue,
            Yellow,
            Magic,
            Bomb
        }

        private Color _color;
        private int _tex;
        private int[] _vbo;

        public Color Type { get { return _color; } }

        public Ball (Color color) {
            _color = color;
        }

        public override void Init () {
            base.Init ();

            string asset;
            switch (_color) {
                case Color.Red: asset = "ball_red.png"; break;
                case Color.Green: asset = "ball_green.png"; break;
                case Color.Blue: asset = "ball_blue.png"; break;
                case Color.Yellow: asset = "ball_yellow.png"; break;
                case Color.Magic: asset = "ball_magic.png"; break; //TODO: magic-rõl az árnyék leszedése
                case Color.Bomb: asset = "bomb.png"; break; //TODO: bomb-ról az árnyék leszedése
                default:
                    throw new NotImplementedException ();
            }

            _tex = LoadTextureFromAsset (asset);
            _vbo = NewTexturedVBO (_tex);
        }

        public override void Shutdown () {
            if (_vbo != null) {
                GL.DeleteBuffers (_vbo.Length, _vbo);
                _vbo = null;
            }

            if (_tex > 0) {
                GL.DeleteTextures (1, ref _tex);
                _tex = 0;
            }

            base.Shutdown ();
        }

        protected override void RenderMesh () {
            RenderTexturedVBO (_tex, _vbo[0], _vbo[1]);
        }
    }

}