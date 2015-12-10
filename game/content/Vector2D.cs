using System;

namespace game.content {
    public struct Vector2D {
        #region Data

        public float X { get; set; }

        public float Y { get; set; }

        public static Vector2D Zero = new Vector2D (0, 0);

        #endregion

        #region Construction
        public Vector2D (float x, float y) {
            X = x;
            Y = y;
        }

        #endregion

        #region Operations

        public float SquareLength { get { return X * X + Y * Y; } }

        public float Length { get { return (float)Math.Sqrt (SquareLength); } }

        public static Vector2D operator +(Vector2D v1, Vector2D v2) {
            return new Vector2D { X = v1.X + v2.X, Y = v1.Y + v2.Y };
        }

        public static Vector2D operator -(Vector2D v1, Vector2D v2) {
            return new Vector2D { X = v1.X - v2.X, Y = v1.Y - v2.Y };
        }

        public static Vector2D operator *(Vector2D vec, Vector2D multiplier) {
            return new Vector2D { X = vec.X * multiplier.X, Y = vec.Y * multiplier.Y };
        }

        public static Vector2D operator *(Vector2D vec, float multiplier) {
            return new Vector2D { X = vec.X * multiplier, Y = vec.Y * multiplier };
        }

        public static Vector2D operator *(float multiplier, Vector2D vec) {
            return new Vector2D { X = vec.X * multiplier, Y = vec.Y * multiplier };
        }

        public static Vector2D operator /(Vector2D vec, Vector2D divider) {
            return new Vector2D { X = vec.X / divider.X, Y = vec.Y / divider.Y };
        }

        public static Vector2D operator /(Vector2D vec, float divider) {
            return new Vector2D { X = vec.X / divider, Y = vec.Y / divider };
        }

        public static Vector2D operator /(float divider, Vector2D vec) {
            return new Vector2D { X = divider / vec.X, Y = divider / vec.Y };
        }

        public static float Dot (Vector2D v1, Vector2D v2) {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public float Dot (Vector2D vec) {
            return Vector2D.Dot (this, vec);
        }

        public static float Cross (Vector2D v1, Vector2D v2) {
            return v1.X * v2.Y - v1.Y * v2.X;
        }

        public float Cross (Vector2D vec) {
            return Vector2D.Cross (this, vec);
        }

        public static Vector2D Normalize (Vector2D vec) {
            return vec / vec.Length;
        }

        public Vector2D Normalize () {
            return Vector2D.Normalize (this);
        }

        public static bool operator >(Vector2D v1, Vector2D v2) {
            return v1.X > v2.X && v1.Y > v2.Y;
        }

        public static bool operator <(Vector2D v1, Vector2D v2) {
            return v1.X < v2.X && v1.Y < v2.Y;
        }

        public static bool operator >=(Vector2D v1, Vector2D v2) {
            return v1 == v2 || (v1.X > v2.X && v1.Y > v2.Y);
        }

        public static bool operator <=(Vector2D v1, Vector2D v2) {
            return v1 == v2 || (v1.X < v2.X && v1.Y < v2.Y);
        }

        public static bool operator ==(Vector2D v1, Vector2D v2) {
            // Testing trivial cases
            if ((object)v1 == null && (object)v2 == null)
                return true;

            if ((object)v1 == null || (object)v2 == null)
                return false;

            // If they are equal anyway, just return True.
            if (v1.X == v2.X && v1.Y == v2.Y)
                return true;

            // Handle NaN, Infinity.
            if (float.IsInfinity (v1.X) | float.IsNaN (v1.X) | float.IsInfinity (v2.X) | float.IsNaN (v2.X))
                return v1.X.Equals (v2.X);
            else if (float.IsInfinity (v1.Y) | float.IsNaN (v1.Y) | float.IsInfinity (v2.Y) | float.IsNaN (v2.Y))
                return v1.Y.Equals (v2.Y);

            // Handle zero to avoid division by zero
            float divisorX = Math.Max (v1.X, v2.X);
            if (divisorX.Equals (0))
                divisorX = Math.Min (v1.X, v2.X);

            float divisorY = Math.Max (v1.Y, v2.Y);
            if (divisorY.Equals (0))
                divisorY = Math.Min (v1.Y, v2.Y);

            return Math.Abs (v1.X - v2.X) / divisorX <= 0.00001 && Math.Abs (v1.Y - v2.Y) / divisorY <= 0.00001;
        }

        public static bool operator !=(Vector2D v1, Vector2D v2) {
            return !(v1 == v2);
        }

        public override bool Equals (object obj) {
            if (obj == null || !(obj is float))
                return false;

            return this == (Vector2D)obj;
        }

        public override int GetHashCode () {
            return ToString ().GetHashCode ();
        }

        public override string ToString () {
            return "(X: " + X + "; Y: " + Y + ")";
        }
        #endregion
    }
}