﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace CPI311.GameEngine {
    public class Rigidbody : Behavior3d {
        public Vector3 Velocity { get; set; }
        public float Mass { get; set; }
        public Vector3 Acceleration { get; set; }
        public Vector3 Impulse { get; set; }

        public float friction = 0.01f;

        public override void Update() {
            Velocity += Acceleration * Time.ElapsedGameTime + Impulse / Mass;
            
            Velocity = Vector3.Min(Velocity, Velocity * friction);
            transform.LocalPosition += Velocity * Time.ElapsedGameTime;
            Impulse = Vector3.Zero;
        }
    }

}
