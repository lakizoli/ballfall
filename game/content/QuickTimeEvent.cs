using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace game.content {
    public abstract class QuickTimeEvent {
        private bool _isStarted = false;
        private bool _isPaused = false;

        public virtual void Init () { }

        public virtual void Shutdown () { }

        public void Start () {
            if (_isStarted)
                return;

            _isStarted = true;
            _isPaused = false;

            OnStart ();
        }

        public void Stop () {
            if (!_isStarted)
                return;

            _isStarted = false;
            _isPaused = false;

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

            OnUpdate (elapsedTime);
        }

        public void Render () {
            if (!_isStarted)
                return;

            OnRender ();
        }

        protected virtual void OnStart () { }

        protected virtual void OnStop () { }

        protected virtual void OnPause () { }

        protected virtual void OnContinue () { }

        protected virtual void OnUpdate (float elapsedTime) { }

        protected virtual void OnRender () { }
    }
}