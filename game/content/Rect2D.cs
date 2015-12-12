using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game.content {
    public struct Rect2D {
        #region Data

        public Vector2D LeftTop { get; set; }

        public Vector2D RightBottom { get; set; }

        public Vector2D Size { get { return new Vector2D (Width, Height); } }

        public float Width { get { return RightBottom.X - LeftTop.X; } }

        public float Height { get { return RightBottom.Y - LeftTop.Y; } }

        #endregion

        #region Construction

        public Rect2D (float left, float top, float right, float bottom)
            : this (new Vector2D (left, top), new Vector2D (right, bottom)) {
        }

        public Rect2D (Vector2D leftTop, Vector2D rightBottom) {
            LeftTop = leftTop;
            RightBottom = rightBottom;
        }

        #endregion

        #region Operations

        public Rect2D Offset (Vector2D pos) {
            return new Rect2D (LeftTop + pos, RightBottom + pos);
        }

        public Rect2D Scale (Vector2D scale) {
            return new Rect2D (LeftTop * scale, RightBottom * scale);
        }

        public bool Contains (Vector2D vec) {
            return vec >= LeftTop && vec <= RightBottom;
        }

        public bool Intersects (Rect2D rect) {
            return (LeftTop >= rect.LeftTop && LeftTop <= rect.RightBottom) ||
                (RightBottom >= rect.LeftTop && RightBottom <= rect.RightBottom) ||
                (rect.LeftTop >= LeftTop && rect.LeftTop <= RightBottom) ||
                (rect.RightBottom >= LeftTop && rect.RightBottom <= RightBottom);
        }

        public override string ToString () {
            return "(LeftTop: " + LeftTop + ", RightBottom: " + RightBottom + ")";
        }
        #endregion
    }
}
