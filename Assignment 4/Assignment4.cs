using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System;
using System.Linq;

namespace Assignment_4 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Assignment4 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameObject3d terrainObject;
        TerrainRenderer terrain;
        Effect effect;
        SpriteBatch text;
        SpriteFont font;

        public Texture2D background;
        SpriteBatch backgrounds;
        Effect offset;

        Camera camera = new Camera();
        Camera mapCam = new Camera();

        GameObject3d playerObject;

        public static int failureCount = 0;
        public static int catchCount = 0;

        public Assignment4() {
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

            StandardLightingMaterial.effect = Content.Load<Effect>("Standard");
            text = new SpriteBatch(GraphicsDevice);
            // *** ScreenManager ***********************
            ScreenManager.Setup(false, 1080, 720);
            camera.Transform.LocalPosition = Vector3.Up * 20;
            camera.Transform.Rotate(camera.Transform.Left, (float)Math.PI/2);
            camera.FieldOfView = 2.6f;

            mapCam.orthographic = true;
            mapCam.Transform.LocalPosition = Vector3.Up * 100;
            mapCam.Transform.Rotate(mapCam.Transform.Left, (float)Math.PI / 2);
            
            mapCam.Size = Vector2.One * 100;
            mapCam.Position = new Vector2(0.9f, 0.01f);
            //***************************************

            // Fucking around
            backgrounds = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("DOGGIE");
            offset = Content.Load<Effect>("offset");

            Enemy.enemyModel = Content.Load<Model>("sphere");

            terrain = new TerrainRenderer(
               Content.Load<Texture2D>("HeightMap"),
               Vector2.One * 100, Vector2.One * 200);

            terrain.NormalMap = Content.Load<Texture2D>("NormalMap");

            terrainObject = GameObject3d.Initialize();

            terrain.ourObject = terrainObject;

            terrainObject.material = terrain;
            terrainObject.transform.LocalScale *= new Vector3(1, 5, 1);

            font = Content.Load<SpriteFont>("font");

            TerrainRenderer.effect = Content.Load<Effect>("TerrainShader");

            Enemy.terrain = terrain;

            // Create Player 
            playerObject = GameObject3d.Initialize();
            playerObject.addBehavior( new PlayerBehav(terrain) );
            playerObject.mesh = Content.Load<Model>("sphere");

            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects) gameObject.Start();
            GameObject.gameStarted = true;
        }

        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            if (InputManager.IsKeyPressed(Keys.R)) Enemy.createEnemy(playerObject);

                GameObject3d.UpdateObjects();
            base.Update(gameTime);
        }

        
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            #region DOG
            GraphicsDevice.Viewport = camera.Viewport;
            backgrounds.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            offset.Parameters["height"].SetValue((float)ScreenManager.Height);
            offset.Parameters["offset"].SetValue((float)Time.TotalGameTimeMilli / 1500);
            offset.CurrentTechnique.Passes[0].Apply();
            backgrounds.GraphicsDevice.Viewport = camera.Viewport;
            backgrounds.Draw(background, new Rectangle(0, 0,
                                                        (int)(camera.Size.X * ScreenManager.Width),
                                                        (int)(camera.Size.Y * ScreenManager.Height)), Color.White);
            backgrounds.End();
            #endregion

            GraphicsDevice.DepthStencilState = new DepthStencilState();



            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects.ToList())
                gameObject.Render(Tuple.Create(camera, GraphicsDevice));

            text.Begin();
            text.DrawString(font, "Failures: " + failureCount, new Vector2(0, 0), Color.White);
            text.DrawString(font, "Catches: " + catchCount, new Vector2(0, 30), Color.White);
            text.DrawString(font, "Time Spent: " + Time.TotalGameTime.ToString(), new Vector2(0, 60), Color.White);
            text.End();


            // MiniMap
            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects.ToList())
                gameObject.Render(Tuple.Create(mapCam, GraphicsDevice));
            base.Draw(gameTime);
        }
    }
}
