using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CPI311.GameEngine;
using Microsoft.Xna.Framework;

namespace Assignment1
{
    public class grazeEnemy : Behavior2d
    {
        public static UInt64 grazeScore;

        float grazeDistance = 30; // Come up with a better distance later;

        //private PlayerBehavior playerB;

        public override void Start()
        {
            base.Start();
            //playerB = PlayerObject.singleton.GetBehavior<PlayerBehavior>() as PlayerBehavior;
        }

        override public void Update () {
            // Check distance from Player
            float dis = Vector2.Distance(objSprite.Position, PlayerObject.singleton.sprite.Position);

            // If less than certain distance check if bounding box 
            if(dis <= grazeDistance)
            {
                if ( Sprite.intersects(PlayerObject.singleton.sprite, objSprite) )
                    // IF bounding box true then call player death method
                    (PlayerObject.singleton.GetBehavior<PlayerBehavior>() as PlayerBehavior).onHit();
                else
                    // IF false increment graze
                    grazeScore++; // This might have some other mod as well
            }
        }
    }
}
