using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPI311.GameEngine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment5
{
    class Bullet3d : Behavior3d
    {
        float speed;
        Vector2 direction;
        //bool facing = false;

        public void Init (Model bulletModel, float spd, Vector2 dir, Vector3 scale, Color c)
        {
            (obj as GameObject3d).mesh = bulletModel;
            transform.LocalScale = scale;
            //objSprite.Color = c;
            speed = spd;
            direction = dir;
            //if (facing) objSprite.Rotation = (float)Math.Atan2(direction.Y, direction.X);
        }
        float PlayfieldSizeX = 50;
        float PlayfieldSizeY = 50;

        override public void Update ()
        {
            Vector2 mov = direction * speed;
            transform.LocalPosition += new Vector3(mov.X, mov.Y, 0);
            //if (facing) objSprite.Rotation = (float) Math.Atan2(direction.Y, direction.X);
            Vector2 position = new Vector2 (transform.LocalPosition.X, transform.LocalPosition.Y) ;
            if (position.X > PlayfieldSizeX || position.X < -PlayfieldSizeX ||
                position.Y > PlayfieldSizeY || position.Y < -PlayfieldSizeY)
                    (obj as BulletPoolObject3d).Destroy();

        }
    }
}
