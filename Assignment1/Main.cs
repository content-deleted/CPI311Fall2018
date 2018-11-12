using System.Collections.Generic;
using System.Linq;
using System;

using Microsoft.Xna.Framework;

using GameStateManagement;

namespace Assignment1 {
    class Main : Game {
        public GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        public Main() {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            // Create the screen manager component.
            screenManager = new ScreenManager(this);

            Components.Add(screenManager);
            
            screenManager.AddScreen(new PlayerSelectScreen(), null);
        }

        protected override void Draw(GameTime gameTime) {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }
    }
}
