using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPI311.GameEngine;
using Microsoft.Xna.Framework;

namespace Assignment3 {
    class SpeedColorEffectController : Behavior3d {
        SpeedAndCollideEffect m;
        Rigidbody r;
        Collider c;

        public override void Start() {
            m = (obj as GameObject3d).material as SpeedAndCollideEffect;
            r = obj.GetBehavior<Rigidbody>() as Rigidbody;
            c = obj.GetBehavior<Collider>() as Collider;
        }

        public override void OnDestory() {
            base.OnDestory();
            m = null;
            r = null;
            c = null;
        }

        public override void Update() {
            float speed = Math.Min(r.Velocity.Length()/2 * r.Velocity.Length()/2 , 1);
            m.diffuseColor = new Vector3(0.5f, 0.5f, speed);
            if (c.collidedThisFrame) m.timeSinceCol = 1;
            else if (m.timeSinceCol > 0) m.timeSinceCol -= 0.05f;
        }
    }
}
