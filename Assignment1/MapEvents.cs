using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Squared.Tiled;

namespace Assignment1 {
    static class MapEvents {
        public static ContentManager content;
        public static void parseEvents (ObjectGroup events , ContentManager c) {
            content = c;
            foreach(Squared.Tiled.Object obj in events.Objects.Values) {
                switch (obj.Type.ToString()) {
                    case "Enemy":
                        SpawnEnemy(obj);
                        break;
                    default:
                        // Put something here to indicate a load error on map
                        break;
                }
            }
        }

        // First event for enemy spawns this is probably most of what will be here
        static void SpawnEnemy (Squared.Tiled.Object obj) {
            obj.X -= 640;
            obj.Y -= 360;
            switch (obj.Name) {
                case "Sun":
                    spawnSun(obj);
                    break;
                default:
                    // Put something here to indicate a load error on map
                    break;
            }
        }

        #region enemies 
        static void spawnSun (Squared.Tiled.Object obj) {

            GameObject2d spawner = GameObject2d.Initialize();

            //TEMP
            Texture2D bulletSprite = content.Load<Texture2D>("bullet_1");

            spawner.sprite = new Sprite(bulletSprite);
            spawner.sprite.Scale *= 2;
            spawner.sprite.LayerDepth = 0.4f;
            spawner.addBehavior(new grazeEnemy());
            BulletSpawner b = new BulletSpawner();
            spawner.sprite.Position = new Vector2(obj.X, obj.Y);

            b.bulletSpeed = 2;
            b.bulletAmount = 3;

            b.bulletfrequency = 0.05f; // ms 

            b.spin = 0.3f;
            b.Wave = 0.2f;

            b.bulletSprite = bulletSprite;
            b.scale = Vector2.One * 0.4f;
            spawner.addBehavior(b);


            // Bar
            Vector2 barscale = Vector2.One * 0.29f;
            Vector2 barPos = new Vector2(300, 45);

            ProgressBar bar = ProgressBar.Initialize() as ProgressBar;
            bar.sprite = new Sprite(content.Load<Texture2D>("bar"));
            bar.sprite.Position = barPos;
            bar.sprite.Scale = barscale;
            bar.sprite.LayerDepth = 0.01f;
            bar.sprite.enableCam = false;

            bar.innerSprite = new Sprite(content.Load<Texture2D>("inner"));
            bar.innerSprite.Position = barPos;
            bar.innerSprite.Color = Color.DarkRed;
            bar.innerSprite.Scale = barscale;
            bar.innerSprite.enableCam = false;
            bar.value = 100;

            // more boss stuff?
            BossController boss = new BossController();
            boss.healthbar = bar;

            spawner.addBehavior(boss);
            spawner.addBehavior(new enemyHealth());
        }
        #endregion
    }
}
