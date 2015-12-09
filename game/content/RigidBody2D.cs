using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace game.content {
    public class RigidBody2D {
        public const float PhysicalScale = 40.0f; //Physical scale [pixel / meter]
        public const float Gravity = 9.81f; // [m/sec2]

        public Vector2D LastPos { get; set; }

        public Vector2D Pos { get; set; }

        public Vector2D Velocity { get; set; }

        public float Mass { get; set; }

        public Vector2D Force { get; set; }

        public void Update (float elapsedTime) {
            //Store last pos
            LastPos = Pos;

            //Calculate new position
            Vector2D accel = Force / Mass / PhysicalScale;
            Vector2D v0 = Velocity;
            Vector2D add = accel * elapsedTime;
            Pos += Velocity * elapsedTime + add / 2.0f;
            Velocity += add;

            //Check collision (and iterate new position, when needed)
            //...
        }
    }
}