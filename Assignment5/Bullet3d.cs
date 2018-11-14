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
        Vector3 direction;

        bool isPoolable = true;
        //bool facing = false;

        public void Init (Model bulletModel, float spd, Vector3 dir, Vector3 scale, Color c, bool poolable = true)
        {
            isPoolable = poolable;
            (obj as GameObject3d).mesh = bulletModel;
            transform.LocalScale = scale;
            //objSprite.Color = c;
            speed = spd;
            direction = Vector3.Normalize( dir );
            //if (facing) objSprite.Rotation = (float)Math.Atan2(direction.Y, direction.X);
        }
        float PlayfieldSizeX = GameConstants.PlayfieldSizeX;
        float PlayfieldSizeY = GameConstants.PlayfieldSizeY;

        override public void Update ()
        {
            transform.LocalPosition += direction * speed;
            //if (facing) objSprite.Rotation = (float) Math.Atan2(direction.Y, direction.X);
            Vector2 position = new Vector2 (transform.LocalPosition.X, transform.LocalPosition.Y) ;
            if (position.X > PlayfieldSizeX || position.X < -PlayfieldSizeX ||
                position.Y > PlayfieldSizeY || position.Y < -PlayfieldSizeY)
                (obj as BulletPoolObject3d).Destroy();

            GameObject3d.activeGameObjects.Where(g => g.HasBehavior<Astroid>()).Where(g=> Vector3.Distance(g.transform.Position, transform.Position) < 4).ToList().ForEach(x => x.Destroy());

        }
    }
}
