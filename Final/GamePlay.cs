using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using System.Linq;
using System;
using Microsoft.Xna.Framework.Content;

namespace Final {

    public class Gameplay : GameScreen {
        SpriteBatch spriteBatch;

        Camera camera = new Camera();

        Effect virtualTerrain;
        CustomTerrainRenderer terrainRenderer;
        GameObject3d terrainObject;

        ContentManager content;

        public Gameplay() {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            
            camera.Transform.LocalPosition += new Vector3(100, 0, 10);
            camera.Transform.Rotate(Vector3.Up, (float)Math.PI);

            Time.Initialize();
            InputManager.Initialize();
            
        }
        
        public override void LoadContent() {

            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GameScreenManager.GraphicsDevice);

            virtualTerrain = content.Load<Effect>("virtualTerrain");
            CustomTerrainRenderer.wire = content.Load<Texture2D>("wire");
            CustomTerrainRenderer.effect = virtualTerrain;

            terrainRenderer = new CustomTerrainRenderer( Vector2.One * 200 );
            
            terrainObject = GameObject3d.Initialize();
            terrainRenderer.obj = terrainObject;

            terrainObject.material = terrainRenderer;

            //SET CAM
            Vector3 pos = camera.Transform.Position;
            pos.Y = 2 + terrainRenderer.GetAltitude(camera.Transform.Position);
            camera.Transform.LocalPosition = pos;

            Hoop.effect = content.Load<Effect>("hoop");

            GameObject3d hoop = GameObject3d.Initialize();
            hoop.transform.LocalPosition = camera.Transform.LocalPosition - camera.Transform.Forward * 20;
            hoop.material = new Hoop(5f, 7f, 1f, 10);

            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects) gameObject.Start();
            GameObject.gameStarted = true;
        }
        
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            float speed = 20;
            float rot = 10;

            if (InputManager.IsKeyDown(Keys.W)) camera.Transform.LocalPosition += speed * camera.Transform.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S)) camera.Transform.LocalPosition += speed * camera.Transform.Backward * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.A)) camera.Transform.LocalPosition += speed * camera.Transform.Left * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.D)) camera.Transform.LocalPosition += speed * camera.Transform.Right * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.Left)) camera.Transform.Rotate(camera.Transform.Up, rot * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Right)) camera.Transform.Rotate(camera.Transform.Down, rot * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Up)) camera.Transform.Rotate(Vector3.Left, rot * Time.ElapsedGameTime/3);
            if (InputManager.IsKeyDown(Keys.Down)) camera.Transform.Rotate(Vector3.Right, rot * Time.ElapsedGameTime/3);

            Vector3 pos = camera.Transform.Position;
            pos.Y = 2+terrainRenderer.GetAltitude(camera.Transform.Position);
            camera.Transform.LocalPosition = pos;

            InputManager.Update();
            Time.Update(gameTime);
            
            GameObject3d.UpdateObjects();
        }
        
        public override void Draw(GameTime gameTime) {
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);
            
            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects.ToList())
                gameObject.Render(Tuple.Create(camera, GameScreenManager.GraphicsDevice));
        }
    }
}
