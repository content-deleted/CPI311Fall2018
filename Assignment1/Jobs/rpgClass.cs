﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Assignment1 {
    public class Job {
        // Menu
        public String name;
        public Texture2D portrait;
        public String description;

        // Gameplay
        public Texture2D animationSheet;
        public Texture2D hitboxSpriteSheet;
        public Texture2D bulletSprite;

    }

    /*
     * Contains static data for jobs and some other assets for dealing with them
     */

    public static class JobInfo {
        public static int JobCount = 1;

        public enum jobType {
            Knight,
            //Magician,
            //Thief,
            //Untitled
        }

        #region contentLoad
        public static Job [] Jobs = new Job[]
        {
            new Job{ name = "Knight", description = "Defense"},
            //new charecterMenuData{ name = "Magician", description = "Range"},
            //new charecterMenuData{ name = "Thief", description = "Offense"},
            //new charecterMenuData{ name = "Untitled", description = "Placeholder"} 
        };

        public static Texture2D portraitBG;

        public static void LoadContent(ContentManager content) {
            foreach(Job j in Jobs) {
                j.portrait = content.Load<Texture2D>(j.name + "Portrait");
                j.animationSheet = content.Load<Texture2D>(j.name + "SpriteSheet");
                j.hitboxSpriteSheet = content.Load<Texture2D>("hitbox"); // might give them unique ones later
                j.bulletSprite = content.Load<Texture2D>(j.name + "Bullet");
            }
            
            portraitBG = content.Load<Texture2D>("PladBG");
        }
        #endregion


    }
}