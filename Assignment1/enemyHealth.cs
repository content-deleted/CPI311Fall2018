using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CPI311.GameEngine;

namespace Assignment1 {
    class enemyHealth : Behavior2d {
        public float max = 1000;
        public float health = 1000;

        public bool boss = false;

        public static List<enemyHealth> enemies = new List<enemyHealth>();



        public override void Start() {
            health = max;
            enemies.Add(this);
            base.Start();
        }
        public void damage (float value) {
            health -= value;
            if (health <= 0)
                if (!boss) (obj as GameObject2d).Destroy();
                else (obj.GetBehavior<BossController>() as BossController).changeState();
        }
        public override void OnDestory() {
            enemies.Remove(this);
            base.OnDestory();
        }
    }
}
