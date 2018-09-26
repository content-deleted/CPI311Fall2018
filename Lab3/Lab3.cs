using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;

namespace Lab3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab3 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Vector3 cameraPosition = new Vector3(0, 0, 10);

        public Lab3()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            InputManager.Initialize();
            Time.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        Model model;

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("Torus");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            var moveSpeed = 10;
            if (InputManager.IsKeyDown(Keys.D)) cameraPosition.X += Time.ElapsedGameTime * moveSpeed;
            if (InputManager.IsKeyDown(Keys.A)) cameraPosition.X -= Time.ElapsedGameTime * moveSpeed;
            if (InputManager.IsKeyDown(Keys.W)) cameraPosition.Y += Time.ElapsedGameTime * moveSpeed;
            if (InputManager.IsKeyDown(Keys.S)) cameraPosition.Y -= Time.ElapsedGameTime * moveSpeed;

            if (InputManager.IsKeyDown(Keys.Right)) modelPosition.X += Time.ElapsedGameTime * moveSpeed;
            if (InputManager.IsKeyDown(Keys.Left)) modelPosition.X -= Time.ElapsedGameTime * moveSpeed;
            if (InputManager.IsKeyDown(Keys.Up)) modelPosition.Z += Time.ElapsedGameTime * moveSpeed;
            if (InputManager.IsKeyDown(Keys.Down)) modelPosition.Z -= Time.ElapsedGameTime * moveSpeed;

            // TODO: Add your update logic here
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        Matrix view, world, projection;
        float yaw, pitch, roll;
        float modelScale = 1;
        Vector3 modelPosition = new Vector3(0,0,0);

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + Vector3.Forward, Vector3.Up);
            world = Matrix.CreateScale(modelScale) * Matrix.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix.CreateTranslation(modelPosition);
            projection = Matrix.CreatePerspectiveFieldOfView(
                            MathHelper.PiOver2,     // field of view
                            GraphicsDevice.Viewport.AspectRatio, // screen aspect
                            0.1f, 1000f); // near and far planes


            model.Draw(world, view, projection);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
