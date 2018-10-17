using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System;
using System.Linq;

namespace Lab6 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab6 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera camera = new Camera();

        Model sphere;

        Random random;

        public Lab6() {
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
            InputManager.Initialize();
            Time.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the models
            sphere = Content.Load<Model>("Sphere");
            foreach (ModelMesh mesh in sphere.Meshes)
                foreach (BasicEffect e in mesh.Effects)
                    e.EnableDefaultLighting();

            // Misc Setup
            camera.Transform.LocalPosition = new Vector3(0, 0, 20);

            random = new Random();

            BoxCollider boxCollider;
            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            GameObject3d b = GameObject3d.Initialize();
            b.drawable = false;
            b.addBehavior(boxCollider);

            for (int i = 0; i < 2; i++) {
                Transform transform = new Transform();
                transform.LocalPosition += new Vector3(i*5, 0, 0); //avoid overlapping each sphere 

                Rigidbody rigidbody = new Rigidbody();
                rigidbody.Mass = 1;

                Vector3 direction = new Vector3(
                  (float)random.NextDouble(), (float)random.NextDouble(),
                  (float)random.NextDouble());
                direction.Normalize();
                rigidbody.Velocity = direction * ((float) random.NextDouble() * 5 + 5);

                SphereCollider sphereCollider = new SphereCollider();
                sphereCollider.Radius = 2 * transform.LocalScale.Y;

                GameObject3d g = GameObject3d.Initialize();
                g.transform = transform;
                g.mesh = sphere;

                g.addBehavior(rigidbody);
                g.addBehavior(sphereCollider);
            }

            //init
            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects) gameObject.Start();
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
            Collider.Update(gameTime);

            GameObject3d.UpdateObjects();

            Collider.Update(gameTime);


            //DEBUG CAM MOVEMENT

            float speed = 2;
            float rot = 2;

            if (InputManager.IsKeyDown(Keys.W)) camera.Transform.LocalPosition += speed * camera.Transform.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S)) camera.Transform.LocalPosition += speed * camera.Transform.Backward * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.A)) camera.Transform.Rotate(Vector3.Up, rot * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.D)) camera.Transform.Rotate(Vector3.Down, rot * Time.ElapsedGameTime);


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects.ToList()) gameObject.Render(camera);

            base.Draw(gameTime);
        }
    }
}
