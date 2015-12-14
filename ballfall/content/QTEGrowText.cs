using game.content;
using game.management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ballfall.content {
    class QTEGrowText : QuickTimeEvent {
        private ImageMesh _test;
        private LinearAnimation _scale;

        public bool IsEnded { get { return _scale.Value >= 0.29f; } }

        public QTEGrowText (string asset, float timeFrame) {
            _test = new ImageMesh (asset);
            _scale = new LinearAnimation (0.01f, 0.3f, timeFrame);
        }

        public override void Init () {
            base.Init ();
            _test.Init ();
            _test.Scale = new Vector2D (0.01f, 0.01f);
            _test.Pos = new Vector2D (Game.Instance.ScreenWidth / 2.0f, Game.Instance.ScreenHeight / 2.0f);
        }

        public override void Shutdown () {
            base.Shutdown ();
            _test.Shutdown ();
        }

        protected override void OnStart () {
            base.OnStart ();
            _scale.Start ();
        }

        protected override void OnStop () {
            base.OnStop ();
            _scale.Stop ();
        }

        protected override void OnUpdate (float elapsedTime) {
            base.OnUpdate (elapsedTime);
            _scale.Update (elapsedTime);
            _test.Scale = new Vector2D (_scale.Value, _scale.Value);
        }

        protected override void OnRender () {
            base.OnRender ();
            _test.Render ();
        }
    }
}