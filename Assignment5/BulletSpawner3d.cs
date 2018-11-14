using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CPI311.GameEngine;
using System.Threading;
using System.Diagnostics;

namespace Assignment5
{
    public class BulletSpawner3d : Behavior3d
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
        public Model bulletModel;

        public Color bulletTint = Color.White;
        
        public Vector3 scale;

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
                Vector3 direction = new Vector3( (float) Math.Cos(angle), (float) Math.Sin(angle), 0);
                
                // Spawn a new bullet
                BulletPoolObject3d b = BulletPoolObject3d.Initialize();

                // Init bullet 
                (b.GetBehavior<Bullet3d>() as Bullet3d).Init(bulletModel, bulletSpeed, direction, scale, bulletTint); //direction, 0, bulletSpeed, null, bulletSprite, bulletTint, true);

                // Set its position to this game objects ( or some offset )
                b.transform.LocalPosition = transform.Position;

                // Depth
                //b.sprite.LayerDepth = 0.5f;
            }

            Time.timers.Add(new EventTimer(SpawnLoop, bulletfrequency));
        }
    }
}
