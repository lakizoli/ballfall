using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using game.content;
using game.management;

namespace ballfall.content {
    class QTEPulseBall : QuickTimeEvent {
        private Ball _ball;
        private Vector2D _ballOriginalScale;
        private PulseAnimation _pulse;

        public QTEPulseBall (Ball ball) {
            _ball = ball;
            _pulse = new PulseAnimation (2, 1);
        }

        public override void Init () {
            base.Init ();
            _ballOriginalScale = _ball.Scale;
        }

        protected override void OnStart () {
            base.OnStart ();
            _pulse.Start ();
        }

        protected override void OnStop () {
            base.OnStop ();
            _pulse.Stop ();
            _ball.Scale = _ballOriginalScale;
        }

        protected override void OnUpdate (float elapsedTime) {
            base.OnUpdate (elapsedTime);
            _pulse.Update (elapsedTime);
            _ball.Scale = _ballOriginalScale * _pulse.ScaleFactor;
        }

        protected override void OnRender () {
            base.OnRender ();
            _ball.Render ();
        }
    }
}