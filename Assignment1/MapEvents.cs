using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameStateManagement;
using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Squared.Tiled;

namespace Assignment1 {
    static class MapEvents {
        public static ContentManager content;
        public static void parseEvents(ObjectGroup events, ContentManager c) {
            content = c;
            foreach (Squared.Tiled.Object obj in events.Objects.Values) {
                switch (obj.Type.ToString()) {
                    case "Enemy":
                        SpawnEnemy(obj);
                        break;
                    case "Map":
                        SwapMap(obj);
                        break;
                    default:
                        // Put something here to indicate a load error on map
                        break;
                }
            }
        }

        static void fixForScreenSize(Squared.Tiled.Object obj) {
            obj.X -= MainGameplayScreen.PreferredBackBufferWidth / 2;
            obj.Y -= MainGameplayScreen.PreferredBackBufferHeight / 2;
        }

        // First event for enemy spawns this is probably most of what will be here
        static void SpawnEnemy(Squared.Tiled.Object obj) {
            fixForScreenSize(obj);
            switch (obj.Name) {
                case "Sun":
                    spawnSun(obj);
                    break;
                case "RoundCactus":
                    spawnRoundCactus(obj);
                    break;
                default:
                    // Put something here to indicate a load error on map
                    break;
            }
        }

        #region enemies 
        static void spawnSun(Squared.Tiled.Object obj) {

            GameObject2d spawner = GameObject2d.Initialize();

            //TEMP
            Texture2D bulletSprite = content.Load<Texture2D>("bullet_1");

            spawner.sprite = new AnimatedSprite(content.Load<Texture2D>("sunSprite"), 3, 1000, 1000, true, 0);
            spawner.sprite.Scale *= 1;
            spawner.sprite.collisionBox *= 0.85f;
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
            var h = new enemyHealth();
            h.max = 20000;
            spawner.addBehavior(h);
        }

        static void spawnRoundCactus(Squared.Tiled.Object obj) {
            GameObject2d enemy = GameObject2d.Initialize();

            enemy.sprite = new Sprite(content.Load<Texture2D>("cactusRound"));
            //enemy.sprite.Scale *= 1;
            enemy.sprite.LayerDepth = 0.4f;
            enemy.addBehavior(new grazeEnemy());
            enemy.addBehavior(new enemyHealth());
            enemy.addBehavior(new BasicShootEnemy());

            enemy.sprite.Position = new Vector2(obj.X, obj.Y);

            BulletSpawner b = new BulletSpawner();

            b.bulletSpeed = 2;
            b.bulletAmount = 6;

            b.bulletfrequency = 0.5f; // ms 

            b.spin = 0f;

            b.bulletSprite = content.Load<Texture2D>("needleBullet");
            b.facing = true;
            b.scale = Vector2.One * 0.7f;
            enemy.addBehavior(b);

        }
        #endregion

        static void SwapMap(Squared.Tiled.Object obj) {
            fixForScreenSize(obj);

            string mapfilename = obj.Name + ".tmx";
            void switchMap (PlayerObject p) =>  MainGameplayScreen.currentGame.loadMap(mapfilename);

            GameObject3d g = GameObject3d.Initialize();
            g.addBehavior(new eventLocation(switchMap, new Vector2(obj.X, obj.Y), 32));
        }

        public delegate void mapLocalEvent(PlayerObject triggerPlayer);

        public class eventLocation : Behavior3d {
            private Vector2 mapPosition;
            private float triggerRadius;
            private mapLocalEvent mapEvent;

            public eventLocation(mapLocalEvent mapEvent, Vector2 mapPosition, float triggerRadius) {
                this.mapEvent = mapEvent;
                this.mapPosition = mapPosition;
                this.triggerRadius = triggerRadius;
            }

            public override void LateUpdate() {
                foreach(PlayerObject player in PlayerObject.players) {
                    if (Vector2.Distance(player.sprite.Position, mapPosition) < triggerRadius) {
                        mapEvent(player);
                        obj.Destroy();
                        break;
                    }
                }
            }
        }
    }
}
