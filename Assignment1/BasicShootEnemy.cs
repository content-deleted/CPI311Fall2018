using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using CPI311.GameEngine;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment1 {
    class BasicShootEnemy : Behavior2d {

        private Random ran;

        public Texture2D bulletSprite;

        int state = 0;

        public override void Start() {
            base.Start();
            ran = new Random();
        }
        float speed = 1;


        Vector2 direction;
        int timer = 10;
        public override void Update() {
            base.Update();
            timer--;
            if (timer <= 0) {
                timer = ran.Next(10, 40);
                direction = Vector2.Normalize(new Vector2((float)ran.NextDouble() -0.5f, (float)ran.NextDouble() - 0.5f));
            }

            objSprite.move(direction * speed);
        }
    }
}
