using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Linq;

namespace Lab8 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab8 : Game {
        SpriteBatch backgrounds;
        public static Texture2D background;

        GraphicsDeviceManager graphics;

        SoundEffect gunSound;
        SoundEffectInstance soundInstance;

        Model model;
        Camera camera1, topDownCamera;
        List<Camera> cameras = new List<Camera>();
        
        Effect offset;


        public Lab8() {
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
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);

            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            backgrounds = new SpriteBatch(GraphicsDevice);
            StandardLightingMaterial.effect = Content.Load<Effect>("Standard");
            model = Content.Load<Model>("model");
            background = Content.Load<Texture2D>("DOGGIE");
            offset = Content.Load<Effect>("offset");

            // *** Lab 8 Item ***********************
            ScreenManager.Setup(false, 1080, 720);
            //***************************************

            camera1 = new Camera();
            camera1.Transform = new Transform();
            camera1.Transform.LocalPosition = Vector3.Backward * 5;
            camera1.Position = new Vector2(0f, 0f);
            camera1.Size = new Vector2(0.5f, 1f);
            camera1.AspectRatio = camera1.Viewport.AspectRatio;

            topDownCamera = new Camera();
            topDownCamera.Transform = new Transform();
            topDownCamera.Transform.LocalPosition = Vector3.Up * 10;
            topDownCamera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            topDownCamera.Position = new Vector2(0.5f, 0f);
            topDownCamera.Size = new Vector2(0.5f, 1f);
            topDownCamera.AspectRatio = topDownCamera.Viewport.AspectRatio;

            cameras.Add(topDownCamera);
            cameras.Add(camera1);

            // Add sphere
            Transform transform = new Transform();
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1 * transform.LocalScale.Y;
            GameObject3d g = GameObject3d.Initialize();
            g.transform = transform;
            g.mesh = model;
            StandardLightingMaterial mat = new StandardLightingMaterial();
            mat.useTexture = false;
            mat.lightPosition = transform.Position + Vector3.Backward * 5;
            g.material = mat;
            g.addBehavior(sphereCollider);

            //init
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
            Collider.Update(gameTime);

            GameObject3d.UpdateObjects();

            Find();

            base.Update(gameTime);
        }


        public void Find() {
            Ray ray = cameras.First().ScreenPointToWorldRay(InputManager.GetMousePosition());

            foreach (Collider collider in Collider.colliders) 
                ((collider.obj as GameObject3d).material as StandardLightingMaterial).diffuseColor =
                (collider.Intersects(ray) != null) ? Color.Blue.ToVector3() : Color.Red.ToVector3();
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            backgrounds.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            foreach (Camera camera in cameras) {
                offset.Parameters["height"].SetValue((float)ScreenManager.Height);
                offset.Parameters["offset"].SetValue((float)Time.TotalGameTimeMilli / 1500);
                offset.CurrentTechnique.Passes[0].Apply();
                backgrounds.GraphicsDevice.Viewport = camera.Viewport;
                backgrounds.Draw(background, new Rectangle(0,0,
                                                          (int)(camera.Size.X * ScreenManager.Width),
                                                          (int)(camera.Size.Y * ScreenManager.Height)), Color.White);
            }
            backgrounds.End();

           foreach (Camera camera in cameras) 
               foreach (GameObject3d gameObject in GameObject3d.activeGameObjects.ToList())
                   gameObject.Render(Tuple.Create(camera, GraphicsDevice));


            base.Draw(gameTime);
        }
    }
}
