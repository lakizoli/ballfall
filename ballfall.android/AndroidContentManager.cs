using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using game;
using Android.Content.Res;
using Android.Graphics;

namespace ballfall.android {
    class AndroidContentManager : IContentManager {
        private Context _context;

        public AndroidContentManager (Context context) {
            _context = context;
        }

        #region IContentManager interface
        public IDisposable LoadImage (string asset) {
            using (var stream = _context.Assets.Open (asset))
                return BitmapFactory.DecodeStream (stream);
        }

        public IntPtr LockPixels (object image) {
            return ((Bitmap)image).LockPixels ();
        }

        public void UnlockPixels (object image) {
            ((Bitmap)image).UnlockPixels ();
        }

        public int GetWidth (object image) {
            return ((Bitmap)image).Width;
        }

        public int GetHeight (object image) {
            return ((Bitmap)image).Height;
        }

        #endregion
    }
}