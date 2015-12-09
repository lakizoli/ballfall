using System.Collections.Generic;
using OpenTK.Graphics.ES11;
using game.management;
using game.content;
using ballfall.content;

namespace ballfall.management {
    public class BallFallGame : Game {
        #region Static data
        Ball _ballRed;
        Ball _ballGreen;
        Ball _ballBlue;
        Ball _ballMagic;
        #endregion

        #region Dynamic data
        Dictionary<int, Vector2D> _touchPositions = new Dictionary<int, Vector2D> ();
        #endregion

        #region Construction
        public BallFallGame (IUtil util, IContentManager contentManager) : base (util, contentManager) {
            _ballRed = new Ball (Ball.Color.Red);
            _ballGreen = new Ball (Ball.Color.Green);
            _ballBlue = new Ball (Ball.Color.Blue);
            _ballMagic = new Ball (Ball.Color.Magic);
        }
        #endregion

        #region Interface
        public override void Init (int width, int height) {
            base.Init (width, height);

            _ballRed.Init (this);
            _ballGreen.Init (this);
            _ballBlue.Init (this);
            _ballMagic.Init (this);

            _ballRed.Scale = new Vector2D (0.1f, 0.1f);
            _ballGreen.Scale = new Vector2D (0.1f, 0.1f);
            _ballBlue.Scale = new Vector2D (0.1f, 0.1f);
            _ballMagic.Scale = new Vector2D (0.1f, 0.1f);
        }

        public override void Shutdown () {
            _ballMagic.Shutdown (this);
            _ballBlue.Shutdown (this);
            _ballGreen.Shutdown (this);
            _ballRed.Shutdown (this);

            base.Shutdown ();
        }

        public override void Update (double elapsedTime) {
            //...
        }

        public override void Render () {
            GL.ClearColor (0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear ((uint)All.ColorBufferBit);

            GL.MatrixMode (All.Modelview);
            GL.LoadIdentity ();

            if (_touchPositions.ContainsKey (0)) {
                _ballRed.Pos = _touchPositions[0];
                _ballRed.Render ();
            }

            if (_touchPositions.ContainsKey (1)) {
                _ballGreen.Pos = _touchPositions[1];
                _ballGreen.Render ();
            }

            if (_touchPositions.ContainsKey (2)) {
                _ballBlue.Pos = _touchPositions[2];
                _ballBlue.Render ();
            }

            if (_touchPositions.ContainsKey (3)) {
                _ballMagic.Pos = _touchPositions[3];
                _ballMagic.Render ();
            }
        }
        #endregion

        #region Input handlers
        public override void TouchDown (int fingerID, float x, float y) {
            base.TouchDown (fingerID, x, y);

            if (!_touchPositions.ContainsKey (fingerID)) {
                _touchPositions.Add (fingerID, ToLocal (x, y));
            }
        }

        public override void TouchUp (int fingerID, float x, float y) {
            base.TouchUp (fingerID, x, y);

            if (_touchPositions.ContainsKey (fingerID)) {
                _touchPositions.Remove (fingerID);
            }
        }

        public override void TouchMove (int fingerID, float x, float y) {
            base.TouchMove (fingerID, x, y);

            if (_touchPositions.ContainsKey (fingerID)) {
                _touchPositions[fingerID] = ToLocal (x, y);
            }
        }
        #endregion
    }
}