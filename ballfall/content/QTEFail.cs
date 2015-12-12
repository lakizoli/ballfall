using game.content;
using game.management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ballfall.content {
    class QTEFail : QuickTimeEvent {
        private ImageMesh _failText;
        private LinearAnimation _scale;

        public bool IsEnded { get { return _scale.Value >= 0.29f; } }

        public QTEFail () {
            _failText = new ImageMesh ("fail.png");
            _scale = new LinearAnimation (0.01f, 0.3f, 0.1f);
        }

        public override void Init () {
            base.Init ();
            _failText.Init ();
            _failText.Scale = new Vector2D (0.01f, 0.01f);
            _failText.Pos = new Vector2D (Game.Instance.ScreenWidth / 2.0f, Game.Instance.ScreenHeight / 2.0f);
        }

        public override void Shutdown () {
            base.Shutdown ();
            _failText.Shutdown ();
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
            _failText.Scale = new Vector2D (_scale.Value, _scale.Value);
        }

        protected override void OnRender () {
            base.OnRender ();
            _failText.Render ();
        }
    }
}