using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Graphics;

namespace ballfall.android {
    [Activity (Label = "ballfall.android",
        MainLauncher = true,
        Icon = "@drawable/icon",
        Theme = "@android:style/Theme.DeviceDefault.NoActionBar",
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden
#if __ANDROID_11__
		,HardwareAccelerated=false
#endif
        )]
    public class MainActivity : Activity {
        GLView1 _view;
        TextView _textTopRight;
        TextView _textTopLeft;

        protected override void OnCreate (Bundle bundle) {
            base.OnCreate (bundle);

            // Create our OpenGL view, and display it
            _view = new GLView1 (this);
            SetContentView (_view);

            //Setup game overlays
            RelativeLayout relativeLayout = new RelativeLayout (this);
            RelativeLayout.LayoutParams rlp = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
            relativeLayout.LayoutParameters = rlp;

            _textTopRight = new TextView (this);
            _textTopRight.Text = "";

            _textTopLeft = new TextView (this);
            _textTopLeft.Text = "";

            Typeface tf = Typeface.CreateFromAsset (Application.Context.Assets, "font.ttf");
            _textTopRight.SetTypeface (tf, TypefaceStyle.Normal);
            _textTopLeft.SetTypeface (tf, TypefaceStyle.Normal);

            RelativeLayout.LayoutParams lpTopRight = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            lpTopRight.AddRule (LayoutRules.AlignParentRight);
            lpTopRight.AddRule (LayoutRules.AlignParentTop);

            RelativeLayout.LayoutParams lpTopLeft = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            lpTopLeft.AddRule (LayoutRules.AlignParentLeft);
            lpTopLeft.AddRule (LayoutRules.AlignParentTop);

            _textTopRight.LayoutParameters = lpTopRight;
            _textTopLeft.LayoutParameters = lpTopLeft;

            relativeLayout.AddView (_textTopRight);
            relativeLayout.AddView (_textTopLeft);
            
            AddContentView (relativeLayout, new ViewGroup.LayoutParams (ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
        }

        protected override void OnPause () {
            base.OnPause ();
            _view.Pause ();
        }

        protected override void OnResume () {
            base.OnResume ();
            _view.Resume ();
        }

        public void SetTopLeftStatus (string text) {
            _textTopLeft.Text = text;
        }

        public void SetTopRightStatus (string text) {
            _textTopRight.Text = text;
        }
    }
}

