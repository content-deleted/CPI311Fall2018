﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CPI311.GameEngine;
using Microsoft.Xna.Framework;

namespace Final {
    class hoopObject : GameObject3d {
        Random ran = new Random();
        // We keep a specific pool of objects with the same behaviors so we don't need to changes things just set inactive 
        public static List<hoopObject> pool = new List<hoopObject>();

        // Preps the pool by newing up some objects at game start
        public static void PreLoadPool(int count) {
            for (int i = 0; i < count; i++) pool.Add(new hoopObject());
        }

        public static Vector3 lastPos;

        public hoopObject() {
            getNextHoopPos();
            material = new Hoop(5f, 7f, 1f, 10);
            addBehavior(new hoopControl());
            addBehavior(new hoopLogic());
        }

        public void getNextHoopPos () {
            transform.LocalPosition = lastPos + new Vector3(ran.Next(-5, 5), ran.Next(-5, 5), 100);
            lastPos = transform.LocalPosition;
        }

        new public static hoopObject Initialize() {
            hoopObject g;
            if (pool.Count > 0) {
                g = pool.FirstOrDefault();
                g.getNextHoopPos(); // MAKE SURE TO UPDATE THE POS OF A HOOP WE REMOVE FROM POOL
                pool.Remove(g);
            }
            else
                g = new hoopObject();

            activeGameObjects.Add(g);
            g.Start();

            return g;
        }

        public override void Destroy() {
            foreach (Behavior3d behavior in behaviors) behavior.OnDestory();
            // Add back to pool and then remove from active list
            pool.Add(this);
            activeGameObjects.Remove(this);
        }
    }
}