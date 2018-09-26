using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace CPI311.GameEngine {

    public class AnimatedSprite : Sprite {

        // Animation stuff
        public int startFrame = 0;
        public float curFrame = 0;
        public int frameCount;
        public bool loop = true;

        private int frameHeight;
        private int frameWidth;

        public override float Height { get => frameHeight; }
        public override float Width { get => frameWidth; }

        public float animationSpeed = 0.1f;
        
        public AnimatedSprite(Texture2D texture, int frames, int height, int width, bool repeat, int start) : base (texture) {
            loop = repeat;
            Texture = texture;
            Position = Vector2.Zero;
            SourceRectangle = new Rectangle(0, 0, width, height);
            Origin = new Vector2(width / 2, height / 2);

            startFrame = start;
            frameCount = frames;
            frameHeight = height;
            frameWidth = width;

            // Default to height and width being our collision box
            collisionBox.X = width;
            collisionBox.Y = height;
        }
        public void swapAnimation (int start, int frames) {
            if(startFrame != start) {
                startFrame = start;
                frameCount = frames;
                curFrame = start;
            }
        }
        override public void Update() {
            curFrame+= animationSpeed;

            // Check if animation is over and loop or end
            if (curFrame > (frameCount + startFrame))
                if (loop) curFrame = startFrame;

            // else end animation?

            // Assign current frame
            int spritSheetX = (int)curFrame * frameWidth;
            int xPos = (spritSheetX) % Texture.Width;
            int yPos = (spritSheetX) / Texture.Width;
            SourceRectangle = new Rectangle(xPos, yPos * frameHeight, frameWidth, frameHeight);
        }
    }
}
