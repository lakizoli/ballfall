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
        struct FallingBall {
            public Ball ball;
            public RigidBody2D body;
            public float lastTouch;
        }

        struct LevelDefinition {
            public int index; //The index of the level
            public float endTime; //The absolute end time of the level [sec]
            public int minAddTime; //minimal time of next add time range [millisec]
            public int maxAddTime; //maximal time of next add time range [millisec]
            public float startVelocityY;
        }

        enum RegionTest {
            NotInRegion,
            GoodRegion,
            WrongRegion
        }
        #endregion

        #region Data
        List<LevelDefinition> _levels;
        int _currentLevel;

        Random _random;
        float _fullTime;
        float _lastAddTime;

        Rect2D _yellowRegion;
        Rect2D _greenRegion;
        Rect2D _redRegion;
        Rect2D _blueRegion;

        List<FallingBall> _fallingBalls;
        List<QTEGoodBall> _endedBalls;
        Ball _failBall;
        Background _background;

        Dictionary<int, FallingBall> _touchedBalls;
        #endregion

        public FallScene () {
        }

        public override void Init (int width, int height) {
            _levels = new List<LevelDefinition> ();
            _levels.Add (new LevelDefinition () { index = 0, endTime = 30, minAddTime = 1000, maxAddTime = 2000, startVelocityY = 0.0f });
            _levels.Add (new LevelDefinition () { index = 1, endTime = 60, minAddTime = 850, maxAddTime = 1500, startVelocityY = 0.5f });
            _levels.Add (new LevelDefinition () { index = 2, endTime = 90, minAddTime = 750, maxAddTime = 1200, startVelocityY = 1.0f });
            _currentLevel = 0;

            _random = new Random ();
            _fullTime = 0;
            _lastAddTime = 0;

            _yellowRegion = new Rect2D (Game.Instance.ToLocal (0, 65 * 4), Game.Instance.ToLocal (20 * 4, 265 * 4));
            _greenRegion = new Rect2D (Game.Instance.ToLocal (0, 265 * 4), Game.Instance.ToLocal (20 * 4, 480 * 4));
            _redRegion = new Rect2D (Game.Instance.ToLocal (width - 20 * 4, 65 * 4), Game.Instance.ToLocal (width, 265 * 4));
            _blueRegion = new Rect2D (Game.Instance.ToLocal (width - 20 * 4, 265 * 4), Game.Instance.ToLocal (width, 480 * 4));

            _fallingBalls = new List<FallingBall> ();
            _endedBalls = new List<QTEGoodBall> ();

            _background = new Background ();
            _background.Init ();

            Vector2D screenSize = Game.Instance.ToLocal (width, height);
            _background.Pos = screenSize / 2.0f;
            _background.Scale = screenSize / _background.BoundingBox.Size;

            _touchedBalls = new Dictionary<int, FallingBall> ();
        }

        public override void Shutdown () {
            _touchedBalls = null;

            _background.Shutdown ();

            if (_failBall != null) {
                _failBall.Shutdown ();
                _failBall = null;
            }

            foreach (QTEGoodBall item in _endedBalls) {
                item.Stop ();
                item.Shutdown ();
            }
            _endedBalls = null;

            foreach (FallingBall item in _fallingBalls)
                item.ball.Shutdown ();
            _fallingBalls = null;

            _levels = null;
        }

        public override void Resize (int oldWidth, int oldHeight, int newWidth, int newHeight) {
            _background.Scale = _background.BoundingBox.Size / Game.Instance.ToLocal (newWidth, newHeight);
        }

        public override void Update (float elapsedTime) {
            //Measure time
            _fullTime += elapsedTime;
            _lastAddTime += elapsedTime;

            //Pick next level
            LevelDefinition level = _levels[_currentLevel];

            if (_fullTime > level.endTime && _currentLevel < _levels.Count - 1) {
                _currentLevel += 1;
                Game.Util.Log ("current level: " + _currentLevel);
            }

            level = _levels[_currentLevel];

            //Add new ball to the system
            if (_lastAddTime > _random.Next (level.minAddTime, level.maxAddTime) / 1000.0f) {
                Ball ball = null;
                switch (_random.Next (6)) {
                    default:
                    case 0: ball = new Ball (Ball.Color.Red); break;
                    case 1: ball = new Ball (Ball.Color.Green); break;
                    case 2: ball = new Ball (Ball.Color.Blue); break;
                    case 3: ball = new Ball (Ball.Color.Yellow); break;
                    case 4: ball = new Ball (Ball.Color.Magic); break;
                    case 5: ball = new Ball (Ball.Color.Bomb); break;
                }

                ball.Init ();
                ball.Scale = new Vector2D (0.1f, 0.1f);

                float screenWidth = Game.Instance.ScreenWidth;
                float border = ball.TransformedBoundingBox.Width / 2.0f + Game.Instance.ToLocal (20 * 4, 0).X;
                float newX = _random.Next ((int)((screenWidth - 2.0f * border) * 1000.0f)) / 1000.0f + border;

                ball.Pos = new Vector2D (newX, ball.TransformedBoundingBox.Height / 2.0f);

                RigidBody2D body = new RigidBody2D () {
                    Mesh = ball,
                    Velocity = new Vector2D (0, level.startVelocityY),
                    Mass = 1.0f, //1 kg
                    FindCollision = new RigidBody2D.FindCollisionDelegate (FindCollision)
                };

                _fallingBalls.Add (new FallingBall () {
                    ball = ball,
                    body = body
                });
                _lastAddTime = 0;
            }

            //Calculate physic (Remove balls not on screen)
            for (int i = _fallingBalls.Count - 1; i >= 0; --i) {
                FallingBall item = _fallingBalls[i];
                item.body.Force += new Vector2D (0, 1.0f * RigidBody2D.Gravity);
                item.body.Update (elapsedTime);
                //TODO: itt kezelni kell az ütközéseket... (de ha bombával ütköztünk, akkor végünk!)

                if (item.ball.Pos.Y - item.ball.BoundingBox.Height * item.ball.Scale.Y / 2.0f > Game.Instance.ScreenHeight) {
                    //TODO: itt le kell kezelni, hogy meghalt a játékos...

                    item.ball.Shutdown ();
                    _fallingBalls.RemoveAt (i);
                }
            }

            //Calculate ended ball animation
            for (int i = _endedBalls.Count - 1;i > 0;--i) {
                QTEGoodBall item = _endedBalls[i];
                item.Update (elapsedTime);
                if (item.IsEnded) {
                    _endedBalls.RemoveAt (i);
                    item.Shutdown ();
                }
            }

            //...
        }

        public override void Render () {
            GL.ClearColor (1.0f, 0.5f, 0.5f, 1.0f);
            GL.Clear ((uint)All.ColorBufferBit);

            GL.MatrixMode (All.Modelview);
            GL.LoadIdentity ();

            _background.Render ();

            //Draw falling balls
            foreach (FallingBall item in _fallingBalls)
                item.ball.Render ();

            //Draw dragged balls
            foreach (FallingBall item in _touchedBalls.Values) {
                item.ball.Render ();
            }

            //Draw ended balls
            foreach (QTEGoodBall item in _endedBalls) {
                item.Render ();
            }
        }

        #region Input handlers
        public override void TouchDown (int fingerID, float x, float y) {
            base.TouchDown (fingerID, x, y);

            if (_failBall == null && !_touchedBalls.ContainsKey (fingerID)) {
                for (int i = 0; i < _fallingBalls.Count; ++i) {
                    FallingBall item = _fallingBalls[i];
                    Vector2D pos = Game.Instance.ToLocal (x, y);
                    if (item.ball.TransformedBoundingBox.Contains (pos)) {
                        if (item.ball.Type == Ball.Color.Bomb) {
                            HandleBombBlowEnd (item.ball);
                        } else {
                            item.lastTouch = _fullTime;
                            _touchedBalls.Add (fingerID, item);
                            _fallingBalls.RemoveAt (i);
                        }
                        break;
                    }
                }
            }
        }

        public override void TouchUp (int fingerID, float x, float y) {
            base.TouchUp (fingerID, x, y);

            if (_failBall == null && _touchedBalls.ContainsKey (fingerID)) {
                FallingBall item = _touchedBalls[fingerID];
                RefreshTouchedBall (item, x, y);

                RegionTest test = TestBallInEndRegions (item);
                switch (test) {
                    default:
                    case RegionTest.NotInRegion:
                        _fallingBalls.Add (item);
                        _touchedBalls.Remove (fingerID);
                        break;
                    case RegionTest.GoodRegion:
                        HandleGoodRegionEnd (fingerID, item.ball);
                        break;
                    case RegionTest.WrongRegion:
                        HandleWrongRegionEnd (fingerID, item.ball);
                        break;
                }
            }
        }

        public override void TouchMove (int fingerID, float x, float y) {
            base.TouchMove (fingerID, x, y);

            if (_failBall == null && _touchedBalls.ContainsKey (fingerID)) {
                FallingBall item = _touchedBalls[fingerID];
                RefreshTouchedBall (item, x, y);

                RegionTest test = TestBallInEndRegions (item);
                switch (test) {
                    default:
                    case RegionTest.NotInRegion:
                        break;
                    case RegionTest.GoodRegion:
                        HandleGoodRegionEnd (fingerID, item.ball);
                        break;
                    case RegionTest.WrongRegion:
                        HandleWrongRegionEnd (fingerID, item.ball);
                        break;
                }
            }
        }
        #endregion

        #region Helper methods
        private void RefreshTouchedBall (FallingBall item, float x, float y) {
            float elapsedTime = _fullTime - item.lastTouch;
            item.lastTouch = _fullTime;

            item.body.LastPos = item.ball.Pos;
            item.ball.Pos = Game.Instance.ToLocal (x, y);

            Vector2D dist = item.ball.Pos - item.body.LastPos;
            item.body.Velocity = dist / elapsedTime / RigidBody2D.PhysicalScale;

            //TODO: megcsinalni a dobast...
            //item.body.Force += dist * 100.0f;
        }

        private RigidBody2D FindCollision (RigidBody2D body) {
            //Check dragged balls for collision
            float radius = 0.1f;
            foreach (int fingerID in _touchedBalls.Keys) {
                FallingBall item = _touchedBalls[fingerID];
                if (item.body == body)
                    continue;

                Vector2D dist = item.ball.Pos - body.Mesh.Pos;
                if (dist.Length < radius * 2.0f) {
                    //Game.Util.Log ("collision1 item: " + item.ball.Pos + ", body: " + body.Mesh.Pos + ", dist: " + dist.Length + ", radius: " + radius + ", type: " + item.ball.Type);
                    return item.body;
                }
            }

            //Check other falling balls for collision
            for (int i = 0; i < _fallingBalls.Count; ++i) {
                FallingBall item = _fallingBalls[i];
                if (item.body == body)
                    continue;

                Vector2D dist = item.ball.Pos - body.Mesh.Pos;
                if (dist.Length < radius * 2.0f) {
                    //Game.Util.Log ("collision2 item: " + item.ball.Pos + ", body: " + body.Mesh.Pos + ", dist: " + dist.Length + ", radius: " + radius + ", type: " + item.ball.Type);
                    return item.body;
                }
            }

            return null;
        }

        private RegionTest TestBallInEndRegions (FallingBall item) {
            if (_yellowRegion.Contains (item.ball.Pos)) {
                return item.ball.Type == Ball.Color.Yellow || item.ball.Type == Ball.Color.Magic ? RegionTest.GoodRegion : RegionTest.WrongRegion;
            } else if (_greenRegion.Contains (item.ball.Pos)) {
                return item.ball.Type == Ball.Color.Green || item.ball.Type == Ball.Color.Magic ? RegionTest.GoodRegion : RegionTest.WrongRegion;
            } else if (_redRegion.Contains (item.ball.Pos)) {
                return item.ball.Type == Ball.Color.Red || item.ball.Type == Ball.Color.Magic ? RegionTest.GoodRegion : RegionTest.WrongRegion;
            } else if (_blueRegion.Contains (item.ball.Pos)) {
                return item.ball.Type == Ball.Color.Blue || item.ball.Type == Ball.Color.Magic ? RegionTest.GoodRegion : RegionTest.WrongRegion;
            }

            return RegionTest.NotInRegion;
        }

        private void HandleGoodRegionEnd (int fingerID, Ball ball) {
            QTEGoodBall qte = new QTEGoodBall (ball);
            qte.Init ();
            qte.Start ();
            _endedBalls.Add (qte);
            _touchedBalls.Remove (fingerID);
        }

        private void HandleWrongRegionEnd (int fingerID, Ball ball) {
            //TODO: handle fail of game
        }

        private void HandleBombBlowEnd (Ball ball) {
            //TODO: bomba robbanas kezelese (fail)
        }
        #endregion
    }
}