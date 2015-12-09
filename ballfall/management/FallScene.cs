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
        #region Definitions
        class BallRecord {
            public Ball ball;
            public RigidBody2D body;
        }
        #endregion

        #region Data
        Random _random;
        float _fullTime;
        float _lastAddTime;
        List<BallRecord> _balls;

        Dictionary<int, Vector2D> _touchPositions = new Dictionary<int, Vector2D> ();
        #endregion

        public FallScene () {
        }

        public override void Init (int width, int height) {
            _random = new Random ();
            _fullTime = 0;
            _lastAddTime = 0;
            _balls = new List<BallRecord> ();

            //_ballRed.Init ();
            //_ballGreen.Init ();
            //_ballBlue.Init ();
            //_ballMagic.Init ();

            //_ballRed.Scale = new Vector2D (0.1f, 0.1f);
            //_ballGreen.Scale = new Vector2D (0.1f, 0.1f);
            //_ballBlue.Scale = new Vector2D (0.1f, 0.1f);
            //_ballMagic.Scale = new Vector2D (0.1f, 0.1f);

            //_pulseRed.Init ();
        }

        public override void Shutdown () {
            foreach (BallRecord item in _balls)
                item.ball.Shutdown ();
            _balls.Clear ();

            //_pulseRed.Shutdown ();

            //_ballMagic.Shutdown ();
            //_ballBlue.Shutdown ();
            //_ballGreen.Shutdown ();
            //_ballRed.Shutdown ();
        }

        public override void Update (float elapsedTime) {
            //Measure time
            _fullTime += elapsedTime;
            _lastAddTime += elapsedTime;

            //Add new ball to the system
            if (_lastAddTime > 2.0f) {
                int rand = _random.Next (3);
                Ball ball = null;
                switch (_random.Next (3)) {
                    default:
                    case 0: ball = new Ball (Ball.Color.Red); break;
                    case 1: ball = new Ball (Ball.Color.Green); break;
                    case 2: ball = new Ball (Ball.Color.Blue); break;
                    case 3: ball = new Ball (Ball.Color.Magic); break;
                }

                float newX = _random.Next ((int)(Game.Instance.ScreenWidth * 1000.0f)) / 1000.0f;

                ball.Init ();
                ball.Pos = new Vector2D (newX, 0.2f);
                ball.Scale = new Vector2D (0.1f, 0.1f);

                RigidBody2D body = new RigidBody2D () {
                    Pos = ball.Pos,
                    Mass = 1.0f, //1 kg
                    Force = new Vector2D (0, 1.0f * RigidBody2D.Gravity)
                };

                _balls.Add (new BallRecord () {
                    ball = ball,
                    body = body
                });
                _lastAddTime = 0;
            }

            //Calculate physic
            foreach (BallRecord item in _balls) {
                item.body.Update (elapsedTime);
                item.ball.Pos = item.body.Pos;
            }

            //Remove balls not on screen
            //...

            //if (_touchPositions.ContainsKey (0)) {
            //    if (!_pulseRed.IsStarted ())
            //        _pulseRed.Start ();

            //    _pulseRed.Update (elapsedTime);
            //} else {
            //    if (_pulseRed.IsStarted ())
            //        _pulseRed.Stop ();
            //}

            //...
        }

        public override void Render () {
            GL.ClearColor (0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear ((uint)All.ColorBufferBit);

            GL.MatrixMode (All.Modelview);
            GL.LoadIdentity ();

            int index = 0;
            foreach (BallRecord item in _balls) {
                Game.Util.Log ("index: " + index++ + ", pos: " + item.ball.Pos);
                item.ball.Render ();
            }

            //if (_touchPositions.ContainsKey (0)) {
            //    _ballRed.Pos = _touchPositions[0];
            //    if (_pulseRed.IsStarted ())
            //        _pulseRed.Render ();
            //    else
            //        _ballRed.Render ();
            //}

            //if (_touchPositions.ContainsKey (1)) {
            //    _ballGreen.Pos = _touchPositions[1];
            //    _ballGreen.Render ();
            //}

            //if (_touchPositions.ContainsKey (2)) {
            //    _ballBlue.Pos = _touchPositions[2];
            //    _ballBlue.Render ();
            //}

            //if (_touchPositions.ContainsKey (3)) {
            //    _ballMagic.Pos = _touchPositions[3];
            //    _ballMagic.Render ();
            //}
        }

        //public override void TouchDown (int fingerID, float x, float y) {
        //    base.TouchDown (fingerID, x, y);

        //    if (!_touchPositions.ContainsKey (fingerID)) {
        //        _touchPositions.Add (fingerID, Game.Instance.ToLocal (x, y));
        //    }
        //}

        //public override void TouchUp (int fingerID, float x, float y) {
        //    base.TouchUp (fingerID, x, y);

        //    if (_touchPositions.ContainsKey (fingerID)) {
        //        _touchPositions.Remove (fingerID);
        //    }
        //}

        //public override void TouchMove (int fingerID, float x, float y) {
        //    base.TouchMove (fingerID, x, y);

        //    if (_touchPositions.ContainsKey (fingerID)) {
        //        _touchPositions[fingerID] = Game.Instance.ToLocal (x, y);
        //    }
        //}
    }
}