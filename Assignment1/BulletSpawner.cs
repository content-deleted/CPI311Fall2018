using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CPI311.GameEngine;
using System.Threading;
using System.Diagnostics;

namespace Assignment1
{
    public class BulletSpawner : Behavior2d
    {
        const float tau = 2 * (float)Math.PI;
            
        public float bulletSpeed;
        public int bulletAmount;

        public float bulletArc = tau;
        public float ArcOffset = 0;

        private float currentOffset;
        public float bulletfrequency = 0.15f; // ms 

        public float spin;
        private float wave;

        public float Wave { get=> wave; set { counter = 0; wave = value; } }

        private float counter;
        public Texture2D bulletSprite;

        public Color bulletTint = Color.White;
        
        public Vector2 scale;

        private bool spawn = true;

        public override void Update()
        {
            counter += wave;
            currentOffset += spin * (float) Math.Cos(counter);
            currentOffset %= bulletArc;
        }

        public override void Start() => Time.timers.Add(new EventTimer(SpawnLoop, bulletfrequency));

        public override void OnDestory() {
            base.OnDestory();
            spawn = false;
        }

        private void SpawnLoop ()
        {
            if (!spawn) return;
            for (int i = 0; i < bulletAmount; i++)
            {
                float angle = ArcOffset + (bulletArc / bulletAmount) * i - bulletArc / 4 + currentOffset;
                Vector2 direction = new Vector2( (float) Math.Cos(angle), (float) Math.Sin(angle));

                // Spawn a new bullet
                BulletPoolObject b = BulletPoolObject.Initialize();

                // Init bullet 
                (b.GetBehavior<Bullet>() as Bullet).Init(bulletSprite, bulletSpeed, direction, scale, bulletTint); //direction, 0, bulletSpeed, null, bulletSprite, bulletTint, true);

                // Set its position to this game objects ( or some offset )
                b.sprite.Position = objSprite.Position;

                // Depth
                b.sprite.LayerDepth = 0.5f;
            }

            Time.timers.Add(new EventTimer(SpawnLoop, bulletfrequency));
        }
    }
}
