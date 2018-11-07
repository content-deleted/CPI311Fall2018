using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

using System.IO;
using Squared.Tiled;
using Microsoft.Xna.Framework.Content;

namespace Assignment1 {
    class MainGameplayScreen : GameScreen {

        ContentManager content;

        SpriteBatch spriteBatch;

        SpriteBatch backgrounds;

        // Should think of a better way to store content
        Texture2D playerSpriteSheet;
        Texture2D bulletSprite;
        Texture2D bulletSprite2;
        Texture2D hitBoxSpriteSheet;
        public static Texture2D background;
        public static Texture2D doggo;

        SpriteFont font;
        Effect zoom;

        Map map;

        public RenderTarget2D renderTarget;


        public MainGameplayScreen() {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);


            InputManager.Initialize();
            Time.Initialize();
        }

        public override void LoadContent() {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            // The testing for tilemaps
            map = Map.Load(Path.Combine(content.RootDirectory, "TESTMAP.tmx"), content);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);
            backgrounds = new SpriteBatch(ScreenManager.GraphicsDevice);

            // Setting up render target
            PresentationParameters pp = ScreenManager.GraphicsDevice.PresentationParameters;

            renderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true, SurfaceFormat.Color, DepthFormat.Depth24);

            // Load extra
            font = content.Load<SpriteFont>("font");
            zoom = content.Load<Effect>("zoom");
            background = content.Load<Texture2D>("space");//"p_u_r_p_b_o_y_s"); 
            doggo = content.Load<Texture2D>("DOGGIE");//"p_u_r_p_b_o_y_s"); 


            // Load sprites n stuff
            playerSpriteSheet = content.Load<Texture2D>("explorer");
            hitBoxSpriteSheet = content.Load<Texture2D>("hitbox");
            bulletSprite2 = content.Load<Texture2D>("bullet_2");
            bulletSprite = content.Load<Texture2D>("bullet_1");
            Texture2D mosueSprite = content.Load<Texture2D>("mouse");


            PlayerObject.CreatePlayer(new Vector2(300, 700), playerSpriteSheet, hitBoxSpriteSheet, bulletSprite2, mosueSprite);  // Oof
            PlayerObject.players.First().sprite.Position = new Vector2(1000, 1000);

            GameObject2d spawner = GameObject2d.Initialize();

            spawner.sprite = new Sprite(bulletSprite);
            spawner.sprite.Scale *= 2;
            spawner.sprite.LayerDepth = 0.4f;
            spawner.addBehavior(new grazeEnemy());
            BulletSpawner b = new BulletSpawner();
            spawner.sprite.Position = new Vector2(300, 200);

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

            //init
            foreach (GameObject2d gameObject in GameObject2d.activeGameObjects) gameObject.Start();
        }

        Vector2 b = Vector2.One;
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            InputManager.Update();
            Time.Update(gameTime);

            GameObject2d.UpdateObjects();

            // Just to prove I can
            //map.Layers.First().Value.Opacity = (float)(Math.Cos(Math.PI * (gameTime.TotalGameTime.Milliseconds * 4) / 10000));

            if (InputManager.IsKeyDown(Keys.Up)) { b.X += 0.01f; b.Y += 0.01f; }
            if (InputManager.IsKeyDown(Keys.Down) && b.Length() > 0) { b.X -= 0.01f; b.Y -= 0.01f; }
            if (InputManager.IsKeyPressed(Keys.R)) 
        }

        Vector2 PlayerLoc = Vector2.Zero;
        Vector2 drawLoc = Vector2.Zero;

        public int PreferredBackBufferWidth = 1280;
        public int PreferredBackBufferHeight = 720;

        public override void Draw(GameTime gameTime) {
            // Set the context to our render target
            ScreenManager.GraphicsDevice.SetRenderTarget(renderTarget);
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            backgrounds.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            /*
            offset.Parameters["height"].SetValue(1000f);
            offset.Parameters["offset"].SetValue((float)Time.TotalGameTimeMilli/1000);
            offset.CurrentTechnique.Passes[0].Apply();

            /*
            backgrounds.Draw(background, new Rectangle(0, 0, 600, 1000), Color.White); //new Rectangle(0, 0, playerSpriteSheet.Width, playerSpriteSheet.Height), Color.White,0 , new Vector2 (300,1000), effec)
            */

            // T I L E M A P   L O G I C
            PlayerLoc = PlayerObject.players[0].sprite.Position;


            Vector2 newPos = PlayerLoc - new Vector2(PreferredBackBufferWidth, PreferredBackBufferHeight) / 2;

            if (!(PlayerLoc.X < 0 || PlayerLoc.X > map.Width * map.TileWidth - PreferredBackBufferWidth)) {
                Sprite.cameraPosition.X = newPos.X;
                drawLoc.X = PlayerLoc.X;
            }

            if (!(PlayerLoc.Y < 0 || PlayerLoc.Y > map.Height * map.TileHeight - PreferredBackBufferHeight)) {
                Sprite.cameraPosition.Y = newPos.Y;
                drawLoc.Y = PlayerLoc.Y;
            }
            map.Draw(backgrounds, new Rectangle(0, 0, PreferredBackBufferWidth, PreferredBackBufferHeight), drawLoc);

            backgrounds.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);


            foreach (GameObject2d gameObject in GameObject2d.activeGameObjects.ToList()) gameObject.Render(spriteBatch);

            spriteBatch.DrawString(font, "Graze: " + grazeEnemy.grazeScore, new Vector2(50, 950), Color.White);


            spriteBatch.End();

            ScreenManager.GraphicsDevice.SetRenderTarget(null);

            // Handle post processing effects
            using (SpriteBatch sprite = new SpriteBatch(ScreenManager.GraphicsDevice)) {
                //sprite.Begin();

                zoom.Parameters["zoom"].SetValue(b);

                sprite.Begin(SpriteSortMode.Deferred, null, null, null, null, zoom);

                sprite.Draw(renderTarget, new Rectangle(ScreenManager.GraphicsDevice.Viewport.X, ScreenManager.GraphicsDevice.Viewport.Y, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), Color.White);
                sprite.End();
            }

            base.Draw(gameTime);
        }
    }
}
