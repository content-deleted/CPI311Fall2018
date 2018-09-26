﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using CPI311.GameEngine;
using System.Diagnostics;

namespace Assignment1 {
    class BossController : Behavior2d {
        public ProgressBar healthbar;

        private enemyHealth health;
        private BulletSpawner spawner;
        int state = 0;

        public override void Start() {
            base.Start();
            health = obj.GetBehavior<enemyHealth>() as enemyHealth;
            health.boss = true;
            spawner = obj.GetBehavior<BulletSpawner>() as BulletSpawner;
        }

        public override void Update() {
            base.Update();
            updateHealthbar();

            if (state == 2) {
                spawner.bulletSpeed = (float)Math.Sin(Time.TotalGameTimeMilli * 100) * 10 ;
            }
        }

        public void updateHealthbar() => healthbar.value = health.health / health.max;

        public void changeState() {
            state++;

            updateHealthbar();

            switch (state) {
                case (1):
                    health.health = health.max;
                    healthbar.innerSprite.Color = Color.ForestGreen;
                    // spawn pattern

                    spawner.bulletSpeed = 4;
                    spawner.bulletAmount = 6;

                    spawner.bulletfrequency = 5; // ms 

                    spawner.spin = 1.9f;
                    spawner.wave = 0;

                    spawner.bulletTint = Color.ForestGreen;
                    //spawner.bulletSprite = bulletSprite;
                    spawner.scale = Vector2.One * 0.55f;

                    break;

                case (2):
                    health.health = health.max;
                    healthbar.innerSprite.Color = Color.DarkBlue;

                    spawner.bulletAmount = 15;
                    spawner.spin = 0.4f;
                    spawner.scale = Vector2.One * 0.4f;
                    spawner.bulletTint = Color.DarkBlue;
                    break;

                case (3):
                    MainGame.background = MainGame.doggo;
                    enemyHealth.enemies.Remove(health);
                    (obj as GameObject2d).Destroy();
                    break;

            }

        }
    }
}
