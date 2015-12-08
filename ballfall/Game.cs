using System;
using OpenTK.Graphics.ES11;
using System.Collections.Generic;

//TODO: l�tre kell hozni a game.dll-t, amiben a konkr�t j�t�kokt�l f�ggetlen logika van... (majd a v�g�n r��r�nk...)
//TODO: l�tre kell hozni a Scene (jelenet) oszt�lyt, ami egy komplett jelenetet foglal mag�ba (like MiniGame), resource-ostul, m�k�d�sest�l
//TODO: l�tre kell hozni a QuickTimeEvent oszt�lyt, ami egy el�re leprogramozott anim�ci�t hajt v�gre a Scene-en (mint pl.: men�pont villog�sa, ha megnyomt�k). Jellemz�je, hogy ilyenkor tiltott az input...
//TODO: l�tre kell hozni az Animation oszt�lyt, amiben egy anim�ci� lehet le�rva. pl.: mesh anim�ci�, text�ra anim�ci�, sz�n anim�ci�

//J�t�k �letek:
//1.) ballfall: lees� labd�k v�logat�sa k�l�nb�z� sz�n� cs�vekbe
//2.) dek�zo j�t�k: egy labd�t kell dek�zni mindenf�le sz�lben, es�ben, ill. k�l�nb�z� skill-ek gesture f�gg�en stb...
//3.) trojan horse
//4.) varfal v�delem
//5.) krakout, traz (C64)
//6.) hunch back (C64)

namespace ballfall {
    #region OS specific interfaces
    public interface IContentManager {
        IDisposable LoadImage (string asset);

        IntPtr LockPixels (object image);

        void UnlockPixels (object image);

        int GetWidth (object image);

        int GetHeight (object image);
    }

    public interface IUtil {
        void Log (string log);
    }

    internal interface ISystem {
        IUtil Util { get; }

        IContentManager ContentManager { get; }
    } 
    #endregion

    public class Game : ISystem {
        #region Management data
        IUtil _util;
        public IUtil Util { get { return _util; } }

        IContentManager _contentManager;
        public IContentManager ContentManager { get { return _contentManager; } }
        #endregion

        #region Static data
        int _width = 0;
        int _height = 0;

        Ball _ballRed;
        Ball _ballGreen;
        Ball _ballBlue;
        Ball _ballMagic;
        #endregion

        #region Dynamic data
        Dictionary<int, Vector2D> _touchPositions = new Dictionary<int, Vector2D> ();
        #endregion

        #region Construction
        public Game (IUtil util, IContentManager contentManager) {
            _util = util;
            _contentManager = contentManager;

            _ballRed = new Ball (Ball.Color.Red);
            _ballGreen = new Ball (Ball.Color.Green);
            _ballBlue = new Ball (Ball.Color.Blue);
            _ballMagic = new Ball (Ball.Color.Magic);
        }
        #endregion

        #region Interface
        public void Init (int width, int height) {
            InitProjection (width, height);

            _ballRed.Init (this);
            _ballGreen.Init (this);
            _ballBlue.Init (this);
            _ballMagic.Init (this);

            _ballRed.Scale = new Vector2D (0.1f, 0.1f);
            _ballGreen.Scale = new Vector2D (0.1f, 0.1f);
            _ballBlue.Scale = new Vector2D (0.1f, 0.1f);
            _ballMagic.Scale = new Vector2D (0.1f, 0.1f);
        }

        public void Shutdown () {
            _ballMagic.Shutdown (this);
            _ballBlue.Shutdown (this);
            _ballGreen.Shutdown (this);
            _ballRed.Shutdown (this);
        }

        public void Resize (int oldWidth, int oldHeight, int newWidth, int newHeight) {
            InitProjection (newWidth, newHeight);
        }

        /// <summary>
        /// The update step of the game.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time from the last update in seconds.</param>
        public void Update (double elapsedTime) {
            //...
        }

        public void Render () {
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
        public void TouchDown (int fingerID, float x, float y) {
            if (!_touchPositions.ContainsKey (fingerID)) {
                _touchPositions.Add (fingerID, ToLocal (x, y));
            }
        }

        public void TouchUp (int fingerID, float x, float y) {
            if (_touchPositions.ContainsKey (fingerID)) {
                _touchPositions.Remove (fingerID);
            }
        }

        public void TouchMove (int fingerID, float x, float y) {
            if (_touchPositions.ContainsKey (fingerID)) {
                _touchPositions[fingerID] = ToLocal (x, y);
            }
        }
        #endregion

        #region Inner methods
        private void InitProjection (int width, int height) {
            _width = width;
            _height = height;

            GL.MatrixMode (All.Projection);
            GL.LoadIdentity ();

            float min = Math.Min (width, height);
            float max = Math.Max (width, height);
            float aspect = max / min;
            if (width <= height) {
                GL.Ortho (0, 1.0f, aspect, 0, -1.0f, 1.0f);
            } else {
                GL.Ortho (0, aspect, 1.0f, 0, -1.0f, 1.0f);
            }
        }

        private Vector2D ToLocal (float x, float y) {
            float min = Math.Min (_width, _height);
            float max = Math.Max (_width, _height);
            float aspect = max / min;
            if (_width <= _height) {
                return new Vector2D (x / (float)_width, aspect * y / (float)_height);
            }

            return new Vector2D (aspect * x / (float)_width, y / (float)_height);
        }

        #endregion
    }
}
