using System.Collections.Generic;
using System.Linq;
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using System.Diagnostics;

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
        Effect offset;

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
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgrounds = new SpriteBatch(GraphicsDevice);

            // Load extra
            font = Content.Load<SpriteFont>("font");
            offset = Content.Load<Effect>("offset");
            background = Content.Load<Texture2D>("space");//"p_u_r_p_b_o_y_s"); 
            doggo = Content.Load<Texture2D>("DOGGIE");//"p_u_r_p_b_o_y_s"); 


            // Load sprites n stuff
            playerSpriteSheet = Content.Load<Texture2D>("explorer");
            hitBoxSpriteSheet = Content.Load<Texture2D>("hitbox");
            bulletSprite2 = Content.Load<Texture2D>("bullet_2");
            PlayerObject.CreatePlayer(new Vector2(300, 700), playerSpriteSheet, hitBoxSpriteSheet, bulletSprite2);  // Oof

            bulletSprite = Content.Load<Texture2D>("bullet_1");
            GameObject2d spawner = GameObject2d.Initialize();

            spawner.sprite = new Sprite(bulletSprite);
            spawner.sprite.Scale *= 2;
            spawner.sprite.LayerDepth = 0.4f;
            spawner.addBehavior(new grazeEnemy());
            BulletSpawner b = new BulletSpawner();
            spawner.sprite.Position = new Vector2(300, 200);

            b.bulletSpeed = 2;
            b.bulletAmount = 3;            
            
            b.bulletfrequency = 5; // ms 

            b.spin = 0.3f;
            b.wave = 0.2f;

            b.bulletSprite = bulletSprite;
            b.scale = Vector2.One * 0.4f;
            spawner.addBehavior(b);


            // MOUSE
            GameObject2d mouse = GameObject2d.Initialize();
            mouse.sprite = new Sprite(Content.Load<Texture2D>("mouse"));
            mouse.addBehavior(new AnchorMouseBehavior());

            // Bar
            Vector2 barscale = Vector2.One * 0.29f;
            Vector2 barPos = new Vector2(300, 45); 

            ProgressBar bar = ProgressBar.Initialize() as ProgressBar;
            bar.sprite = new Sprite(Content.Load<Texture2D>("bar"));
            bar.sprite.Position = barPos;
            bar.sprite.Scale = barscale;
            bar.sprite.LayerDepth = 0.01f;

            bar.innerSprite = new Sprite(Content.Load<Texture2D>("inner"));
            bar.innerSprite.Position = barPos;
            bar.innerSprite.Color = Color.DarkRed;
            bar.innerSprite.Scale = barscale;
            bar.value = 100;

            // more boss stuff?
            BossController boss = new BossController();
            boss.healthbar = bar;

            spawner.addBehavior(boss);
            spawner.addBehavior(new enemyHealth());

            //init
            foreach (GameObject2d gameObject in GameObject2d.activeGameObjects) gameObject.Start();
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            GameObject2d.UpdateObjects();
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            backgrounds.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            offset.Parameters["height"].SetValue(1000f);
            offset.Parameters["offset"].SetValue(Time.TotalGameTimeMilli/1000);
            offset.CurrentTechnique.Passes[0].Apply();


            backgrounds.Draw(background, new Rectangle(0, 0, 600, 1000), Color.White); //new Rectangle(0, 0, playerSpriteSheet.Width, playerSpriteSheet.Height), Color.White,0 , new Vector2 (300,1000), effec)

            backgrounds.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            foreach (GameObject2d gameObject in GameObject2d.activeGameObjects.ToList()) gameObject.Render(spriteBatch);

            spriteBatch.DrawString(font, "Graze: " + grazeEnemy.grazeScore, new Vector2(50, 950), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
