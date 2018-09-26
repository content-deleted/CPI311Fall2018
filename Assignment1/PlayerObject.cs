using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CPI311.GameEngine;

namespace Assignment1
{
    public class PlayerObject : GameObject2d
    {
        public static PlayerObject singleton;
        
        public static void CreatePlayer (Vector2 position, Texture2D animationSheet, Texture2D hitboxSpriteSheet, Texture2D bulletSprite)
        {
            PlayerObject p = new PlayerObject();
            
            p.sprite = new AnimatedSprite(animationSheet, 8, 32, 32, true, 0);
            p.sprite.Position = position;

            // This seems about right
            p.sprite.collisionBox = new Vector2(8, 8);
            activeGameObjects.Add(p);
            singleton = p;

            // Init Hitbox
            GameObject2d h = GameObject2d.Initialize();
            h.sprite = new AnimatedSprite(hitboxSpriteSheet, 3, 8, 8, true, 0);
            h.addBehavior(new AnchorPosBehavior());

            // Init playerBehavior
            PlayerBehavior b = new PlayerBehavior();
            b.hitbox = h;
            b.playerBullet = bulletSprite;

            singleton.addBehavior(b);

        }

    }
}
