﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Squared.Tiled;
using GameStateManagement;

//
// Summary:
//     Submit a sprite for drawing in the current batch.
//
// Parameters:
//   texture:
//     A texture.
//
//   position:
//     The drawing location on screen.
//
//   sourceRectangle:
//     An optional region on the texture which will be rendered. If null - draws full
//     texture.
//
//   color:
//     A color mask.
//
//   rotation:
//     A rotation of this sprite.
//
//   origin:
//     Center of the rotation. 0,0 by default.
//
//   scale:
//     A scaling of this sprite.
//
//   effects:
//     Modificators for drawing. Can be combined.
//
//   layerDepth:
//     A depth of the layer of this sprite.

namespace CPI311.GameEngine
{
    public class Sprite {
        // this is a bad design choice
        public static Map currentMap;
        public static Vector2 cameraPosition;
        public bool enableCam = true;
        public Texture2D Texture { get; set; }
        public bool solid = true; // THIS CONTROLS TILE COLLISION NOT ENTITY 
        private Vector2 position;
        public Vector2 Position {
            get => position;
            set => position = value;
        }

        public void move(Vector2 dir) {
            // check collides with tile
            Vector2 mov = dir;

            if (currentMap != null && solid) {
                Layer col = currentMap.Layers["COLLISION"];
                int w = currentMap.TileWidth;
                int h = currentMap.TileHeight;
                int tilePosX = (int)((position.X + 640) / w);
                int tilePosY = (int)((position.Y + 360) / h);
                int nexttilePosX = (int)((position.X + dir.X + 640) / w); 
                int nexttilePosY = (int)((position.Y + dir.Y + 360) / h); 
                if (tilePosX <= currentMap.Width && tilePosX >= 0 && tilePosY <= currentMap.Height && tilePosY >= 0) {
                    // x
                    if (col.GetTile(nexttilePosX, tilePosY) != 0) {
                        mov.X = 0;
                    }
                    // y
                    if (col.GetTile(tilePosX, nexttilePosY) != 0) {
                        mov.Y = 0;
                    }
                }
            }
            position += mov;
        }

        public Vector2 CameraSpacePosition { get => (enableCam) ? Position - cameraPosition : Position; }
        public Rectangle SourceRectangle { get; set; }
        public virtual float Height { get => SourceRectangle.Height; }
        public virtual float Width { get => SourceRectangle.Width; }

        //public Rectangle collisionRect { get => new Rectangle((int)(Position.X), (int)(Position.Y), (int)(Width * Scale.X), (int)(Height * Scale.Y)); }
        public Vector2 collisionBox;
        
        public Vector2? Origin { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }
        public Color Color { get; set; }
        public SpriteEffects Effects { get; set; }
        public float LayerDepth { get; set; }

        public bool visible = true;

        // Square collision 
        /*public static bool intersects(Sprite a, Sprite b) {
            float aw = (a.collisionBox.X * a.Scale.X) / 2;
            float ah = (a.collisionBox.Y * a.Scale.Y) / 2;

            float bw = (b.collisionBox.X * b.Scale.X) / 2;
            float bh = (b.collisionBox.Y * b.Scale.Y) / 2;

            return
            ((b.Position.X + bw) >= a.Position.X - aw && b.Position.X - bw <= (a.Position.X + aw))
            && ((b.Position.Y + bh) >= a.Position.Y - ah && b.Position.Y - bh <= (a.Position.Y + ah));
        }*/

        public static bool intersects(Sprite a, Sprite b) => (a.collisionBox.X * a.Scale.X) / 2 + (b.collisionBox.X * b.Scale.X) / 2 > Vector2.Distance(a.Position, b.Position);

        public Sprite(Texture2D texture)
        {
            Texture = texture;
            Position = Vector2.Zero;
            SourceRectangle = (texture != null ) ? new Rectangle(0, 0, Texture.Width, Texture.Height) : new Rectangle();
            Origin = (texture != null) ?  new Vector2(Texture.Width / 2, Texture.Height / 2) : Vector2.Zero;
            Rotation = 0;
            Scale = Vector2.One;
            Color = Color.White;
            Effects = SpriteEffects.None;
            LayerDepth = 0;

            // Default to height and width being our collision box
            collisionBox.X = Width;
            collisionBox.Y = Height;
        }

        public virtual void Update() {   }

        public virtual void Draw(SpriteBatch s)
        {
            if (visible) {
                s.Draw(texture: Texture, position: Position - ((enableCam) ? cameraPosition : Vector2.Zero), sourceRectangle: SourceRectangle, color: Color, rotation: Rotation, origin: Origin, scale: Scale, effects: Effects, layerDepth: LayerDepth);
            }
        }

        public static bool onScreen() {
            return true;
        }
    }
}
