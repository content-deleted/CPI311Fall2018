using System;
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

        // Stats
        public Vector2 dimensions;
        public float bulletRange;
        public float bulletDamage;

    }

    /*
     * Contains static data for jobs and some other assets for dealing with them
     */

    public static class JobInfo {
        public static int JobCount = 2;

        public enum jobType {
            Knight,
            Magician,
            //Thief,
            //Untitled
        }

        #region contentLoad
        public static Job [] Jobs = new Job[]
        {
            new Job{ name = "Knight", description = "Defense", dimensions = new Vector2 (16,18), bulletDamage = 10, bulletRange = 5},
            new Job{ name = "Magician", description = "Range", dimensions = new Vector2 (16,22), bulletDamage = 1, bulletRange = 200},
            //new charecterMenuData{ name = "Thief", description = "Offense"},
            //new charecterMenuData{ name = "Untitled", description = "Placeholder"} 
        };

        public static Texture2D portraitBG;
        public static Texture2D ready;

        public static void LoadContent(ContentManager content) {
            foreach(Job j in Jobs) {
                j.portrait = content.Load<Texture2D>(j.name + "Portrait");
                j.animationSheet = content.Load<Texture2D>(j.name + "SpriteSheet");
                j.hitboxSpriteSheet = content.Load<Texture2D>("hitbox"); // might give them unique ones later
                j.bulletSprite = content.Load<Texture2D>(j.name + "Bullet");
            }
            
            portraitBG = content.Load<Texture2D>("PladBG");
            ready = content.Load<Texture2D>("ready");
        }
        #endregion


    }
}