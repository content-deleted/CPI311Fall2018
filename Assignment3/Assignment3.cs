using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace Assignment3 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Assignment3 : Game {
        GraphicsDeviceManager graphics;

        Camera camera = new Camera();

        Model sphere;

        Random random;

        SpriteFont font;

        SpriteBatch backgrounds;
        SpriteBatch text;
        string UITEXT="";
        float offsetSpeed = 1;
        float tileAmount = 1;

        public static Texture2D background;
        Effect offset;

        public Assignment3() {
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
            InputManager.Initialize();
            Time.Initialize();
            ScreenManager.Initialize(graphics);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            ScreenManager.Setup(false, 1080, 720);
            // Create a new SpriteBatch, which can be used to draw textures.
            backgrounds = new SpriteBatch(GraphicsDevice);
            text = new SpriteBatch(GraphicsDevice);

            // Load Font
            font = Content.Load<SpriteFont>("font");

            // Load shaders 
            SpeedAndCollideEffect.effect = Content.Load<Effect>("SpeedEffectShader");
            SpeedAndCollideEffect.disperseSample = Content.Load<Texture2D>("noiseTexture");
            offset = Content.Load<Effect>("offset");
            background = Content.Load<Texture2D>("DOGGIE");

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
                makeThatSphere();
            }

            //init
            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects) gameObject.Start();
            GameObject.gameStarted = true;
        }

        private void makeThatSphere() {
            Transform transform = new Transform();
            transform.LocalPosition += new Vector3((float)random.NextDouble() * 5, (float)random.NextDouble() * 5, (float)random.NextDouble() * 5); //avoid overlapping each sphere 

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Mass = 1;

            Vector3 direction = new Vector3(
              (float)random.NextDouble(), (float)random.NextDouble(),
              (float)random.NextDouble());
            direction.Normalize();
            rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);
            rigidbody.Mass = 1f + (float)random.NextDouble();

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1 * transform.LocalScale.Y;

            SpeedColorEffectController effectControl = new SpeedColorEffectController();

            GameObject3d g = GameObject3d.Initialize();
            g.transform = transform;
            g.mesh = sphere;
            g.material = new SpeedAndCollideEffect();

            g.addBehavior(rigidbody);
            g.addBehavior(sphereCollider);
            g.addBehavior(effectControl);

            if (GameObject.gameStarted) g.Start();
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

            //CAM MOVEMENT

            float speed = 10;
            float rot = 3;

            if (InputManager.IsKeyDown(Keys.W)) camera.Transform.LocalPosition += speed * camera.Transform.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S)) camera.Transform.LocalPosition += speed * camera.Transform.Backward * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.A)) camera.Transform.Rotate(Vector3.Up, rot * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.D)) camera.Transform.Rotate(Vector3.Down, rot * Time.ElapsedGameTime);

            if (InputManager.IsKeyDown(Keys.Space)) makeThatSphere();
            if (InputManager.IsKeyPressed(Keys.R)) 
                GameObject3d.activeGameObjects.Where(g => g.mesh?.Equals(sphere) == true).FirstOrDefault()?.Destroy();

            if (InputManager.IsKeyDown(Keys.Up))
                foreach (GameObject3d g in GameObject3d.activeGameObjects) {
                    Rigidbody b = g.GetBehavior<Rigidbody>();
                    if(b != null) b.Velocity *= 1.05f;
                }
            if (InputManager.IsKeyDown(Keys.Down))
                foreach (GameObject3d g in GameObject3d.activeGameObjects) {
                    Rigidbody b = g.GetBehavior<Rigidbody>();
                    if (b != null) b.Velocity *= 0.95f;
                }

            if (InputManager.IsKeyPressed(Keys.T)) offsetSpeed += 0.1f;
            if (InputManager.IsKeyPressed(Keys.Y)) offsetSpeed -= 0.1f;

            if (InputManager.IsKeyPressed(Keys.G)) tileAmount += 1f;
            if (InputManager.IsKeyPressed(Keys.H)) tileAmount -= (tileAmount > 1) ? 1f : 0;

            //UpdateUI();

            base.Update(gameTime);
        }

        private Queue<int> prevCol = new Queue<int>(new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });

        void UpdateUI () {
            prevCol.Dequeue();
            int count=0;
            float totalVelocity = 0;
            int totalCol = 0;
            foreach (GameObject3d g in GameObject3d.activeGameObjects) {
                Rigidbody b = g.GetBehavior<Rigidbody>();
                if (b != null) {
                    count++;
                    totalVelocity += b.Velocity.Length();
                    SphereCollider s = g.GetBehavior<SphereCollider>();
                    if(s.collidedThisFrame) totalCol++;
                }
            }

            prevCol.Enqueue(totalCol);

            UITEXT = $"Ball Count: {count} \n" +
                     $"Average Velocity: {totalVelocity / (count > 0 ? count : 1)}\n" +
                     $"Average Collisions Per Frame: {(float) prevCol.Sum()/ prevCol.Count()}\n" +
                     $"Framerate: {(1 / Time.gameTime.ElapsedGameTime.TotalSeconds)}";
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            backgrounds.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            offset.Parameters["height"].SetValue((float)ScreenManager.Height);
            offset.Parameters["offset"].SetValue(offsetSpeed * (float)Time.TotalGameTimeMilli / 1000);
            offset.Parameters["tile"].SetValue(tileAmount);
            offset.CurrentTechnique.Passes[0].Apply();

            backgrounds.Draw(background, new Rectangle(0, 0,
                                                          (int)(camera.Size.X * ScreenManager.Width),
                                                          (int)(camera.Size.Y * ScreenManager.Height)), Color.White);
            backgrounds.End();

            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects.ToList()) gameObject.Render(Tuple.Create(camera, GraphicsDevice));

            UpdateUI();

            text.Begin();
            text.DrawString(font, UITEXT, new Vector2(50, 25), Color.White);
            text.End();
                
            base.Draw(gameTime);
        }
    }
}
