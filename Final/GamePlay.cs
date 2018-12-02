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
//using Microsoft.Xna.Framework.Media;
using NAudio;
using NAudio.Wave;
using NAudio.Utils;
using NAudio.Wave.SampleProviders;
using NAudio.Dsp;
using System.Collections.Generic;

namespace Final {

    public class Gameplay : GameScreen {
        SpriteBatch spriteBatch;

        Camera camera = new Camera();

        Effect virtualTerrain;
        CustomTerrainRenderer terrainRenderer;
        GameObject3d terrainObject;

        ContentManager content;
        
        
        Mp3FileReader reader;
        WaveOut waveOut;
        List<Mp3Frame> mp3Frames = new List<Mp3Frame>();
        Texture2D test;

        public Gameplay(SongSelect.songInfo s) {

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            
            camera.Transform.LocalPosition += new Vector3(100, 0, 10);
            camera.Transform.Rotate(Vector3.Up, (float)Math.PI);

            // Load song
            //System.Uri uri = new System.Uri(s.songPath, System.UriKind.Relative);
            //song = Song.FromUri(s.songName,uri);
            //audioFileReader = new NAudio.Wave.AudioFileReader(s.songPath);

            reader = new Mp3FileReader(s.songPath);

            waveOut = new WaveOut(); // or WaveOutEvent()
            //byte[] buffer = new byte[2000];

            //reader.Read(buffer, 0, 2000);
            var totalLength = reader.Length;
            Mp3Frame b = reader.ReadNextFrame();
            
            while (b != null) {
                mp3Frames.Add(b);
                b = reader.ReadNextFrame();
            }

            var MaxFrameLength = mp3Frames.Max(f => f.FrameLength);

            test = new Texture2D(GameScreenManager.GraphicsDevice, MaxFrameLength, mp3Frames.Count());

            int index = 0;

            uint [] totalData = new uint[MaxFrameLength * mp3Frames.Count()];
            foreach (Mp3Frame frame in mp3Frames) {
                Array.Copy(frame.RawData, 0, totalData, index, frame.RawData.Length);
                //totalData[index] 
                //test.SetData(frame.RawData, index, frame.RawData.Length);
                index += MaxFrameLength;
            }
            test.SetData(totalData);

            CustomTerrainRenderer.song = test;

            reader.Seek(0, System.IO.SeekOrigin.Begin);

            waveOut.Init(reader);
            waveOut.Play();

            //FastFourierTransform.
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
            hoop.addBehavior(new hoopControl());


            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects) gameObject.Start();
            GameObject.gameStarted = true;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            float speed = 50;
            float rot = 10;

            if (InputManager.IsKeyDown(Keys.W)) camera.Transform.LocalPosition += speed * camera.Transform.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S)) camera.Transform.LocalPosition += speed * camera.Transform.Backward * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.A)) camera.Transform.LocalPosition += speed * camera.Transform.Left * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.D)) camera.Transform.LocalPosition += speed * camera.Transform.Right * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.Left)) camera.Transform.Rotate(camera.Transform.Up, rot * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Right)) camera.Transform.Rotate(camera.Transform.Down, rot * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Up)) camera.Transform.Rotate(Vector3.Left, rot * Time.ElapsedGameTime / 3);
            if (InputManager.IsKeyDown(Keys.Down)) camera.Transform.Rotate(Vector3.Right, rot * Time.ElapsedGameTime / 3);

            Vector3 pos = camera.Transform.Position;
            pos.Y = 2 + terrainRenderer.GetAltitude(camera.Transform.Position);
            camera.Transform.LocalPosition = pos;

            InputManager.Update();
            Time.Update(gameTime);

            GameObject3d.UpdateObjects();

            while (terrainRenderer.lastRowDepth < camera.Transform.Position.Z - 5) {
                terrainRenderer.updateNormals(Math.Abs(terrainRenderer.lastRowDepth - 5), Vector3.Up);
                terrainRenderer.updateDepth(terrainRenderer.lastRowDepth);
                terrainRenderer.updateNormals(terrainRenderer.lastRowDepth + 1, Vector3.Down);
            }

            terrainRenderer.songPos = (float)waveOut.GetPosition() / (float)reader.Length;
            //terrainRenderer.totalFrames = mp3Frames.Count();
        }
        
        public override void Draw(GameTime gameTime) {
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);
            
            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects.ToList())
                gameObject.Render(Tuple.Create(camera, GameScreenManager.GraphicsDevice));
        }
    }
}
