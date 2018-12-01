using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using GameStateManagement;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using System.Linq;
using System;

namespace Final {

    public class Game1 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera camera = new Camera();

        Effect virtualTerrain;
        CustomTerrainRenderer terrainRenderer;
        GameObject3d terrainObject;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }


        protected override void Initialize() {
            camera.Transform.LocalPosition += Vector3.Up * 12;

            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);

            base.Initialize();
        }
        
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ScreenManager.Setup(false, 1080, 720);

            virtualTerrain = Content.Load<Effect>("virtualTerrain");
            CustomTerrainRenderer.effect = virtualTerrain;

            terrainRenderer = new CustomTerrainRenderer( Vector2.One * 200 );
            
            terrainObject = GameObject3d.Initialize();

            terrainObject.material = terrainRenderer;


            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects) gameObject.Start();
            GameObject.gameStarted = true;
        }
        
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float speed = 20;
            float rot = 10;

            if (InputManager.IsKeyDown(Keys.W)) camera.Transform.LocalPosition += speed * camera.Transform.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S)) camera.Transform.LocalPosition += speed * camera.Transform.Backward * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.A)) camera.Transform.LocalPosition += speed * camera.Transform.Left * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.D)) camera.Transform.LocalPosition += speed * camera.Transform.Right * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.Left)) camera.Transform.Rotate(Vector3.Up, rot * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Right)) camera.Transform.Rotate(Vector3.Down, rot * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Up)) camera.Transform.Rotate(Vector3.Left, rot * Time.ElapsedGameTime/3);
            if (InputManager.IsKeyDown(Keys.Down)) camera.Transform.Rotate(Vector3.Right, rot * Time.ElapsedGameTime/3);


            InputManager.Update();
            Time.Update(gameTime);
            
            GameObject3d.UpdateObjects();

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.DepthStencilState = new DepthStencilState();

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects.ToList())
                gameObject.Render(Tuple.Create(camera, GraphicsDevice));

            base.Draw(gameTime);
        }
    }
}
