﻿using System.Collections.Generic;
using System.Linq;
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GameStateManagement;

namespace Final {
    class Main : Game {
        public GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        public Main() {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);
            CPI311.GameEngine.GameScreenManager.Initialize(graphics);
            CPI311.GameEngine.GameScreenManager.Setup(false, 1080, 720);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;

            // Create the screen manager component.
            screenManager = new ScreenManager(this);

            screenManager.Initialize();

            Components.Add(screenManager);
            
            screenManager.AddScreen(new MainMenu(), null);
        }

        protected override void Draw(GameTime gameTime) {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }
    }
}