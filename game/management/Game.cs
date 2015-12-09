using System;
using OpenTK.Graphics.ES11;
using game.content;

//TODO: létre kell hozni a QuickTimeEvent osztályt, ami egy elõre leprogramozott animációt hajt végre a Scene-en (mint pl.: menüpont villogása, ha megnyomták). Jellemzõje, hogy ilyenkor tiltott az input...
//TODO: létre kell hozni az Animation osztályt, amiben egy animáció lehet leírva. pl.: mesh animáció, textúra animáció, szín animáció

//Játék öletek:
//1.) ballfall: leesõ labdák válogatása különbözõ színû csövekbe
//2.) dekázo játék: egy labdát kell dekázni mindenféle szélben, esõben, ill. különbözõ skill-ek gesture függõen stb...
//3.) trojan horse
//4.) varfal védelem
//5.) krakout, traz (C64)
//6.) hunch back (C64)

namespace game.management {
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

    public interface ISystem {
        IUtil Util { get; }

        IContentManager ContentManager { get; }
    } 
    #endregion

    public abstract class Game : ISystem {
        #region Management data
        IUtil _util;
        public IUtil Util { get { return _util; } }

        IContentManager _contentManager;
        public IContentManager ContentManager { get { return _contentManager; } }
        #endregion

        #region Data
        int _width = 0;
        int _height = 0;

        private Scene _currentScene;
        public Scene CurrentScene
        {
            get { return _currentScene; }
            set
            {
                if (_currentScene != null) {
                    _currentScene.Shutdown ();
                }
                if (value != null) {
                    value.Init (_width, _height);
                }
                _currentScene = value;
            }
        }
        #endregion

        #region Construction
        public Game (IUtil util, IContentManager contentManager) {
            _util = util;
            _contentManager = contentManager;
        }
        #endregion

        #region Interface
        public virtual void Init (int width, int height) {
            InitProjection (width, height);
        }

        public virtual void Shutdown () {
            CurrentScene = null;
        }

        public void Resize (int oldWidth, int oldHeight, int newWidth, int newHeight) {
            InitProjection (newWidth, newHeight);
        }

        /// <summary>
        /// The update step of the game.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time from the last update in seconds.</param>
        public virtual void Update (double elapsedTime) {
            if (_currentScene != null)
                _currentScene.Update (elapsedTime);
        }

        public virtual void Render () {
            if (_currentScene != null)
                _currentScene.Render ();
        }
        #endregion

        #region Input handlers
        public virtual void TouchDown (int fingerID, float x, float y) {
            if (_currentScene != null)
                _currentScene.TouchDown (fingerID, x, y);
        }

        public virtual void TouchUp (int fingerID, float x, float y) {
            if (_currentScene != null)
                _currentScene.TouchUp (fingerID, x, y);
        }

        public virtual void TouchMove (int fingerID, float x, float y) {
            if (_currentScene != null)
                _currentScene.TouchMove (fingerID, x, y);
        }
        #endregion

        #region Helper methods
        public Vector2D ToLocal (float x, float y) {
            float min = Math.Min (_width, _height);
            float max = Math.Max (_width, _height);
            float aspect = max / min;
            if (_width <= _height) {
                return new Vector2D (x / (float)_width, aspect * y / (float)_height);
            }

            return new Vector2D (aspect * x / (float)_width, y / (float)_height);
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
        #endregion
    }
}
