using game.content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ballfall.content {
    class QTEExplodeBall : QuickTimeEvent {
        private Ball _ball;
        private ImageMesh _explosion;
        private FrameAnimation _frame;

        public bool IsEnded { get { return _frame.Frame >10; } }

        public QTEExplodeBall (Ball ball) {
            _ball = ball;
            _explosion = new ImageMesh ("explosion.png");
            _frame = new FrameAnimation (0.2f);
        }

        public override void Init () {
            base.Init ();
            _explosion.Init ();
            _explosion.Scale = new Vector2D (0.2f, 0.2f);
        }

        public override void Shutdown () {
            base.Shutdown ();
            _explosion.Shutdown ();
            _ball.Shutdown ();
        }

        protected override void OnStart () {
            base.OnStart ();
            _frame.Start ();
        }

        protected override void OnStop () {
            base.OnStop ();
            _frame.Stop ();
        }

        protected override void OnUpdate (float elapsedTime) {
            base.OnUpdate (elapsedTime);
            _frame.Update (elapsedTime);
            _explosion.Rotation = (_frame.Frame % 4) * 90.0f;
            _explosion.Pos = _ball.Pos;
        }

        protected override void OnRender () {
            base.OnRender ();
            _ball.Render ();
            _explosion.Render ();
        }
    }
}