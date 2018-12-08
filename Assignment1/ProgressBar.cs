using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment1 {
    class ProgressBar : GameObject2d {
        public Sprite innerSprite;

        public float value; //between 0 and 1

        public override void Render(dynamic Renderer) {

            base.Render(Renderer as SpriteBatch);
            (Renderer as SpriteBatch).Draw(texture: innerSprite.Texture, position: sprite.Position,
                sourceRectangle: new Rectangle(0,0, (int) ( innerSprite.Width * (value)) , (int) innerSprite.Height ) , color: innerSprite.Color, 
                rotation: innerSprite.Rotation, origin: innerSprite.Origin, scale: innerSprite.Scale, effects: innerSprite.Effects, layerDepth: 0.1f);
            
        }

        new public static ProgressBar Initialize () {
            ProgressBar b = new ProgressBar();
            b.sprite.LayerDepth = 0.1f;
            activeGameObjects.Add(b);
            return b;
        }
    }
}
