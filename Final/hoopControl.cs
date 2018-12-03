using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CPI311.GameEngine;
using Microsoft.Xna.Framework;

namespace Final {
    public class hoopControl : Behavior3d {
        float r = 0;
        public float rate = 0.1f;
        public float rotationRate = 0.1f;

        Hoop hoop;

        public override void Start() {
            base.Start();
            hoop = (((obj) as GameObject3d).material as Hoop);
        }

        public override void Update() {
            base.Update();
            r += rate;
            hoop.rotation = r;
            transform.Rotate(Vector3.Forward, rotationRate);
        }

        public override void OnDestory() {
            hoop = null;
            base.OnDestory();
        }
    }

    public class hoopLogic : Behavior3d {
        public static Camera player;

        public override void Update() {
            base.Update();
            if(transform.LocalPosition.Z < player.Transform.LocalPosition.Z - 5) {
                (obj as hoopObject).Destroy();
            }
        }
    }
}
