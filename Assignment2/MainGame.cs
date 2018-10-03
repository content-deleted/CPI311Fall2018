using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;

namespace Assignment2 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model sphereModel;

        Transform earthTransform = new Transform();
        Transform sunTransform = new Transform();
        Transform lunaTransform = new Transform();
        Transform marsTransform = new Transform();

        Camera camera = new Camera();

        Effect sunEffect;

        int index = 0;

        public MainGame() {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
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

            earthTransform.LocalPosition = new Vector3(5, 0, 0);
            lunaTransform.LocalPosition = new Vector3(3, 0, 0);

            camera.Transform.LocalPosition = new Vector3(0, 0, 5);

            foreach (ModelMesh mesh in sphereModel.Meshes)
                foreach (BasicEffect e in mesh.Effects)
                    e.EnableDefaultLighting();

            lunaTransform.Parent = earthTransform;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sphereModel = Content.Load<Model>("Sphere");
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
            float rot = 2;
            float speed = 10;

            InputManager.Update();
            Time.Update(gameTime);


            if (InputManager.IsKeyPressed(Keys.Tab)) index = (index + 1) % 4;

            if (InputManager.IsKeyDown(Keys.W)) camera.Transform.LocalPosition += speed * camera.Transform.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S)) camera.Transform.LocalPosition += speed * camera.Transform.Backward * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.A)) camera.Transform.Rotate(Vector3.Up, rot * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.D)) camera.Transform.Rotate(Vector3.Down, rot * Time.ElapsedGameTime);

            if (InputManager.IsKeyDown(Keys.Up)) earthTransform.LocalPosition += Vector3.Forward;
            if (InputManager.IsKeyDown(Keys.Down)) earthTransform.LocalPosition += Vector3.Backward;

            earthTransform.Rotate(Vector3.Up, rot * Time.ElapsedGameTime); 

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            sphereModel.Draw(earthTransform.World, camera.View, camera.Projection);
            sphereModel.Draw(lunaTransform.World, camera.View, camera.Projection);

            base.Draw(gameTime);
        }
    }
}
