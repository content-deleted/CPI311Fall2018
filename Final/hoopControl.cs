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

        //public bool activated = false;

        public override void Update() {
            base.Update();

            // first check if we're close
            if (transform.LocalPosition.Z < player.Transform.LocalPosition.Z + 1) {

                // if we're through then activate 
                if(transform.LocalPosition.Z == player.Transform.LocalPosition.Z && Vector3.Distance(transform.LocalPosition, player.Transform.LocalPosition) < 2) {
                    
                }
            
                // if we're behind then destroy
                if (transform.LocalPosition.Z < player.Transform.LocalPosition.Z - 5) {
                    (obj as hoopObject).Destroy();
                }
            }
        }
    }
}
