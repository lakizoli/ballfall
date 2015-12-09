using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace game.content {
    public abstract class Animation {
        private bool _isStarted = false;
        private bool _isPaused = false;
        protected float _timeOffset = 0;

        public void Start () {
            if (_isStarted)
                return;

            _isStarted = true;
            _isPaused = false;
            _timeOffset = 0;

            OnStart ();
        }

        public void Stop () {
            if (!_isStarted)
                return;

            _isStarted = false;
            _isPaused = false;
            _timeOffset = 0;

            OnStop ();
        }

        public bool IsStarted () {
            return _isStarted;
        }

        public void Pause () {
            if (!_isStarted || _isPaused)
                return;

            _isPaused = true;

            OnPause ();
        }

        public void Continue () {
            if (!_isStarted || !_isPaused)
                return;

            _isPaused = false;

            OnContinue ();
        }

        public bool IsPaused () {
            return _isPaused;
        }

        public void Update (float elapsedTime) {
            if (!_isStarted || _isPaused)
                return;

            _timeOffset += elapsedTime;

            OnUpdate (elapsedTime);
        }

        protected virtual void OnStart () { }

        protected virtual void OnStop () { }

        protected virtual void OnPause () { }

        protected virtual void OnContinue () { }

        protected virtual void OnUpdate (float elapsedTime) { }
    }

    public class PulseAnimation : Animation {
        private float _topScale;
        private float _timeFrame;

        private float _scale;
        public float Scale { get { return _scale; } }

        public PulseAnimation (float topScale, float timeFrame) {
            _topScale = topScale;
            _timeFrame = timeFrame;
            _scale = 1.0f;
        }

        protected override void OnUpdate (float elapsedTime) {
            base.OnUpdate (elapsedTime);

            float offsetInFrame = _timeOffset % _timeFrame;
            float halfTime = _timeFrame / 2.0f;

            if (offsetInFrame >= _timeFrame / 2.0f) //go down with scale
                _scale = (_topScale - 1.0f) * (2.0f * halfTime - offsetInFrame) / halfTime + 1.0f;
            else //go up with scale
                _scale = (_topScale - 1.0f) * offsetInFrame / halfTime + 1.0f;
        }
    }
}