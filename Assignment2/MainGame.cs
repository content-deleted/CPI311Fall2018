﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System.Collections.Generic;
using System;

namespace Assignment2 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model sphereModel;

        Transform earthTransform = new Transform();
        Transform solTransform = new Transform();
        Transform lunaTransform = new Transform();
        Transform mercuryTransform = new Transform();

        Texture2D earthTexture;
        Texture2D solTexture;
        Texture2D lunaTexture;
        Texture2D mercuryTexture;

        // Special textures used for sun shader
        Texture2D noise;
        Texture2D ramp;
        
        Camera firstPerson = new Camera();
        Camera topDown = new Camera();

        Camera camera = new Camera();

        Effect sunEffect;
        Effect standardLighting;

        Vector3 ChannelFactor = Vector3.One;
        float Displacement = 0.5f;

        Random random = new Random();

        Dictionary<Transform, Texture2D> solarBodies = new Dictionary<Transform, Texture2D>();

        public float loopduration = 1000f;
        public float pulseAmount = 0.1f;
        public float pulseOffset = 0.15f;
        public float pulseTiming = 0.002f;

        public MainGame() {
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

            base.Initialize();
            InputManager.Initialize();
            Time.Initialize();

            solTransform.LocalPosition = new Vector3(0, 0, 0);
            solTransform.LocalScale = Vector3.One * 7;

            mercuryTransform.LocalPosition = new Vector3(3, 0, 0);
            mercuryTransform.LocalScale = Vector3.One * 2f/7f;

            lunaTransform.LocalPosition = new Vector3(3, 0, 0);
            lunaTransform.LocalScale = Vector3.One * 1f/3f;

            earthTransform.LocalPosition = new Vector3(6, 0, 0);
            earthTransform.LocalScale = Vector3.One * 3f/7f;
            
            earthTransform.Parent = solTransform;
            mercuryTransform.Parent = solTransform;
            lunaTransform.Parent = earthTransform;

            camera = firstPerson;
            camera.Transform.LocalPosition = new Vector3(0, 0, 10);

            topDown.Transform.LocalPosition = new Vector3(0, 100, 0);
            topDown.Transform.Rotate(Vector3.Left, (float)Math.PI/2f);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sphereModel = Content.Load<Model>("Sphere");

            earthTexture = Content.Load<Texture2D>("earthTexture");
            //solTexture = Content.Load<Texture2D>("solTexture");
            lunaTexture = Content.Load<Texture2D>("lunaTexture");
            mercuryTexture = Content.Load<Texture2D>("mercuryTexture");

            // Special textures used for sun shader
            noise = Content.Load<Texture2D>("noise");
            ramp = Content.Load<Texture2D>("ramp");

            sunEffect = Content.Load<Effect>("DisplacementEffect");
            standardLighting = Content.Load<Effect>("StandardShading");

            Camera camera = new Camera();

            // Fill dictonary
            solarBodies.Add(earthTransform, earthTexture);
            solarBodies.Add(lunaTransform, lunaTexture);
            solarBodies.Add(mercuryTransform, mercuryTexture);

        }
       
        float rot = 2;
        float speed = 10;
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            if (InputManager.IsKeyPressed(Keys.Tab)) camera = (camera == firstPerson) ? topDown : firstPerson;

            if(camera == firstPerson) {
                if (InputManager.IsKeyDown(Keys.W)) firstPerson.Transform.LocalPosition += speed * firstPerson.Transform.Forward * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.S)) firstPerson.Transform.LocalPosition += speed * firstPerson.Transform.Backward * Time.ElapsedGameTime;

                if (InputManager.IsKeyDown(Keys.A)) firstPerson.Transform.Rotate(Vector3.Up, rot * Time.ElapsedGameTime);
                if (InputManager.IsKeyDown(Keys.D)) firstPerson.Transform.Rotate(Vector3.Down, rot * Time.ElapsedGameTime);

                if (InputManager.IsKeyDown(Keys.Up)) earthTransform.LocalPosition += Vector3.Forward;
                if (InputManager.IsKeyDown(Keys.Down)) earthTransform.LocalPosition += Vector3.Backward;
            }

            // Solar Body Rotations
            earthTransform.Rotate(Vector3.Up, rot * Time.ElapsedGameTime);
            solTransform.Rotate(Vector3.Up, rot * Time.ElapsedGameTime);
            mercuryTransform.Rotate(Vector3.Down, rot * Time.ElapsedGameTime); // Counter the rotation of Sol

            // For both cameras
            if (InputManager.IsKeyDown(Keys.Space)) camera.FieldOfView -= 0.05f;
            if (InputManager.IsKeyDown(Keys.LeftShift)) camera.FieldOfView += 0.05f;



            // Update shader values
            ChannelFactor = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            Displacement = 0.5f + (float)Math.Sin(Time.TotalGameTimeMilli) / 6f;


            float r = (float) (Math.Sin((Time.TotalGameTimeMilli / loopduration) * (2 * Math.PI)) * 0.25f + 0.25f);
            float g = (float) (Math.Sin((Time.TotalGameTimeMilli / loopduration + 0.33333333f) * 2 * Math.PI) * 0.25f + 0.25f);
            float b = (float) (Math.Sin((Time.TotalGameTimeMilli / loopduration + 0.66666667f) * 2 * Math.PI) * 0.25f + 0.25f);
            float correction = 1 / (r + g + b);

            r *= correction;
            g *= correction;
            b *= correction;
            ChannelFactor =  new Vector3(r, g, b);
            Displacement = (float) (pulseOffset + Math.Sin(Time.TotalGameTimeMilli * pulseTiming) * pulseAmount);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            // Standard Drawing
            /*sphereModel.Draw(solTransform.World, camera.View, camera.Projection);
            sphereModel.Draw(mercuryTransform.World, camera.View, camera.Projection);
            sphereModel.Draw(earthTransform.World, camera.View, camera.Projection);
            sphereModel.Draw(lunaTransform.World, camera.View, camera.Projection); */

            Matrix view = camera.View;
            Matrix projection = camera.Projection;

            // Set up lighting info 

            standardLighting.CurrentTechnique = standardLighting.Techniques[0]; 
            standardLighting.Parameters["View"].SetValue(view);
            standardLighting.Parameters["Projection"].SetValue(projection);
            standardLighting.Parameters["LightPosition"].SetValue(Vector3.Zero);
            standardLighting.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            standardLighting.Parameters["Shininess"].SetValue(20f);
            standardLighting.Parameters["AmbientColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            standardLighting.Parameters["SpecularColor"].SetValue(new Vector3(0.3f, 0.3f, 0.5f));

            // Iterate over models and draw with our shaders
            foreach(KeyValuePair<Transform, Texture2D> current in solarBodies) {
                standardLighting.Parameters["World"].SetValue(current.Key.World);
                standardLighting.Parameters["DiffuseTexture"].SetValue(current.Value);
                foreach (EffectPass pass in standardLighting.CurrentTechnique.Passes) {
                    pass.Apply();
                    foreach (ModelMesh mesh in sphereModel.Meshes)
                        foreach (ModelMeshPart part in mesh.MeshParts) {
                            GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                            GraphicsDevice.Indices = part.IndexBuffer;
                            GraphicsDevice.DrawIndexedPrimitives(
                                PrimitiveType.TriangleList, part.VertexOffset, 0,
                                part.NumVertices, part.StartIndex, part.PrimitiveCount);
                        }
                }
            }

            // Seperate shader for sun
            
            sunEffect.CurrentTechnique = sunEffect.Techniques[0];
            sunEffect.Parameters["View"].SetValue(view);
            sunEffect.Parameters["Projection"].SetValue(projection);
            sunEffect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);

            sunEffect.Parameters["World"].SetValue(solTransform.World);
            sunEffect.Parameters["DiffuseTexture"].SetValue(noise);

            sunEffect.Parameters["DispTex"].SetValue(noise);
            sunEffect.Parameters["RampTex"].SetValue(ramp);
            sunEffect.Parameters["ChannelFactor"].SetValue(ChannelFactor);
            sunEffect.Parameters["Displacement"].SetValue(Displacement);

            foreach (EffectPass pass in sunEffect.CurrentTechnique.Passes) {
                pass.Apply();
                foreach (ModelMesh mesh in sphereModel.Meshes)
                    foreach (ModelMeshPart part in mesh.MeshParts) {
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList, part.VertexOffset, 0,
                            part.NumVertices, part.StartIndex, part.PrimitiveCount);
                    }
            }

            base.Draw(gameTime);
        }
    }
}
