using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CPI311.GameEngine;

namespace Assignment1
{
    class BulletPoolObject : GameObject2d
    {
        // We keep a specific pool of objects with the same behaviors so we don't need to changes things just set inactive 
        public static List<BulletPoolObject> pool = new List<BulletPoolObject>();

        // Preps the pool by newing up some objects at game start
        public static void PreLoadPool(int count) 
        {
            for(int i = 0; i < count; i++) pool.Add(new BulletPoolObject());
        }

        public BulletPoolObject() {
            addBehavior(new Bullet());
            addBehavior(new grazeEnemy());
        }

        new public static BulletPoolObject Initialize()
        {
            BulletPoolObject g;
            if (pool.Count > 0) {
                g = pool.FirstOrDefault();
                pool.Remove(g);
            }
            else 
                g = new BulletPoolObject();
            
            activeGameObjects.Add(g);

            return g;

        }

        public void Destroy()
        {
            foreach (Behavior2d behavior in behaviors) behavior.OnDestory();
            // Add back to pool and then remove from active list
            pool.Add(this);
            GameObject2d.activeGameObjects.Remove(this);
        }
    }
}
