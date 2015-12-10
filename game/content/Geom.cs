using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace game.content {
    public static class Geom {
        /// <summary>
        /// Calculate intersection point of a line and a circle.
        /// 
        /// kor egyenlet: (x - u)^2 + (y - v)^2 = r^2, ahol [u,v] a kozeppont, es r a sugar
        /// Egyenes egyenlet: v2 * x - v1 * y = v2 * x0 - v1 * y0, ahol v(v1, v2) az iranyvektor, ill. [x0,y0] a pont, amin atmegy
        ///
        /// 1. egyenlet:
        /// v2 * x - v1 * y = const
        /// y = (v2 * x - const) / v1       , ahol const = v2*x0 - v1*y0
        ///
        /// 2. egyenlet:
        /// (x^2 - 2*x*u + u^2) + (y^2 - 2*y*v + v^2) = r^2
        /// x^2 + y^2 - 2*u*x - 2*v*y = r^2 - u^2 - v^2
        ///
        /// Osszerakva:
        /// x^2 + (v2/v1*x - const/v1))^2 - 2*u*x - 2*v*(v2/v1 * x - const/v1) = r^2 - u^2 - v^2
        /// x^2 + (v2/v1*x)^2 - 2*v2/v1*x * const/v1 + (const/v1)^2 - 2*u*x - 2*v*v2/v1*x + 2*v*const/v1 = r^2 - u^2 - v^2
        ///
        /// Masodfoku egyenlet parameterei:
        /// a = (v2 / v1)^2 + 1
        /// b = -2*v2/v1*const/v1 - 2*u - 2*v2/v1
        /// c = (const/v1)^2 + 2*v*const/v1 - r^2 + u^2 + v^2
        ///
        /// </summary>
        /// <param name="dir">The normalized direction vector of the line.</param>
        /// <param name="pt">On point of the line.</param>
        /// <param name="origin">The origin of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <returns>null, when no intersection found.</returns>
        public static Vector2D[] LineCircleIntersection (Vector2D dir, Vector2D pt, Vector2D origin, float radius) {
            if (dir.X == 0.0f) {
                Vector2D[] res = LineCircleIntersection (new Vector2D (dir.Y, dir.X), new Vector2D (pt.Y, pt.X), new Vector2D(origin.Y, origin.X), radius);
                if (res == null)
                    return null;

                if (res.Length > 0) {
                    float temp = res[0].X;
                    res[0].X = res[0].Y;
                    res[0].Y = temp;
                }

                if (res.Length > 1) {
                    float temp = res[1].X;
                    res[1].X = res[1].Y;
                    res[1].Y = temp;
                }

                return res;
            } 

            float m = dir.Y / dir.X;
            float cl = dir.Y * pt.X - dir.X * pt.Y;
            float a = (float)Math.Pow (m, 2.0f) + 1.0f;
            float b = -2.0f * m * cl / dir.X - 2.0f * origin.X - 2.0f * m;
            float c = (float)Math.Pow (cl / pt.X, 2.0f) + 2 * origin.Y * cl / dir.X - (float)Math.Pow (radius, 2) + (float)Math.Pow (origin.X, 2) + (float)Math.Pow (origin.Y, 2);

            float det = (float)Math.Pow (b, 2) - 4 * a * c;
            if (det < 0) //No intersection point
                return null;

            if (det == 0.0f) { //One intersection point
                Vector2D res = new Vector2D ();
                res.X = -1.0f * b / (2.0f * a);
                res.Y = (dir.Y * res.X - cl) / dir.X;

                return new Vector2D[] { res };
            }

            //Two intersection point
            det = (float)Math.Sqrt (det);

            Vector2D res1 = new Vector2D ();
            res1.X = (-1.0f * b + det) / (2.0f * a);
            res1.Y = (dir.Y * res1.X - cl) / dir.X;

            Vector2D res2 = new Vector2D ();
            res2.X = (-1.0f * b - det) / (2.0f * a);
            res2.Y = (dir.Y * res2.X - cl) / dir.X;

            return new Vector2D[] { res1, res2 };
        }
    }
}