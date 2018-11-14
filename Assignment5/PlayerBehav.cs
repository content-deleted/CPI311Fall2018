using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment5 {
    class PlayerBehav : Behavior3d {

        float speed = 0.5f;

        Rigidbody r;

        public override void Start() {
            base.Start();
            r = obj.GetBehavior<Rigidbody>() as Rigidbody;
        }

        public override void Update() {
            checkPlayerMovement();
            checkFire();

            r.Velocity *= 0.97f;
        }

        Vector3 dir = new Vector3 (1, 0 , 0);

        public void checkPlayerMovement() {
            // Movement Controls
            Vector2 mov = Vector2.Zero;


            if (InputManager.IsKeyDown(Keys.A)) transform.Rotate(Vector3.Up, 0.04f);

            if (InputManager.IsKeyDown(Keys.D)) transform.Rotate(Vector3.Up, -0.04f);

            if (InputManager.IsKeyDown(Keys.W)) r.Impulse += transform.Right * 200;

            if (InputManager.IsKeyDown(Keys.S)) r.Impulse -= transform.Right * 200;

            if (InputManager.IsKeyPressed(Keys.Space)) transform.LocalPosition = Vector3.Zero;


            if (mov != Vector2.Zero) {
                // partical effect

                mov.Normalize();
                mov *= speed;
                transform.LocalPosition += new Vector3(mov.X, mov.Y, 0);

                dir = new Vector3(mov.X, mov.Y, 0);
            }
        }
        float bulletSpeed = 1.5f;

        public static Model bulletMesh;

        public void checkFire() {
            if (InputManager.IsMouseDown()) {
                BulletPoolObject3d g = BulletPoolObject3d.Initialize();
                
                g.transform.LocalPosition = transform.LocalPosition;

                (g.GetBehavior<Bullet3d>() as Bullet3d).Init(bulletMesh, bulletSpeed, transform.Right, Vector3.One, Color.White, false);
                

                GameConstants.score -= GameConstants.ShotPenalty;
                // sound
                MainGameScreen.soundInstance = MainGameScreen.gunSound.CreateInstance();
                MainGameScreen.soundInstance.Play();
            }
        }
    }
}