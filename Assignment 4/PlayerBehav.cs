using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment_4 {
    public class PlayerBehav : Behavior3d {

        float speed = 0.5f;

        float threshold = 0.5f;

        public TerrainRenderer terrain;

        public override void Start() {
            base.Start();
        }

        public override void Update() {
            checkPlayerMovement();
        }

        public PlayerBehav(TerrainRenderer r) => terrain = r;

        Vector3 dir = new Vector3 (1, 0 , 0);

        public void checkPlayerMovement() {
            // Movement Controls
            Vector3 movHor = Vector3.Zero, movVert = Vector3.Zero;
            
            if (InputManager.IsKeyDown(Keys.A)) movHor += Vector3.Left* speed;

            if (InputManager.IsKeyDown(Keys.D)) movHor += Vector3.Right * speed;

            if (InputManager.IsKeyDown(Keys.W)) movVert += Vector3.Forward * speed;

            if (InputManager.IsKeyDown(Keys.S)) movVert += Vector3.Backward * speed;

            if (InputManager.IsKeyPressed(Keys.Space)) transform.LocalPosition = Vector3.Zero;


            if (terrain.GetAltitude(transform.LocalPosition + movHor) < threshold ) 
                transform.LocalPosition += movHor;

            if (terrain.GetAltitude(transform.LocalPosition + movVert) < threshold)
                transform.LocalPosition += movVert;

            transform.LocalPosition = new Vector3(transform.LocalPosition.X, terrain.GetAltitude(transform.LocalPosition), transform.LocalPosition.Z);
        }
    }
}