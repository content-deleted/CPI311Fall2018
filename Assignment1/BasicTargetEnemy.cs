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
    class BasicTargetEnemy : Behavior2d {

        private enemyHealth health;
        private BulletSpawner spawner;
        private Random ran;

        public Texture2D bulletSprite;

        int state = 0;

        public override void Start() {
            base.Start();
            ran = new Random();
            health = obj.GetBehavior<enemyHealth>() as enemyHealth;
            spawner = obj.GetBehavior<BulletSpawner>() as BulletSpawner;
            spawner.scale = Vector2.One * 0.5f;
            spawner.bulletSpeed = 2;
            spawner.bulletSprite = bulletSprite;
        }
        float speed = 4;


        Vector2 direction;
        int timer= 10;
        public override void Update() {
            base.Update();
            timer--;
            if (timer <= 0) {
                timer = ran.Next(10, 40);
                direction = new Vector2((float)ran.NextDouble(), (float)ran.NextDouble());
            }

            objSprite.move(  direction * speed );
        }
    }
}
