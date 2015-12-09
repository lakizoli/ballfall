using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace game.management {
    public abstract class Scene {
        public abstract void Init (int width, int height);

        public abstract void Shutdown ();

        public abstract void Resize (int oldWidth, int oldHeight, int newWidth, int newHeight);

        public abstract void Update (float elapsedTime);

        public abstract void Render ();

        public virtual void TouchDown (int fingerID, float x, float y) { }

        public virtual void TouchUp (int fingerID, float x, float y) { }

        public virtual void TouchMove (int fingerID, float x, float y) { }
    }
}