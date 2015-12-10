using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace game.content {
    public class RigidBody2D {
        public delegate RigidBody2D FindCollisionDelegate (RigidBody2D body);

        public const float PhysicalScale = 30.0f; //Physical scale [pixel / meter]
        public const float Gravity = 9.81f; // [m/sec2]

        public Mesh2D Mesh { get; set; }

        public Vector2D LastPos { get; set; }

        public Vector2D Velocity { get; set; }

        public float Mass { get; set; }

        public Vector2D Force { get; set; }

        public FindCollisionDelegate FindCollision { get; set; }

        public void Update (float elapsedTime) {
            //Store last pos
            LastPos = Mesh.Pos;

            //Calculate new position
            Vector2D accel = Force / Mass / PhysicalScale;
            Vector2D add = accel * elapsedTime;
            Mesh.Pos += Velocity * elapsedTime + add / 2.0f;
            Velocity += add;

            //Check collision (and iterate new position, when needed)
            if (FindCollision != null) {
                RigidBody2D coll = FindCollision (this);
                if (coll != null) {
                    //Find collision position (circle intersection with velocity direction line)
                    float radius = Mesh.TransformedBoundingBox.Width / 2.0f + coll.Mesh.TransformedBoundingBox.Width / 2.0f; //Most egyszerusitunk, mert minden mesh kor...
                    Vector2D[] intersection = Geom.LineCircleIntersection (Velocity.Normalize (), Mesh.Pos, coll.Mesh.Pos, radius);

                    //Calculate elapsed time before collision

                    //Calculate new velocity and position after collision (until remaining time)
                    Vector2D dist = coll.Mesh.Pos - Mesh.Pos;
                    Vector2D norm = dist.Normalize ();
                    Vector2D proj = Velocity.Dot (norm) * norm;
                    Velocity = (2.0f * (Velocity - proj)) - Velocity;

                    //TODO: itt rendesen ki kell szamolni az utkozesi poziciot, mert most bele tud ragadni a golyo a masikba, mert az uj pozicio is utkozesi pozicioban van...
                    Mesh.Pos += Velocity * elapsedTime;
                }
            }

            //Clear all forces
            Force = Vector2D.Zero;
        }
    }
}