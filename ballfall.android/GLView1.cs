using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES11;
using OpenTK.Platform;
using OpenTK.Platform.Android;
using Android.Views;
using Android.Content;
using Android.Util;
using System.Collections.Generic;

namespace ballfall.android {
    class GLView1 : AndroidGameView {
        private Game _game = new Game ();
        private Size _viewSize = null;
        private HashSet<int> _pointerIDs = new HashSet<int> ();

        public GLView1 (Context context) : base (context) {
        }

        // This gets called when the drawing surface is ready
        protected override void OnLoad (EventArgs e) {
            base.OnLoad (e);

            // Init platform specific stuff
            _viewSize = new Android.Util.Size (Size.Width, Size.Height);

            // Init game's content
            _game.Init (Size.Width, Size.Height);

            // Run the render loop
            Run ();
        }

        // This method is called everytime the context needs
        // to be recreated. Use it to set any egl-specific settings
        // prior to context creation
        //
        // In this particular case, we demonstrate how to set
        // the graphics mode and fallback in case the device doesn't
        // support the defaults
        protected override void CreateFrameBuffer () {
            // the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
            try {
                Log.Verbose ("GLCube", "Loading with default settings");

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer ();
                return;
            } catch (Exception ex) {
                Log.Verbose ("GLCube", "{0}", ex);
            }

            // this is a graphics setting that sets everything to the lowest mode possible so
            // the device returns a reliable graphics setting.
            try {
                Log.Verbose ("GLCube", "Loading with custom Android settings (low mode)");
                GraphicsMode = new AndroidGraphicsMode (0, 0, 0, 0, 0, false);

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer ();
                return;
            } catch (Exception ex) {
                Log.Verbose ("GLCube", "{0}", ex);
            }
            throw new Exception ("Can't load egl, aborting");
        }

        protected override void OnUnload (EventArgs e) {
            // Release game's content
            _game.Shutdown ();

            base.OnUnload (e);
        }

        protected override void OnResize (EventArgs e) {
            base.OnResize (e);

            if (_viewSize.Width != Size.Width || _viewSize.Height != Size.Height) {
                _game.Resize (_viewSize.Width, _viewSize.Height, Size.Width, Size.Height);
                _viewSize = new Android.Util.Size (Size.Width, Size.Height);
            }
        }

        protected override void OnUpdateFrame (FrameEventArgs e) {
            base.OnUpdateFrame (e);

            _game.Update (e.Time);
        }

        // This gets called on each frame render
        protected override void OnRenderFrame (FrameEventArgs e) {
            base.OnRenderFrame (e);

            _game.Render ();

            SwapBuffers ();
        }

        public override bool OnTouchEvent (MotionEvent e) {
            if (e.Action == MotionEventActions.Down || ((int)e.Action & (int)MotionEventActions.PointerDown) == (int)MotionEventActions.PointerDown) {
                int id = e.GetPointerId (e.ActionIndex);
                if (!_pointerIDs.Contains (id)) {
                    int index = e.FindPointerIndex (id);
                    //Log.Debug ("OnTouchEvent", "Down -> idx: " + index + ", id: " + id + ", x: " + e.GetX (index) + ", y: " + e.GetY (index));
                    _pointerIDs.Add (id);
                    _game.TouchDown (id, e.GetX (index), e.GetY (index));
                }
            } else if (e.Action == MotionEventActions.Up || (((int)e.Action & (int)MotionEventActions.PointerUp) == (int)MotionEventActions.PointerUp)) {
                int id = e.GetPointerId (e.ActionIndex);
                if (_pointerIDs.Contains (id)) {
                    int index = e.FindPointerIndex (id);
                    //Log.Debug ("OnTouchEvent", "Up -> idx: " + index + ", id: " + id + ", x: " + e.GetX (index) + ", y: " + e.GetY (index));
                    _pointerIDs.Remove (id);
                    _game.TouchUp (id, e.GetX (index), e.GetY (index));
                }
            } else if (e.Action == MotionEventActions.Move) {
                int id = e.GetPointerId (e.ActionIndex);
                if (_pointerIDs.Contains (id)) {
                    int index = e.FindPointerIndex (id);
                    //Log.Debug ("OnTouchEvent", "Move -> idx: " + index + ", id: " + id + ", x: " + e.GetX (index) + ", y: " + e.GetY (index));
                    _game.TouchMove (id, e.GetX (index), e.GetY (index));
                }
            }

            return true; // base.OnTouchEvent (e);
        }
    }
}
