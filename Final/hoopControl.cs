using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CPI311.GameEngine;
namespace Final {
    public class hoopControl : Behavior3d {
        float r = 0;
        public float rate = 0.1f;

        Hoop hoop;

        public override void Start() {
            base.Start();
            hoop = (((obj) as GameObject3d).material as Hoop);
        }

        public override void Update() {
            base.Update();
            r += rate;
            hoop.rotation = r;
        }

        public override void OnDestory() {
            base.OnDestory();
            hoop = null;
        }
    }
}
