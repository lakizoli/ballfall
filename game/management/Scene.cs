using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace game.management {
    public abstract class Scene {
        protected Game _game;

        public Scene (Game game) {
            _game = game;
        }

        public abstract void Init (int width, int height);

        public abstract void Shutdown ();

        public abstract void Update (double elapsedTime);

        public abstract void Render ();

        public virtual void TouchDown (int fingerID, float x, float y) { }

        public virtual void TouchUp (int fingerID, float x, float y) { }

        public virtual void TouchMove (int fingerID, float x, float y) { }
    }
}