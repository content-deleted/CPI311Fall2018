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
        public float damage;
        public float bulletRange;

        public Texture2D playerBullet;

        public GameObject2d hitbox;

        public PlayerObject player { get => obj as PlayerObject;  }

        public override void Start ()
        {
            
        }

        private Vector2 lastFireDir = new Vector2(0, 1);

        public override void Update ()
        {
            if (objSprite.visible == false) return; // Gross short circuit

            checkPlayerMovement();

            if (player.keyboard) {
                MouseState state = Mouse.GetState();
                if (state.LeftButton == ButtonState.Pressed) {
                    Vector2 direction = new Vector2(state.Position.X - objSprite.CameraSpacePosition.X, state.Position.Y - objSprite.CameraSpacePosition.Y);
                    PlayerFire(direction);
                }
            }
            else {
                GamePadState state = GamePad.GetState(player.controllingPlayer);
                if (state.IsButtonDown(Buttons.LeftShoulder)) {
                    Vector2 direction = new Vector2(state.ThumbSticks.Right.X, -state.ThumbSticks.Right.Y);
                    PlayerFire( (direction == Vector2.Zero) ? lastFireDir : direction);
                    if (direction != Vector2.Zero) lastFireDir = direction;
                }
            }

            //hitbox.sprite.Update();
        }

        const int walk = 2, run = 4;

        public void checkPlayerMovement() {
            // Movement Controls
            Vector2 mov = Vector2.Zero;

            // G A M E P A D
            if (!player.keyboard) {
                GamePadState state = GamePad.GetState(player.controllingPlayer);
                // Check if running
                if (state.IsButtonDown(Buttons.RightShoulder)) {
                    speed = walk;
                    (objSprite as AnimatedSprite).animationSpeed = 0.15F;
                }
                else {
                    speed = run;
                    (objSprite as AnimatedSprite).animationSpeed = 0.2F;
                }

                mov = state.ThumbSticks.Left;
                // invert y;
                mov.Y = -mov.Y;
            }
            else {
                // Check if shift and adjust speed
                if (InputManager.IsKeyDown(Keys.LeftShift)) {
                    speed = walk;
                    (objSprite as AnimatedSprite).animationSpeed = 0.15F;
                }
                else {
                    speed = run;
                    (objSprite as AnimatedSprite).animationSpeed = 0.2F;
                }

                if (InputManager.IsKeyDown(Keys.A)) mov.X -= 1;

                if (InputManager.IsKeyDown(Keys.D)) mov.X += 1;

                if (InputManager.IsKeyDown(Keys.W)) mov.Y -= 1;

                if (InputManager.IsKeyDown(Keys.S)) mov.Y += 1;
            }

            if (mov.X != 0) {
                if (mov.X < 0) (objSprite as AnimatedSprite).swapAnimation(9, 3);
                else (objSprite as AnimatedSprite).swapAnimation(3, 3);
            }
            else if (mov.Y != 0) {
                if (mov.Y < 0) (objSprite as AnimatedSprite).swapAnimation(0, 3);
                else (objSprite as AnimatedSprite).swapAnimation(6, 3);
            }

            if (mov != Vector2.Zero) {
                objSprite.Update();
                mov.Normalize();
                objSprite.move( mov * speed );
            }
        }

        public void PlayerFire(Vector2 direction) {
            direction.Normalize();

            // Spawn a new bullet
            GameObject2d b = GameObject2d.Initialize();
               
            PlayerBullet behav = new PlayerBullet();
            b.addBehavior(behav);
            behav.facing = true;
            behav.Init(playerBullet, 20, direction, Vector2.One, damage, bulletRange);

            // Set its position to this game objects ( or some offset )
            b.sprite.Position = objSprite.Position + direction * 25;
            
        }

        public void onHit()
        {
            if (invicible) return; // Gross short circuit
            // Set sprite to invisible 
            objSprite.visible = false;
            //hitbox.sprite.visible = false;

            // Start counter
            Time.timers.Add(new EventTimer(respawnPlayer, 0.5f));

            // Reduce lives
            invicible = true;
            lives--;
        }

        public void respawnPlayer() {
            objSprite.visible = true;
            //hitbox.sprite.visible = true;
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
