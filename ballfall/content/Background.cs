using game.content;
using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ballfall.content {
    class Background : Mesh2D {
        private int _tex;
        private int[] _vbo;

        public override void Init () {
            base.Init ();

            _tex = LoadTextureFromAsset ("background.png");
            _vbo = NewTexturedVBO (_tex);
        }

        public override void Shutdown () {
            GL.DeleteBuffers (_vbo.Length, _vbo);
            _vbo = null;

            GL.DeleteTextures (1, ref _tex);
            _tex = 0;

            base.Shutdown ();
        }

        protected override void RenderMesh () {
            RenderTexturedVBO (_tex, _vbo[0], _vbo[1]);
        }
    }
}