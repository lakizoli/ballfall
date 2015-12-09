using game.content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ballfall.content {
    class QTEGoodBall : QuickTimeEvent {
        private Ball _ball;
        private Vector2D _ballOriginalScale;
        private LinearAnimation _scale;

        public bool IsEnded { get { return _scale.Value < 0.105f; } }

        public QTEGoodBall (Ball ball) {
            _ball = ball;
            _scale = new LinearAnimation (1.0f, 0.1f, 0.1f);
        }

        public override void Init () {
            base.Init ();
            _ballOriginalScale = _ball.Scale;
        }

        public override void Shutdown () {
            base.Shutdown ();
            _ball.Shutdown ();
        }

        protected override void OnStart () {
            base.OnStart ();
            _scale.Start ();
        }

        protected override void OnStop () {
            base.OnStop ();
            _scale.Stop ();
            _ball.Scale = _ballOriginalScale;
        }

        protected override void OnUpdate (float elapsedTime) {
            base.OnUpdate (elapsedTime);
            _scale.Update (elapsedTime);
            _ball.Scale = _ballOriginalScale * _scale.Value;
        }

        protected override void OnRender () {
            base.OnRender ();
            _ball.Render ();
        }
    }
}