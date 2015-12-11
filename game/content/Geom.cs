using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace game.content {
    public static class Geom {
        /// <summary>
        /// Calculate intersection point of a line and a circle.
        /// 
        /// Circle equation: (x - u)^2 + (y - v)^2 = r^2, where [u,v] is the origin of the circle, and r is the radius of the circle
        /// Line equation: v2 * x - v1 * y = v2 * x0 - v1 * y0, where [v1, v2] is the normalized direction vector of the line, and [x0,y0] is a point of the line
        /// </summary>
        /// <param name="dir">The normalized direction vector of the line.</param>
        /// <param name="pt">On point of the line.</param>
        /// <param name="origin">The origin of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <returns>null, when no intersection found.</returns>
        public static Vector2D[] LineCircleIntersection (Vector2D dir, Vector2D pt, Vector2D origin, float radius) {
            if (dir.X == 0.0f) {
                Vector2D[] res = LineCircleIntersection (new Vector2D (dir.Y, dir.X), new Vector2D (pt.Y, pt.X), new Vector2D (origin.Y, origin.X), radius);
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

            float t1 = dir.X * dir.X;
            float t3 = dir.X * dir.Y;
            float t6 = dir.Y * dir.Y;
            float t8 = t1 * t1;
            float t9 = origin.Y * origin.Y;
            float t14 = pt.Y * pt.Y;
            float t16 = radius * radius;
            float t19 = t1 * dir.X * dir.Y;
            float t32 = t1 * t6;
            float t33 = origin.X * origin.X;
            float t38 = pt.X * pt.X;
            float t41 = 2 * t19 * origin.X * origin.Y + 2 * t32 * origin.X * pt.X - 2 * t19 * origin.X * pt.Y - 2 * t19 * origin.Y * pt.X + 2 * t8 * origin.Y * pt.Y + 2 * t19 * pt.X * pt.Y - t8 * t14 + t32 * t16 + t8 * t16 - t32 * t33 - t32 * t38 - t8 * t9;
            float t42 = (float)Math.Sqrt (t41);
            float resX1 = (t1 * origin.X + t3 * origin.Y + t6 * pt.X - t3 * pt.Y + t42) / (t1 + t6);
            float resX2 = -(-(t1 * origin.X) - t3 * origin.Y - t6 * pt.X + t3 * pt.Y + t42) / (t1 + t6);

            float resY1 = t6 = (dir.X * pt.Y - dir.Y * pt.X + dir.Y * resX1) / dir.X;
            float resY2 = t6 = (dir.X * pt.Y - dir.Y * pt.X + dir.Y * resX2) / dir.X;

            return new Vector2D[] { new Vector2D (resX1, resY1), new Vector2D (resX2, resY2) };
        }
    }
}