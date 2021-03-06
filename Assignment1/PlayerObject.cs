﻿using System;
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

        #region static properties
        public static List<PlayerObject> players = new List<PlayerObject>();

        public const int maxPlayers = 4;

        public static int currentPlayers { get => players.Count(); }

        public static void removePlayer(int playerNumber) {
            if (playerNumber <= currentPlayers && playerNumber > 1) {
                players[playerNumber]?.Destroy();
                players.RemoveAt(playerNumber);
            }
        }
        #endregion

        public Job playerJob;
        public PlayerIndex controllingPlayer;
        public bool keyboard = false;

        public static PlayerObject CreatePlayer (Job job, PlayerIndex control, Vector2 position = new Vector2() )
        {
            if (currentPlayers >= maxPlayers) return null;
            PlayerObject p = new PlayerObject();

            // This isn't the best way to store it but whatev
            if ((int)control == 5) p.keyboard = true;
            p.controllingPlayer = control;
            p.playerJob = job;
            
            p.sprite = new AnimatedSprite(p.playerJob.animationSheet, 3, (int)job.dimensions.X, (int)job.dimensions.Y, true, 0);
            p.sprite.enableCam = true;
            p.sprite.Position = position;
            

            // This seems about right
            p.sprite.collisionBox = new Vector2(4, 4);
            activeGameObjects.Add(p);

            // Init Hitbox
            //GameObject2d h = GameObject2d.Initialize();
            //h.sprite = new AnimatedSprite(p.playerJob.hitboxSpriteSheet, 3, 8, 8, true, 0);
            //h.addBehavior(new AnchorPosBehavior(p.sprite));

            // Old Mouse
            /*
            GameObject2d mouse = GameObject2d.Initialize();
            mouse.sprite = new Sprite(mouseSprite);
            mouse.addBehavior(new AnchorMouseBehavior());
            mouse.sprite.enableCam = false;
            */

            // Init playerBehavior
            PlayerBehavior b = new PlayerBehavior();
            //b.hitbox = h;
            b.playerBullet = p.playerJob.bulletSprite;
            b.bulletRange = job.bulletRange;
            b.damage = job.bulletDamage; 
            p.addBehavior(b);

            players.Add( p );

            return p;
        }

    }
}
