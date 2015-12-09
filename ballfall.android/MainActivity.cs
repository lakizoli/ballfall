﻿using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;

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
        GLView1 view;

        protected override void OnCreate (Bundle bundle) {
            base.OnCreate (bundle);

            // Create our OpenGL view, and display it
            view = new GLView1 (this);
            SetContentView (view);
        }

        protected override void OnPause () {
            base.OnPause ();
            view.Pause ();
        }

        protected override void OnResume () {
            base.OnResume ();
            view.Resume ();
        }
    }
}

