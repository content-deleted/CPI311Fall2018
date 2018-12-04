using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using CPI311.GameEngine.GUI;
using System.Collections.Generic;

namespace Lab11 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab11 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Dictionary<string, Scene> scenes;
        Scene currentScene;
        List<GUIElement> guiElements = new List<GUIElement>();
        SpriteFont font;

        public Lab11() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            scenes = new Dictionary<string, Scene>();
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            Time.Initialize();
            GameScreenManager.Initialize(graphics);

            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
         Checkbox toggle;
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");

            Button exitButton = new Button();
            exitButton.Text = "Exit";
            exitButton.Texture = Content.Load<Texture2D>("bar");
            exitButton.Bounds = new Rectangle(50, 50, 300, 50);
           // exitButton.Action += ExitGame;
            exitButton.Action += playScreen;
            

            toggle = new Checkbox();
            toggle.Text = "FullScreen";
            toggle.Texture = Content.Load<Texture2D>("bar");
            toggle.Bounds = new Rectangle(50, 120, 300, 50);
            toggle.Action += fullscreen;

            Checkbox.Box = Content.Load<Texture2D>("box");

            guiElements.Add(exitButton);
            guiElements.Add(toggle);
            // TODO: use this.Content to load your game content here
            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            currentScene = scenes["Menu"];

        }
        //void ExitGame(GUIElement element) => background = (background == Color.White ? Color.Blue : Color.White);

        void fullscreen(GUIElement element) => graphics.ToggleFullScreen();

        void playScreen( GUIElement element) => currentScene = scenes["Play"];

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            
            currentScene.Update();

            InputManager.Update();
            Time.Update(gameTime);

            //currentScene = (toggle.state) ? scenes["Play"] : scenes["Menu"];
            background = (currentScene == scenes["Play"] ? Color.Blue : Color.White);
            base.Update(gameTime);
        }
        void MainMenuUpdate() {
            foreach (GUIElement element in guiElements)
                element.Update();
        }
        void MainMenuDraw() {
            spriteBatch.Begin();
            foreach (GUIElement element in guiElements)
                element.Draw(spriteBatch, font);
            spriteBatch.End();
        }
        void PlayUpdate() {
            if (InputManager.IsKeyDown(Keys.Escape))
                currentScene = scenes["Menu"];
        }

        void PlayDraw() {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Play Mode! Press \"Esc\" to go back", Vector2.Zero, Color.Black);
            spriteBatch.End();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        Color background = Color.Blue;
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(background);

            // TODO: Add your drawing code here
            currentScene.Draw();
            base.Draw(gameTime);
        }
    }
}
