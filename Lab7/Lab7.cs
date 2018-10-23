using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System;
using System.Threading;
using System.Linq;

namespace Lab7 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab7 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera camera = new Camera();

        Model sphere;

        Random random;

        SpriteBatch backgrounds;
        public static Texture2D background;
        Effect offset;

        public Lab7() {
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

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            backgrounds = new SpriteBatch(GraphicsDevice);

            // Load shaders 
            SpeedAndCollideEffect.effect = Content.Load<Effect>("SpeedEffectShader");
            SpeedAndCollideEffect.disperseSample = Content.Load<Texture2D>("noiseTexture");
            offset = Content.Load<Effect>("offset");
            background = Content.Load<Texture2D>("DOGGIE");//"p_u_r_p_b_o_y_s"); 

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

            //DEBUG CAM MOVEMENT

            float speed = 6;
            float rot = 2;

            if (InputManager.IsKeyDown(Keys.W)) camera.Transform.LocalPosition += speed * camera.Transform.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S)) camera.Transform.LocalPosition += speed * camera.Transform.Backward * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.A)) camera.Transform.Rotate(Vector3.Up, rot * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.D)) camera.Transform.Rotate(Vector3.Down, rot * Time.ElapsedGameTime);

            if (InputManager.IsKeyDown(Keys.Space)) makeThatSphere();


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            backgrounds.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            offset.Parameters["height"].SetValue(600f);
            offset.Parameters["offset"].SetValue((float)Time.TotalGameTimeMilli / 1000);
            offset.CurrentTechnique.Passes[0].Apply();
            
            backgrounds.Draw(background, new Rectangle(0, 0, 1000, 600), Color.White); //new Rectangle(0, 0, playerSpriteSheet.Width, playerSpriteSheet.Height), Color.White,0 , new Vector2 (300,1000), effec)

            backgrounds.End();

            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects.ToList()) gameObject.Render( Tuple.Create(camera,GraphicsDevice) );

            base.Draw(gameTime);
        }
    }
}
