using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Audio;

namespace Assignment5 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGameScreen : Game {
        GraphicsDeviceManager graphics;
        public static Random random;

        SpriteBatch spriteBatch;
        SpriteFont font;

        Camera camera = new Camera();
        //Light light;
        //Audio components
        public static SoundEffect gunSound;
        public static SoundEffectInstance soundInstance;

        //Score & background
        int score;
        Texture2D stars;
        SpriteFont lucidaConsole;
        Vector2 scorePosition = new Vector2(100, 50);

        // Particles
        public static ParticleManager particleManager;
        Texture2D particleTex;
        Effect particleEffect;

        // J A C O B
        static GameObject3d Player;
        public Texture2D background;
        SpriteBatch backgrounds;
        Effect offset;

        public MainGameScreen() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            Time.Initialize();
            InputManager.Initialize();
            GameScreenManager.Initialize(graphics);
        } 

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            base.Initialize();
            random = new Random();

            camera.Transform.LocalPosition = Vector3.Backward * 100;
            camera.FarPlane = 5000;


            ResetAsteroids();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GameScreenManager.GraphicsDevice);

            // Fucking around
            backgrounds = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("DOGGIE");
            offset = Content.Load<Effect>("offset");


            // *** Lab 8 Item ***********************
            GameScreenManager.Setup(false, 1080, 720);
            //***************************************

            particleManager = new ParticleManager(GraphicsDevice, 100);
            particleEffect = Content.Load<Effect>("ParticleShader");
            PlayerBehav.bulletMesh = Content.Load<Model>("Sphere");
            AsteroidObject.AstroidModel = Content.Load<Model>("Sphere");
            AsteroidObject.tex = Content.Load<Texture2D>("lunaTexture");
            StandardLightingMaterial.effect = Content.Load<Effect>("StandardShading");
            particleTex = Content.Load<Texture2D>("fire");
            font = Content.Load<SpriteFont>("font");

            Player = GameObject3d.Initialize();
            Player.transform.LocalScale *= 0.01f;
            Player.transform.Rotate(Vector3.Left, -(float)Math.PI / 2);
            PlayerBehav.bulletMesh = Content.Load<Model>("Sphere");
            Player.addBehavior(new PlayerBehav());
            Rigidbody r = new Rigidbody();
            Player.addBehavior(r);
            SphereCollider s = new SphereCollider();
            Player.addBehavior(s);
            Player.mesh = Content.Load<Model>("PlayerModel");

            // Sound shit
            gunSound = Content.Load<SoundEffect>("Gun");
            soundInstance = gunSound.CreateInstance();
            AudioListener listener = new AudioListener();
            listener.Position = camera.Transform.Position;
            listener.Forward = camera.Transform.Forward;
            AudioEmitter emitter = new AudioEmitter();
            emitter.Position = Vector3.Zero;
            soundInstance.Apply3D(listener, emitter);

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
            Collider.Update(gameTime);
            particleManager.Update();

            GameObject3d.UpdateObjects();

            if (InputManager.IsKeyPressed(Keys.R)) ResetAsteroids();

            base.Update(gameTime);
        }
        

        private void ResetAsteroids() {
            AsteroidObject.initMany(30);
        }


        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            backgrounds.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            offset.Parameters["height"].SetValue((float)GameScreenManager.Height);
            offset.Parameters["offset"].SetValue((float)Time.TotalGameTimeMilli / 1500);
            offset.CurrentTechnique.Passes[0].Apply();
            backgrounds.GraphicsDevice.Viewport = camera.Viewport;
            backgrounds.Draw(background, new Rectangle(0, 0,
                                                        (int)(camera.Size.X * GameScreenManager.Width),
                                                        (int)(camera.Size.Y * GameScreenManager.Height)), Color.White);
            backgrounds.End();



            GraphicsDevice.DepthStencilState = new DepthStencilState();

            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects.ToList())
                gameObject.Render(Tuple.Create(camera, GraphicsDevice));

            //particle draw
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            particleEffect.CurrentTechnique = particleEffect.Techniques["particle"];
            particleEffect.CurrentTechnique.Passes[0].Apply();
            particleEffect.Parameters["ViewProj"].SetValue(camera.View * camera.Projection);
            particleEffect.Parameters["World"].SetValue(Matrix.Identity);
            particleEffect.Parameters["CamIRot"].SetValue(
                    Matrix.Invert(Matrix.CreateFromQuaternion(camera.Transform.Rotation)));
            particleEffect.Parameters["Texture"].SetValue(particleTex);
            
            particleManager.Draw(GraphicsDevice); 

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.DrawString(font, "Score: " + GameConstants.score, new Vector2(0, 650), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
