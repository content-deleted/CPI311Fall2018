using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment5 {
    class AsteroidObject : GameObject3d {
        public static Model AstroidModel;

        // We keep a specific pool of objects with the same behaviors so we don't need to changes things just set inactive 
        public static List<AsteroidObject> pool = new List<AsteroidObject>();

        // Preps the pool by newing up some objects at game start
        public static void PreLoadPool(int count) {
            for (int i = 0; i < count; i++) pool.Add(new AsteroidObject());
        }

        public AsteroidObject() {
            SphereCollider s = new SphereCollider();
            s.Radius = 3;
            addBehavior(s);
            addBehavior(new Astroid());
            mesh = AstroidModel;
            transform.LocalScale = Vector3.One * 3;
        }

        new public static AsteroidObject Initialize() {
            AsteroidObject g;
            if (pool.Count > 0) {
                g = pool.FirstOrDefault();
                pool.Remove(g);
                SphereCollider s = new SphereCollider();
                s.Radius = 3;
                g.addBehavior(s);
                
            }
            else {
                g = new AsteroidObject();
            }
            g.Start();
            activeGameObjects.Add(g);

            return g;
        }
        
        public static void initMany (int count) {
            for (int i = 0; i < count; i++) {
                AsteroidObject g = Initialize();
                g.transform.LocalPosition = new Vector3( MainGameScreen.random.Next( -50, 50), MainGameScreen.random.Next( -50, 50), 0);
            }
        }

        public override void Destroy() {
            foreach (Behavior3d behavior in behaviors) behavior.OnDestory();
            // Add back to pool and then remove from active list
            pool.Add(this);
            activeGameObjects.Remove(this);


            releaseBehavior(GetBehavior<SphereCollider>());
        }
    }
    class Astroid : Behavior3d {

        Collider c;
        public override void Start() {
            c = obj.GetBehavior<Collider>() as Collider;
        }

        public override void LateUpdate() {
            if (c.collidedThisFrame) (obj as AsteroidObject).Destroy();
        }

        public override void OnDestory() {
            // Spawn particle effect
            Particle particle = MainGameScreen.particleManager.getNext();
            particle.Position = transform.Position;
            particle.Velocity = new  Vector3(  MainGameScreen.random.Next( -5, 5), 2, MainGameScreen.random.Next( -50, 50));
            particle.Acceleration = new Vector3(0, 3, 0);
            particle.MaxAge = MainGameScreen.random.Next(1, 6);
            particle.Init();

            c.collidedThisFrame = false;
            c = null;

            base.OnDestory();
        }

    }

}
