using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CPI311.GameEngine;

namespace Assignment5 
{
    class BulletPoolObject3d : GameObject3d
    {
        // We keep a specific pool of objects with the same behaviors so we don't need to changes things just set inactive 
        public static List<BulletPoolObject3d> pool = new List<BulletPoolObject3d>();

        // Preps the pool by newing up some objects at game start
        public static void PreLoadPool(int count) 
        {
            for(int i = 0; i < count; i++) pool.Add(new BulletPoolObject3d());
        }

        public BulletPoolObject3d() {
            addBehavior(new Bullet3d());
            //addBehavior(new grazeEnemy());
        }

        new public static BulletPoolObject3d Initialize()
        {
            BulletPoolObject3d g;
            if (pool.Count > 0) {
                g = pool.FirstOrDefault();
                pool.Remove(g);
            }
            else 
                g = new BulletPoolObject3d();
            
            activeGameObjects.Add(g);

            return g;

        }

        public override void Destroy()
        {
            foreach (Behavior3d behavior in behaviors) behavior.OnDestory();
            // Add back to pool and then remove from active list
            pool.Add(this);
            activeGameObjects.Remove(this);
        }
    }
}
