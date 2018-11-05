using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CPI311.GameEngine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment1 {
    class PlayerBullet : Behavior2d {
        float damage = 2;

        float speed;
        Vector2 direction;
        public bool facing = false;

        public void Init(Texture2D spr, float spd, Vector2 dir, Vector2 scale) {
            objSprite = new Sprite(spr);
            objSprite.Scale = scale;
            speed = spd;
            direction = dir;
            if (facing) objSprite.Rotation = (float)Math.Atan2(direction.Y, direction.X);
        }

        override public void Update() {
            objSprite.Position += direction * speed;
            if (facing) objSprite.Rotation = (float)Math.Atan2(direction.Y, direction.X);
            if (objSprite.CameraSpacePosition.X < -100 || objSprite.CameraSpacePosition.X > 1000 || objSprite.CameraSpacePosition.Y < -100 || objSprite.CameraSpacePosition.Y > 1000) {
                (obj as GameObject2d).Destroy();
            }

            // Check collisions 
            foreach (enemyHealth enemy in enemyHealth.enemies.ToList()) {
                if (Sprite.intersects(enemy.objSprite, objSprite)) {
                    enemy.damage(damage);
                    (obj as GameObject2d).Destroy();
                }
             }
        }
    }
}
