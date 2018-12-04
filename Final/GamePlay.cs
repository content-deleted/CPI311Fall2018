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
        public RenderTarget2D renderTarget;
        Effect postProcess;

        Camera camera = new Camera();

        Effect virtualTerrain;
        CustomTerrainRenderer terrainRenderer;
        GameObject3d terrainObject;

        ContentManager content;

        Mp3FileReader reader;
        WaveOut waveOut;
        List<Mp3Frame> mp3Frames = new List<Mp3Frame>();
        Texture2D test;
        Texture2D background;

        byte[] avgE;

        public Gameplay(SongSelect.songInfo s) {

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            camera.Transform.LocalPosition += new Vector3(100, 0, 10);
            camera.Transform.Rotate(Vector3.Up, (float)Math.PI);


            //SONG LOADING AFTER THIS
            reader = new Mp3FileReader(s.songPath); //reader = new AudioFileReader(s.songPath);

            waveOut = new WaveOut();

            var totalLength = reader.Length;
            Mp3Frame b = reader.ReadNextFrame();

            while (b != null) {
                mp3Frames.Add(b);
                b = reader.ReadNextFrame();
            }

            var MaxFrameLength = mp3Frames.Max(f => f.FrameLength);

            test = new Texture2D(GameScreenManager.GraphicsDevice, MaxFrameLength, mp3Frames.Count());

            int index = 0;

            uint[] totalData = new uint[MaxFrameLength * mp3Frames.Count()];

            avgE = new byte[mp3Frames.Count()];
            int i=0;
            byte lastInput = 0;
            foreach (Mp3Frame frame in mp3Frames) {
                //FastFourierTransform.HammingWindow(0, )
                //FastFourierTransform.FFT(true, 2, )
                long sum = frame.RawData.Sum(x=> (uint) x);
                
                byte a = (byte)( ((sum / frame.RawData.Length) + lastInput) / 2);
                avgE[i++] = a;
                lastInput = a;

                Array.Copy(frame.RawData, 0, totalData, index, frame.RawData.Length);
                //totalData[index] 
                //test.SetData(frame.RawData, index, frame.RawData.Length);
                index += MaxFrameLength;
            }
            test.SetData(totalData);

            CustomTerrainRenderer.song = test;

            reader.Seek(0, System.IO.SeekOrigin.Begin);

            waveOut.Init(reader);
            
            //Time.timers.Add(new EventTimer(incCountDown, 2));

            //FastFourierTransform.
        }
        /*
        public int countdown = 100;
        public void incCountDown() {
            countdown--;
            terrainRenderer.heightAlter = (1) / (countdown+1);
            if (countdown <= 0) {
                vert = terrainRenderer.lastRowDepth;
                Time.timers.Add(new EventTimer(PlayEvent, 0.5f));
            }
            else Time.timers.Add(new EventTimer(incCountDown, 0.001f));
        }
        public int vert;

        public void PlayEvent() {
            if(vert == terrainRenderer.lastRowDepth % 200) waveOut.Play();
            else Time.timers.Add(new EventTimer(PlayEvent, 0.0001f));
        }*/

        public void newHoop() {
            hoopObject.Initialize();
            Time.timers.Add(new EventTimer(newHoop, 5f));
        }

        public override void LoadContent() {

            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GameScreenManager.GraphicsDevice);
            background = content.Load<Texture2D>("back");

            // Setting up render target
            PresentationParameters pp = ScreenManager.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true, SurfaceFormat.Color, DepthFormat.Depth24);
            postProcess = content.Load<Effect>("PostProcess");

            virtualTerrain = content.Load<Effect>("virtualTerrain");
            CustomTerrainRenderer.wire = content.Load<Texture2D>("wire");
            CustomTerrainRenderer.effect = virtualTerrain;

            terrainRenderer = new CustomTerrainRenderer(Vector2.One * 200);

            terrainObject = GameObject3d.Initialize();
            terrainRenderer.obj = terrainObject;

            terrainObject.material = terrainRenderer;

            //SET CAM
            Vector3 pos = camera.Transform.Position;
            pos.Y = 2 + terrainRenderer.GetAltitude(camera.Transform.Position);
            camera.Transform.LocalPosition = pos;
            camera.NearPlane = 0.001f;
            // Setup skybox
            camera.skybox = new Skybox { skyboxModel = content.Load<Model>("Box"), skyboxTexture = background };

            Skybox.shader = content.Load<Effect>("skybox");

            //SET HOOP
            hoopLogic.player = camera;
            hoopObject.lastPos = camera.Transform.LocalPosition + new Vector3(0, 2, 100);
            hoopObject.Initialize();

            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects) gameObject.Start();
            GameObject.gameStarted = true;
        }

        public bool songStarted = false;

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {

            InputManager.Update();
            Time.Update(gameTime);

            GameObject3d.UpdateObjects();

            while (terrainRenderer.lastRowDepth < camera.Transform.Position.Z - 5) {
                terrainRenderer.updateNormals(Math.Abs(terrainRenderer.lastRowDepth - 5), Vector3.Up);
                terrainRenderer.updateDepth(terrainRenderer.lastRowDepth);
                terrainRenderer.updateNormals(terrainRenderer.lastRowDepth + 1, Vector3.Down);
            }

            terrainRenderer.songPos = (float)waveOut.GetPosition() / (float)reader.Length;
            if (terrainRenderer.songPos < 1) terrainRenderer.avgE = 1 / (float)(avgE[(int)(avgE.Length*terrainRenderer.songPos)] - 100);

            if (!songStarted && terrainRenderer.lastRowDepth > 100) {
                songStarted = true;
                newHoop();
                waveOut.Play();
            }
            if (InputManager.IsKeyPressed(Keys.T)) postToggle = !postToggle;
            if (InputManager.IsKeyPressed(Keys.F)) GameScreenManager.graphics.ToggleFullScreen();
            updateCam();
            //pos.Y += terrainRenderer.avgE;
            //terrainRenderer.totalFrames = mp3Frames.Count();
        }

        // Camera update function because Im lazy
        public Vector3 cameraCurrectVelocity;
        public float camForwardSpeed = 15;
        public float leftRightSpeed = 0.075f;
        public float upDownSpeed = 0.04f;

        public float curUpDown = 0;
        public float curLeftRight = 0;
        
        public void updateCam() {
            if (songStarted && 2 + terrainRenderer.GetAltitude(camera.Transform.Position) > camera.Transform.Position.Y) {
                // camera crashes
                int i = 0;
            }
            
            if (InputManager.IsKeyDown(Keys.A)) curLeftRight += Time.ElapsedGameTime * leftRightSpeed;
            if (InputManager.IsKeyDown(Keys.D)) curLeftRight -= Time.ElapsedGameTime * leftRightSpeed;

            if (InputManager.IsKeyDown(Keys.W)) curUpDown -= Time.ElapsedGameTime * upDownSpeed;
            if (InputManager.IsKeyDown(Keys.S)) curUpDown += Time.ElapsedGameTime * upDownSpeed;

            cameraCurrectVelocity.X += curLeftRight;
            cameraCurrectVelocity.Y += curUpDown;

            cameraCurrectVelocity.Y = MathHelper.Clamp( cameraCurrectVelocity.Y, -0.1f, 0.1f);
            cameraCurrectVelocity.X = MathHelper.Clamp(cameraCurrectVelocity.X, -0.15f, 0.15f);
            cameraCurrectVelocity.Z = (camForwardSpeed) * Time.ElapsedGameTime + cameraCurrectVelocity.Y / 5;
            
            camera.Transform.lookAt(camera.Transform.LocalPosition + cameraCurrectVelocity);
            camera.Transform.LocalPosition += cameraCurrectVelocity;
            camera.Transform.Rotate(camera.Transform.Forward, 1 * cameraCurrectVelocity.X);

            curLeftRight /= 1.5f;
            curUpDown /= 1.2f;
        }

        bool postToggle=true;

        public override void Draw(GameTime gameTime) {
            // Set our graphics device to draw to a texture
            ScreenManager.GraphicsDevice.SetRenderTarget(renderTarget);
            ScreenManager.GraphicsDevice.Clear(Color.Purple);

            camera.drawSkybox(GameScreenManager.GraphicsDevice);

            postProcess.Parameters["toggle"].SetValue(postToggle);

            foreach (GameObject3d gameObject in GameObject3d.activeGameObjects.ToList())
                gameObject.Render(Tuple.Create(camera, GameScreenManager.GraphicsDevice));

            // Make sure to clear the graphics device render target
            ScreenManager.GraphicsDevice.SetRenderTarget(null);

            // Handle post processing effects
            using (SpriteBatch sprite = new SpriteBatch(ScreenManager.GraphicsDevice)) {
                //sprite.Begin();
                
                sprite.Begin(SpriteSortMode.Deferred, null, null, null, null, postProcess);

                sprite.Draw(renderTarget, new Rectangle(ScreenManager.GraphicsDevice.Viewport.X, ScreenManager.GraphicsDevice.Viewport.Y, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), Color.White);
                sprite.End();
            }

            //THIS IS THE TESTING CODE FOR DRAWING SOUND
            /*
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(test, new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();
            */
            //test, new Rectangle(500, 500, 250, 1000),null, Color.White, 3.141f,Vector2.One*500,SpriteEffects.None,0.5f);
        }
    }
}
