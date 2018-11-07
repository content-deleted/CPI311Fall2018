using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CPI311.GameEngine;
using System.Diagnostics;

namespace Assignment1
{
    public class PlayerBehavior : Behavior2d
    {
        int lives = 5;
        float speed = 4f;
        bool invicible = false;

        public Texture2D playerBullet;

        public GameObject2d hitbox;

        public override void Start ()
        {
            
        }

        public override void Update ()
        {
            if (objSprite.visible == false) return; // Gross short circuit

            checkPlayerMovement();

            checkPlayerFire();

            hitbox.sprite.Update();
        }

        public void checkPlayerMovement () {
            // Movement Controls
            Vector2 mov = Vector2.Zero;

            // Check if shift and adjust speed
            if (InputManager.IsKeyDown(Keys.LeftShift)) {
                speed = 4;
                (objSprite as AnimatedSprite).animationSpeed = 0.15F;
            }
            else {
                speed = 6;
                (objSprite as AnimatedSprite).animationSpeed = 0.2F;
            }

            if (InputManager.IsKeyDown(Keys.A))  mov.X -= 1;

            if (InputManager.IsKeyDown(Keys.D)) mov.X += 1;

            if (InputManager.IsKeyDown(Keys.W))  mov.Y -= 1;
            
            if (InputManager.IsKeyDown(Keys.S))  mov.Y += 1; 


            if(mov.X != 0) {
                if(mov.X < 0) (objSprite as AnimatedSprite).swapAnimation(16, 8);
                else (objSprite as AnimatedSprite).swapAnimation(24, 8);
            }
            else if (mov.Y != 0) {
                if (mov.Y < 0) (objSprite as AnimatedSprite).swapAnimation(0, 8);
                else (objSprite as AnimatedSprite).swapAnimation(8, 8);
            }

            if (mov != Vector2.Zero) {
                objSprite.Update();
                mov.Normalize();
                objSprite.Position += mov * speed;
            }
        }

        public void checkPlayerFire () {
            MouseState state = Mouse.GetState();
            if(state.LeftButton == ButtonState.Pressed) {
                // Get angle
                Vector2 direction = new Vector2(state.Position.X - objSprite.CameraSpacePosition.X, state.Position.Y - objSprite.CameraSpacePosition.Y);
                direction.Normalize();

                // Spawn a new bullet
                GameObject2d b = GameObject2d.Initialize();
               
                PlayerBullet behav = new PlayerBullet();
                b.addBehavior(behav);
                behav.facing = true;
                behav.Init(playerBullet, 20, direction, Vector2.One);

                // Set its position to this game objects ( or some offset )
                b.sprite.Position = objSprite.Position + direction * 25;
            }
        }

        public void onHit()
        {
            if (invicible) return; // Gross short circuit
            // Set sprite to invisible 
            objSprite.visible = false;
            hitbox.sprite.visible = false;

            // Start counter
            Time.timers.Add(new EventTimer(respawnPlayer, 0.5f));

            // Reduce lives
            invicible = true;
            lives--;
        }

        public void respawnPlayer() {
            objSprite.visible = true;
            hitbox.sprite.visible = true;
            // Probably start a blinking thing and return player to middle of screen
            objSprite.Color = Color.Red;
            Time.timers.Add(new EventTimer(endIframes, 2));
        }

        public void endIframes() {
            invicible = false;
            objSprite.Color = Color.White;
        }
    }
}
