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
using Android.Util;

using game;

namespace ballfall.android {
    class AndroidUtil : IUtil {
        public void Log (string log) {
            Android.Util.Log.Debug ("Game", log);
        }
    }
}