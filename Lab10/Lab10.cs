using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System;
using System.Linq;

namespace Lab10 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab10 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        TerrainRenderer terrain;
        Effect effect;

        Camera camera= new Camera();

        public Lab10() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


            // *** Lab 8 Item ***********************
            ScreenManager.Setup(false, 1080, 720);
            //***************************************

            GameObject3d TerrainObject = GameObject3d.Initialize();

            terrain = new TerrainRenderer(
                Content.Load<Texture2D>("Heightmap"),
                Vector2.One * 100, Vector2.One * 200);

            terrain.NormalMap = Content.Load<Texture2D>("Normalmap");

            terrain.ourObject = TerrainObject;

            TerrainObject.material = terrain;
            TerrainObject.transform.LocalScale *= new Vector3(1, 5, 1);

            effect = Content.Load<Effect>("TerrainShader");

            TerrainRenderer.effect = effect;

            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects) gameObject.Start();
            GameObject.gameStarted = true;
        }

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            if (InputManager.IsKeyDown(Keys.W)) // move forward
                camera.Transform.LocalPosition += camera.Transform.Forward * 10 * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.S)) 
                camera.Transform.LocalPosition -= camera.Transform.Forward * 10 * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.A)) camera.Transform.Rotate(Vector3.Up, 2 * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.D)) camera.Transform.Rotate(Vector3.Down, 2 * Time.ElapsedGameTime);


            camera.Transform.LocalPosition = new Vector3(camera.Transform.LocalPosition.X,
                                                             terrain.GetAltitude(camera.Transform.LocalPosition),
                                                                camera.Transform.LocalPosition.Z) + Vector3.Up;

            GameObject3d.UpdateObjects();
            base.Update(gameTime);
        }
        


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects.ToList())
                gameObject.Render(Tuple.Create(camera, GraphicsDevice));


            base.Draw(gameTime);
        }
    }
}
