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
            public Vector2D touchPosition;
            public List<Vector2D> lastVelocities;
        }

        struct LevelDefinition {
            public int index; //The index of the level
            public float endTime; //The absolute end time of the level [sec]
            public int minAddTime; //minimal time of next add time range [millisec]
            public int maxAddTime; //maximal time of next add time range [millisec]
            public float startVelocityY;
            public int maxAddCount;
        }

        enum RegionTest {
            NotInRegion,
            GoodRegion,
            WrongRegion
        }

        enum State {
            PreGame,
            Game,
            FallError,
        }
        #endregion

        #region Data
        List<LevelDefinition> _levels;
        int _currentLevel;
        int _score;
        int _lastMins;
        int _lastSecs;

        Random _random;
        float _fullTime;
        float _lastAddTime;

        Rect2D _yellowRegion;
        Rect2D _greenRegion;
        Rect2D _redRegion;
        Rect2D _blueRegion;

        QTEGrowText _ready;
        QTEGrowText _steady;
        QTEGrowText _go;

        List<FallingBall> _fallingBalls;
        List<QTEGoodBall> _endedBalls;
        List<QTEGoodBall> _wrongBalls;
        List<QTEExplodeBall> _explodedBalls;
        ImageMesh _background;
        QTEGrowText _fail;
        State _state;

        Dictionary<int, FallingBall> _touchedBalls;
        #endregion

        public FallScene () {
        }

        public override void Init (int width, int height) {
            _state = State.PreGame;

            _levels = new List<LevelDefinition> ();
            _levels.Add (new LevelDefinition () { index = 0, endTime = 30, minAddTime = 1000, maxAddTime = 2000, startVelocityY = 0.0f, maxAddCount = 1 });
            _levels.Add (new LevelDefinition () { index = 1, endTime = 60, minAddTime = 850, maxAddTime = 1500, startVelocityY = 0.5f, maxAddCount = 1 });
            _levels.Add (new LevelDefinition () { index = 2, endTime = 90, minAddTime = 750, maxAddTime = 1200, startVelocityY = 1.0f, maxAddCount = 1 });
            _levels.Add (new LevelDefinition () { index = 3, endTime = 120, minAddTime = 750, maxAddTime = 1200, startVelocityY = 1.0f, maxAddCount = 2 });

            _currentLevel = 0;
            _score = 0;
            _lastMins = 0;
            _lastSecs = 0;

            _random = new Random ();
            _fullTime = 0;
            _lastAddTime = 0;

            _yellowRegion = new Rect2D (Game.Instance.ToLocal (0, 65 * 4), Game.Instance.ToLocal (20 * 4, 265 * 4));
            _greenRegion = new Rect2D (Game.Instance.ToLocal (0, 265 * 4), Game.Instance.ToLocal (20 * 4, 480 * 4));
            _redRegion = new Rect2D (Game.Instance.ToLocal (width - 20 * 4, 65 * 4), Game.Instance.ToLocal (width, 265 * 4));
            _blueRegion = new Rect2D (Game.Instance.ToLocal (width - 20 * 4, 265 * 4), Game.Instance.ToLocal (width, 480 * 4));

            _ready = new QTEGrowText ("ready.png", 0.5f);
            _ready.Init ();
            _ready.Start ();

            _steady = null;
            _go = null;

            _fallingBalls = new List<FallingBall> ();
            _endedBalls = new List<QTEGoodBall> ();
            _wrongBalls = new List<QTEGoodBall> ();
            _explodedBalls = new List<QTEExplodeBall> ();

            _background = new ImageMesh ("background.png");
            _background.Init ();

            Vector2D screenSize = Game.Instance.ToLocal (width, height);
            _background.Pos = screenSize / 2.0f;
            _background.Scale = screenSize / _background.BoundingBox.Size;

            _touchedBalls = new Dictionary<int, FallingBall> ();

            Game.ContentManager.SetTopLeftStyle (20, 1, 0, 0, 1);
            Game.ContentManager.SetTopRightStyle (20, 1, 0, 0, 1);

            RefreshOverlays (true);
        }

        public override void Shutdown () {
            _touchedBalls = null;

            if (_go != null) {
                _go.Stop ();
                _go.Shutdown ();
            }

            if (_steady != null) {
                _steady.Stop ();
                _steady.Shutdown ();
            }

            if (_ready != null) {
                _ready.Stop ();
                _ready.Shutdown ();
            }

            if (_fail != null) {
                _fail.Stop ();
                _fail.Shutdown ();
            }

            _background.Shutdown ();

            foreach (QTEExplodeBall item in _explodedBalls) {
                item.Stop ();
                item.Shutdown ();
            }
            _explodedBalls.Clear ();

            foreach (QTEGoodBall item in _endedBalls) {
                item.Stop ();
                item.Shutdown ();
            }
            _endedBalls = null;

            foreach (QTEGoodBall item in _wrongBalls) {
                item.Stop ();
                item.Shutdown ();
            }
            _wrongBalls = null;

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

            //Handle pre game effects
            if (_state == State.PreGame) {
                if (_ready != null) {
                    _ready.Update (elapsedTime);
                    if (_ready.IsEnded) {
                        _fullTime = 0;
                        _lastAddTime = 0;

                        _ready.Shutdown ();
                        _ready = null;

                        _steady = new QTEGrowText ("steady.png", 0.5f);
                        _steady.Init ();
                        _steady.Start ();
                    }
                }

                if (_steady != null) {
                    _steady.Update (elapsedTime);
                    if (_steady.IsEnded) {
                        _fullTime = 0;
                        _lastAddTime = 0;

                        _steady.Shutdown ();
                        _steady = null;

                        _go = new QTEGrowText ("go.png", 0.5f);
                        _go.Init ();
                        _go.Start ();
                    }
                }

                if (_go != null) {
                    _go.Update (elapsedTime);
                    if (_go.IsEnded) {
                        _fullTime = 0;
                        _lastAddTime = 0;

                        _go.Shutdown ();
                        _go = null;

                        _state = State.Game;
                    }
                }

                return;
            }

            //Refresh overlay texts
            RefreshOverlays (false);

            //Pick next level
            LevelDefinition level = _levels[_currentLevel];

            if (_fullTime > level.endTime && _currentLevel < _levels.Count - 1) {
                _currentLevel += 1;
                RefreshOverlays (true);
            }

            level = _levels[_currentLevel];

            //Add new ball to the system
            AddNewBalls (level);

            //Calculate physic (Remove balls not on screen)
            for (int i = _fallingBalls.Count - 1; i >= 0; --i) {
                FallingBall item = _fallingBalls[i];
                item.body.Force += new Vector2D (0, 1.0f * RigidBody2D.Gravity);
                item.body.Update (elapsedTime);

                if (item.body.CollideBody != null) { //If this item collide with other item
                    Ball collideBall = item.body.CollideBody.Mesh as Ball;
                    if (collideBall != null && collideBall.Type == Ball.Color.Bomb) { //Handle collision with bomb
                        _fallingBalls.RemoveAt (i);
                        HandleBombBlowEnd (collideBall);
                        continue;
                    } else if (item.ball.Type == Ball.Color.Bomb) { //Handle collision with bomb
                        _fallingBalls.RemoveAt (i);
                        HandleBombBlowEnd (item.ball);
                        continue;
                    }
                }

                RegionTest test = TestBallInEndRegions (item);
                switch (test) {
                    default:
                    case RegionTest.NotInRegion:
                        break;
                    case RegionTest.GoodRegion:
                        _fallingBalls.RemoveAt (i);
                        HandleGoodRegionEnd (item.ball);
                        continue;
                    case RegionTest.WrongRegion:
                        _fallingBalls.RemoveAt (i);
                        HandleWrongRegionEnd (item.ball);
                        continue;
                }

                //Test fall off from screen
                float yTest = item.ball.Pos.Y - item.ball.BoundingBox.Height * item.ball.Scale.Y / 2.0f;
                if (yTest < 0 || yTest > Game.Instance.ScreenHeight) { //When ball has fallen
                    if (item.ball.Type == Ball.Color.Bomb) {
                        _fallingBalls.RemoveAt (i);
                        item.ball.Shutdown ();
                    } else { //Fail
                        float xTest = item.ball.Pos.X - item.ball.BoundingBox.Width * item.ball.Scale.X / 2.0f;
                        if (xTest >= 0 && xTest <= Game.Instance.ScreenWidth)
                            HandleBallFallFailEnd ();
                        else { //Ball has fallen, but not fail (hack)
                            _fallingBalls.RemoveAt (i);
                            item.ball.Shutdown ();
                        }
                        break;
                    }
                }
            }

            //Calculate ended ball animation
            for (int i = _endedBalls.Count - 1;i >= 0;--i) {
                QTEGoodBall item = _endedBalls[i];
                item.Update (elapsedTime);
                if (item.IsEnded) {
                    _endedBalls.RemoveAt (i);
                    item.Stop ();
                    item.Shutdown ();
                }
            }

            //Calculate wrong ball animation
            for (int i = _wrongBalls.Count - 1; i >= 0; --i) {
                QTEGoodBall item = _wrongBalls[i];
                item.Update (elapsedTime);
                if (item.IsEnded) {
                    _wrongBalls.RemoveAt (i);
                    item.Stop ();
                    item.Shutdown ();

                    HandleBallFallFailEnd ();
                }
            }

            //Calculate exploded ball animation
            for (int i = _explodedBalls.Count - 1; i >= 0; --i) {
                QTEExplodeBall item = _explodedBalls[i];
                item.Update (elapsedTime);
                if (item.IsEnded) {
                    _explodedBalls.RemoveAt (i);
                    item.Stop ();
                    item.Shutdown ();

                    HandleBallFallFailEnd ();
                }
            }

            if (_fail != null)
                _fail.Update (elapsedTime);

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
            foreach (FallingBall item in _touchedBalls.Values)
                item.ball.Render ();

            //Draw ended balls
            foreach (QTEGoodBall item in _endedBalls)
                item.Render ();

            //Draw wrong balls
            foreach (QTEGoodBall item in _wrongBalls)
                item.Render ();

            //Draw exploded balls
            foreach (QTEExplodeBall item in _explodedBalls)
                item.Render ();

            //QTE texts
            if (_ready != null)
                _ready.Render ();

            if (_steady != null)
                _steady.Render ();

            if (_go != null)
                _go.Render ();

            if (_fail != null)
                _fail.Render ();
        }

        #region Input handlers
        public override void TouchDown (int fingerID, float x, float y) {
            base.TouchDown (fingerID, x, y);

            switch (_state) {
                case State.Game:
                    if (!_touchedBalls.ContainsKey (fingerID)) {
                        for (int i = 0; i < _fallingBalls.Count; ++i) {
                            FallingBall item = _fallingBalls[i];
                            Vector2D pos = Game.Instance.ToLocal (x, y);

                            Rect2D testRect = item.ball.TransformedBoundingBox.Scale (new Vector2D (1.1f, 1.1f));
                            testRect.LeftTop -= new Vector2D (0, 0.15f);
                            if (testRect.Contains (pos)) {
                                if (item.ball.Type == Ball.Color.Bomb) {
                                    _fallingBalls.RemoveAt (i);
                                    HandleBombBlowEnd (item.ball);
                                } else {
                                    item.lastTouch = _fullTime;
                                    item.touchPosition = pos;
                                    item.lastVelocities = new List<Vector2D> ();
                                    _touchedBalls.Add (fingerID, item);
                                    _fallingBalls.RemoveAt (i);
                                }
                                break;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override void TouchUp (int fingerID, float x, float y) {
            base.TouchUp (fingerID, x, y);

            switch (_state) {
                case State.Game:
                    if (_touchedBalls.ContainsKey (fingerID)) {
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
                                _touchedBalls.Remove (fingerID);
                                HandleGoodRegionEnd (item.ball);
                                break;
                            case RegionTest.WrongRegion:
                                HandleWrongRegionEnd (item.ball);
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override void TouchMove (int fingerID, float x, float y) {
            base.TouchMove (fingerID, x, y);

            switch (_state) {
                case State.Game:
                    if (_touchedBalls.ContainsKey (fingerID)) {
                        FallingBall item = _touchedBalls[fingerID];
                        RefreshTouchedBall (item, x, y);

                        RegionTest test = TestBallInEndRegions (item);
                        switch (test) {
                            default:
                            case RegionTest.NotInRegion:
                                break;
                            case RegionTest.GoodRegion:
                                _touchedBalls.Remove (fingerID);
                                HandleGoodRegionEnd (item.ball);
                                break;
                            case RegionTest.WrongRegion:
                                HandleWrongRegionEnd (item.ball);
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Helper methods
        private void AddNewBalls (LevelDefinition level) {
            if (_state == State.Game && _lastAddTime > _random.Next (level.minAddTime, level.maxAddTime) / 1000.0f) {
                int addCount = 1; //_random.Next (1, level.maxAddCount + 1);
                for (int i = 0; i < addCount; ++i) {
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

                    float screenWidth = Game.Instance.ScreenWidth / (float)addCount;
                    float screenOffset = i * Game.Instance.ScreenWidth / (float)addCount;
                    float border = ball.TransformedBoundingBox.Width / 2.0f + Game.Instance.ToLocal (20 * 4, 0).X;
                    float newX = _random.Next ((int)(screenOffset * 1000.0f), (int)((screenOffset + screenWidth - 2.0f * border) * 1000.0f)) / 1000.0f + border;

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
            }
        }

        private void RefreshTouchedBall (FallingBall item, float x, float y) {
            Vector2D curPos = Game.Instance.ToLocal (x, y);
            if (item.ball.Pos != curPos) {
                item.body.LastPos = item.ball.Pos;
                item.ball.Pos = curPos;

                float elapsedTime = _fullTime - item.lastTouch;
                item.lastTouch = _fullTime;

                Vector2D dist = item.ball.Pos - item.body.LastPos;
                Vector2D velocity = dist / elapsedTime * RigidBody2D.PhysicalScale;
                item.lastVelocities.Add (velocity);

                while (item.lastVelocities.Count > 3)
                    item.lastVelocities.RemoveAt (0);

                float touchDist = (item.ball.Pos - item.touchPosition).Length;
                if (touchDist > 0.05f) { //Prevent the single touch to speed up the touched ball
                    Vector2D avgVel = Vector2D.Zero;
                    foreach (var vel in item.lastVelocities)
                        avgVel += vel;
                    avgVel /= (float)item.lastVelocities.Count;

                    item.body.Velocity = avgVel;

                    if (item.body.Velocity.Length > 3.0f) {
                        item.body.Velocity = item.body.Velocity.Normalize () * 3.0f;
                    }
                }
            }
        }

        private RigidBody2D FindCollision (RigidBody2D body) {
            //Check dragged balls for collision
            float radius = 0.1f;
            foreach (int fingerID in _touchedBalls.Keys) {
                FallingBall item = _touchedBalls[fingerID];
                if (item.body == body)
                    continue;

                Vector2D dist = item.ball.Pos - body.Mesh.Pos;
                if (dist.Length < radius * 2.0f)
                    return item.body;
            }

            //Check other falling balls for collision
            for (int i = 0; i < _fallingBalls.Count; ++i) {
                FallingBall item = _fallingBalls[i];
                if (item.body == body)
                    continue;

                Vector2D dist = item.ball.Pos - body.Mesh.Pos;
                if (dist.Length < radius * 2.0f)
                    return item.body;
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

        private void HandleGoodRegionEnd (Ball ball) {
            QTEGoodBall qte = new QTEGoodBall (ball);
            qte.Init ();
            qte.Start ();
            _endedBalls.Add (qte);

            ++_score;
            RefreshOverlays (true);
        }

        private void HandleWrongRegionEnd (Ball ball) {
            QTEGoodBall qte = new QTEGoodBall (ball);
            qte.Init ();
            qte.Start ();
            _wrongBalls.Add (qte);
        }

        private void HandleBombBlowEnd (Ball ball) {
            QTEExplodeBall qte = new QTEExplodeBall (ball);
            qte.Init ();
            qte.Start ();
            _explodedBalls.Add (qte);
        }

        private void HandleBallFallFailEnd () {
            _state = State.FallError;

            foreach (FallingBall item in _fallingBalls)
                item.ball.Shutdown ();
            _fallingBalls.Clear ();

            foreach (FallingBall item in _touchedBalls.Values)
                item.ball.Shutdown ();
            _touchedBalls.Clear ();

            _fail = new QTEGrowText ("fail.png", 0.1f);
            _fail.Init ();
            _fail.Start ();
        }

        private void RefreshOverlays (bool force) {
            int mins = (int)(_fullTime / 60.0f);
            int secs = (int)(_fullTime % 60.0f);
            if (force || mins != _lastMins || secs != _lastSecs) {
                _lastMins = mins;
                _lastSecs = secs;

                Game.ContentManager.SetTopLeftStatus ("Level: " + (_currentLevel + 1) + " Time: " + mins.ToString ().PadLeft (2, '0') + ":" + secs.ToString ().PadLeft (2, '0'));
                Game.ContentManager.SetTopRightStatus ("Score: " + _score);
            }
        }
        #endregion
    }
}