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
            // Set on sprites
            Sprite.currentMap = map;

            // Parse out and create our events
            ObjectGroup Events = map.ObjectGroups["EVENTS"];
            MapEvents.parseEvents(Events, content);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);
            backgrounds = new SpriteBatch(ScreenManager.GraphicsDevice);

            // Setting up render target
            PresentationParameters pp = ScreenManager.GraphicsDevice.PresentationParameters;

            renderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true, SurfaceFormat.Color, DepthFormat.Depth24);

            // Load extra
            font = content.Load<SpriteFont>("font");
            zoom = content.Load<Effect>("zoom");
            
            // THIS SHOULD BE MOVED
            Texture2D mosueSprite = content.Load<Texture2D>("mouse");

            //init
            foreach (GameObject2d gameObject in GameObject2d.activeGameObjects) gameObject.Start();
        }

        Vector2 b = Vector2.One;
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            InputManager.Update();
            Time.Update(gameTime);

            GameObject2d.UpdateObjects();

            // Testing screen stuff
            if (InputManager.IsKeyDown(Keys.Up)) { b.X += 0.01f; b.Y += 0.01f; }
            if (InputManager.IsKeyDown(Keys.Down) && b.Length() > 0) { b.X -= 0.01f; b.Y -= 0.01f; }
            if (InputManager.IsKeyPressed(Keys.F)) (ScreenManager.Game as Main).graphics.ToggleFullScreen();
        }

        Vector2 PlayerLoc = Vector2.Zero;
        Vector2 drawLoc = Vector2.Zero;

        public static int PreferredBackBufferWidth = 1280;
        public static int PreferredBackBufferHeight = 720;

        public override void Draw(GameTime gameTime) {
            // Set the context to our render target
            ScreenManager.GraphicsDevice.SetRenderTarget(renderTarget);
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            backgrounds.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null);

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
