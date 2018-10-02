using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;

namespace Lab5 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab5 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model playerModel;
        Model planeModel;
        Model torusModel;

        Transform playerTransform = new Transform();
        Transform planeTransform = new Transform();
        Transform torusTransform = new Transform();

        Camera camera = new Camera();

        public Lab5() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            InputManager.Initialize();
            Time.Initialize();

            playerTransform.LocalPosition = new Vector3(2, 2, 2);

            torusTransform.LocalPosition = new Vector3(2, 0, 0);

            camera.Transform.LocalPosition = new Vector3(10, 1, 1);

            foreach (ModelMesh mesh in playerModel.Meshes)
                foreach (BasicEffect e in mesh.Effects)
                    e.EnableDefaultLighting();

            foreach (ModelMesh mesh in planeModel.Meshes)
                foreach (BasicEffect e in mesh.Effects)
                    e.EnableDefaultLighting();

            foreach (ModelMesh mesh in torusModel.Meshes)
                foreach (BasicEffect e in mesh.Effects)
                    e.EnableDefaultLighting();

            torusTransform.Parent = playerTransform;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            planeModel = Content.Load<Model>("Plane");
            playerModel = Content.Load<Model>("Sphere");
            torusModel = Content.Load<Model>("Torus");
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

            // TODO: Add your update logic here
            float rot = 5;
            float speed = 10;

            InputManager.Update();
            Time.Update(gameTime);

            if (InputManager.IsKeyDown(Keys.W)) camera.Transform.LocalPosition += speed * camera.Transform.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S)) camera.Transform.LocalPosition += speed * camera.Transform.Backward * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.A)) camera.Transform.Rotate(Vector3.Up, rot * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.D)) camera.Transform.Rotate(Vector3.Down, rot * Time.ElapsedGameTime);

            if (InputManager.IsKeyDown(Keys.Up)) playerTransform.LocalPosition += Vector3.Forward;
            if (InputManager.IsKeyDown(Keys.Down)) playerTransform.LocalPosition += Vector3.Backward;

            playerTransform.Rotate(Vector3.Up, rot * Time.ElapsedGameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            playerModel.Draw(playerTransform.World, camera.View, camera.Projection);
            planeModel.Draw(planeTransform.World, camera.View, camera.Projection);
            torusModel.Draw(torusTransform.World, camera.View, camera.Projection);

            base.Draw(gameTime);
        }
    }
}
