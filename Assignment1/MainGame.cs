using System.Collections.Generic;
using System.Linq;
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using System.Diagnostics;
using System.IO;
using Squared.Tiled;

namespace Assignment1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
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

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            InputManager.Initialize();
            Time.Initialize();

            graphics.PreferredBackBufferWidth = 600;  
            graphics.PreferredBackBufferHeight = 1000;   
            graphics.ApplyChanges();


            //foreach (GameObject2d gameObject in GameObject2d.activeGameObjects) gameObject.Start();

            base.Initialize();
        }

        protected override void LoadContent()
        {


            // The testing for tilemaps
            map = Map.Load(Path.Combine(Content.RootDirectory, "TESTMAP.tmx"), Content);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgrounds = new SpriteBatch(GraphicsDevice);

            // Setting up render target
            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;

            renderTarget = new RenderTarget2D(graphics.GraphicsDevice,  pp.BackBufferWidth, pp.BackBufferHeight, true, SurfaceFormat.Color, DepthFormat.Depth24);

            // Load extra
            font = Content.Load<SpriteFont>("font");
            zoom = Content.Load<Effect>("zoom");
            background = Content.Load<Texture2D>("space");//"p_u_r_p_b_o_y_s"); 
            doggo = Content.Load<Texture2D>("DOGGIE");//"p_u_r_p_b_o_y_s"); 


            // Load sprites n stuff
            playerSpriteSheet = Content.Load<Texture2D>("explorer");
            hitBoxSpriteSheet = Content.Load<Texture2D>("hitbox");
            bulletSprite2 = Content.Load<Texture2D>("bullet_2");
            bulletSprite = Content.Load<Texture2D>("bullet_1");
            Texture2D mosueSprite = Content.Load<Texture2D>("mouse");


            PlayerObject.CreatePlayer(new Vector2(300, 700), playerSpriteSheet, hitBoxSpriteSheet, bulletSprite2, mosueSprite);  // Oof
            
            /*
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
            */

            // Bar
            Vector2 barscale = Vector2.One * 0.29f;
            Vector2 barPos = new Vector2(300, 45); 

            ProgressBar bar = ProgressBar.Initialize() as ProgressBar;
            bar.sprite = new Sprite(Content.Load<Texture2D>("bar"));
            bar.sprite.Position = barPos;
            bar.sprite.Scale = barscale;
            bar.sprite.LayerDepth = 0.01f;
            bar.sprite.enableCam = false;

            bar.innerSprite = new Sprite(Content.Load<Texture2D>("inner"));
            bar.innerSprite.Position = barPos;
            bar.innerSprite.Color = Color.DarkRed;
            bar.innerSprite.Scale = barscale;
            bar.innerSprite.enableCam = false;
            bar.value = 100;

            // more boss stuff?
            //BossController boss = new BossController();
            //boss.healthbar = bar;

            //spawner.addBehavior(boss);
            //spawner.addBehavior(new enemyHealth());

            //init
            foreach (GameObject2d gameObject in GameObject2d.activeGameObjects) gameObject.Start();
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        Vector2 b = Vector2.One;
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            GameObject2d.UpdateObjects();

            map.Layers.First().Value.Opacity = (float)(Math.Cos(Math.PI * (gameTime.TotalGameTime.Milliseconds * 4) / 10000));

            if (InputManager.IsKeyDown(Keys.Up)) { b.X += 0.01f; b.Y += 0.01f; }
            if (InputManager.IsKeyDown(Keys.Down) && b.Length() > 0) { b.X -= 0.01f; b.Y -= 0.01f; }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        Vector2 loc = Vector2.Zero;
        Vector2 drawLoc = Vector2.Zero;
        protected override void Draw(GameTime gameTime)
        {
            // Set the context to our render target
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            backgrounds.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            /*
            offset.Parameters["height"].SetValue(1000f);
            offset.Parameters["offset"].SetValue((float)Time.TotalGameTimeMilli/1000);
            offset.CurrentTechnique.Passes[0].Apply();

            /*
            backgrounds.Draw(background, new Rectangle(0, 0, 600, 1000), Color.White); //new Rectangle(0, 0, playerSpriteSheet.Width, playerSpriteSheet.Height), Color.White,0 , new Vector2 (300,1000), effec)
            */

            // T I L E M A P   L O G I C
            loc = PlayerObject.players[0].sprite.Position;

            
            Vector2 newPos = loc - new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight) / 2;
            if (!(newPos.X < 0 || newPos.X > 32 * 100 - graphics.PreferredBackBufferWidth*2)) {
                Sprite.cameraPosition.X = newPos.X;
                drawLoc.X = loc.X;
            }

            if( !(newPos.Y < 0 || newPos.Y > 32 * 100 - graphics.PreferredBackBufferHeight*2)) {
                Sprite.cameraPosition.Y = newPos.Y;
                drawLoc.Y = loc.Y;
            }
            map.Draw(backgrounds, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), drawLoc);

            backgrounds.End();
            
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            

            foreach (GameObject2d gameObject in GameObject2d.activeGameObjects.ToList()) gameObject.Render(spriteBatch);

            spriteBatch.DrawString(font, "Graze: " + grazeEnemy.grazeScore, new Vector2(50, 950), Color.White);
            

            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            // Handle post processing effects
            using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice)) {
                //sprite.Begin();
                
                zoom.Parameters["zoom"].SetValue(b);

                sprite.Begin(SpriteSortMode.Deferred, null, null, null, null, zoom);

                sprite.Draw(renderTarget, new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                sprite.End();
            }

            base.Draw(gameTime);
        }
    }
}
