using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;

/*
 * Unused animation implementation
 * Decided to extend sprite instead
 */

namespace CPI311.GameEngine {
    public class Animation2d : Behavior2d {
        private Sprite animSprite;

        public Texture2D[] frames;
        public bool loop = true;

        private int curFrame;

        public Animation2d (Texture2D[] animation, bool repeat) {
            frames = animation;
            loop = repeat;
        }

        public override void Start() {
            base.Start();
            animSprite = objSprite;
        }

        public override void Update() {
            base.Update();

            curFrame++;

            // Check if animation is over and loop or end
            if (curFrame > frames.Count())
                if (loop) curFrame = 0;
                else release();

            // Assign current frame
            objSprite.Texture = frames[curFrame];
        }

    }
}
