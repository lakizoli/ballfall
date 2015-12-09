using ballfall.content;
using game.content;
using game.management;
using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ballfall.management {
    public class FallScene : Scene {
        #region Static data
        Ball _ballRed;
        Ball _ballGreen;
        Ball _ballBlue;
        Ball _ballMagic;
        #endregion

        #region Dynamic data
        Dictionary<int, Vector2D> _touchPositions = new Dictionary<int, Vector2D> ();
        #endregion

        public FallScene (Game game) : base (game) {
            _ballRed = new Ball (Ball.Color.Red);
            _ballGreen = new Ball (Ball.Color.Green);
            _ballBlue = new Ball (Ball.Color.Blue);
            _ballMagic = new Ball (Ball.Color.Magic);
        }

        public override void Init (int width, int height) {
            _ballRed.Init (_game);
            _ballGreen.Init (_game);
            _ballBlue.Init (_game);
            _ballMagic.Init (_game);

            _ballRed.Scale = new Vector2D (0.1f, 0.1f);
            _ballGreen.Scale = new Vector2D (0.1f, 0.1f);
            _ballBlue.Scale = new Vector2D (0.1f, 0.1f);
            _ballMagic.Scale = new Vector2D (0.1f, 0.1f);
        }
        public override void Shutdown () {
            _ballMagic.Shutdown (_game);
            _ballBlue.Shutdown (_game);
            _ballGreen.Shutdown (_game);
            _ballRed.Shutdown (_game);
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

        public override void TouchDown (int fingerID, float x, float y) {
            base.TouchDown (fingerID, x, y);

            if (!_touchPositions.ContainsKey (fingerID)) {
                _touchPositions.Add (fingerID, _game.ToLocal (x, y));
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
                _touchPositions[fingerID] = _game.ToLocal (x, y);
            }
        }
    }
}