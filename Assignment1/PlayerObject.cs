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
        public static List<PlayerObject> players = new List<PlayerObject>();

        public const int maxPlayers = 4;

        public static int currentPlayers=0;

        public static void removePlayer(int playerNumber) {
            if(playerNumber<=currentPlayers && playerNumber >1) players[playerNumber]?.Destroy();
        }

        #region tempSpriteData
        public static Texture2D animationSheet;
        public static Texture2D hitboxSpriteSheet;
        public static Texture2D bulletSprite;

        #endregion

        public static PlayerObject CreatePlayer ( Vector2 position = new Vector2() )
        {
            if (currentPlayers >= maxPlayers) return null;
            PlayerObject p = new PlayerObject();
            
            p.sprite = new AnimatedSprite(animationSheet, 8, 32, 32, true, 0);
            p.sprite.enableCam = true;
            p.sprite.Position = position;

            // This seems about right
            p.sprite.collisionBox = new Vector2(4, 4);
            activeGameObjects.Add(p);

            // Init Hitbox
            GameObject2d h = GameObject2d.Initialize();
            h.sprite = new AnimatedSprite(hitboxSpriteSheet, 3, 8, 8, true, 0);
            h.addBehavior(new AnchorPosBehavior(p.sprite));

            // Old Mouse
            /*
            GameObject2d mouse = GameObject2d.Initialize();
            mouse.sprite = new Sprite(mouseSprite);
            mouse.addBehavior(new AnchorMouseBehavior());
            mouse.sprite.enableCam = false;
            */

            // Init playerBehavior
            PlayerBehavior b = new PlayerBehavior();
            b.hitbox = h;
            b.playerBullet = bulletSprite;

            p.addBehavior(b);

            players.Add( p );

            return p;
        }

    }
}
