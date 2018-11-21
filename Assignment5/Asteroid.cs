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
        public static Texture2D tex;

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
            addBehavior(new Rigidbody());
            mesh = AstroidModel;
            transform.LocalScale = Vector3.One * 3;
            StandardLightingMaterial m = new StandardLightingMaterial();
            m.texture = tex;
            m.ambientColor = Vector3.Zero;
            m.useTexture = true;
            material = m;
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
                g.transform.LocalPosition = new Vector3( MainGameScreen.random.Next( -75, 75), MainGameScreen.random.Next( -50, 50), 0);
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
        //Rigidbody r;
        public override void Start() {
            c = obj.GetBehavior<Collider>() as Collider;
            (obj.GetBehavior<Rigidbody>() as Rigidbody).Impulse += new Vector3(MainGameScreen.random.Next(-5, 5),  MainGameScreen.random.Next(-5, 5), 0)*2;
        }

        public override void LateUpdate() {
            // if (c.collidedThisFrame) (obj as AsteroidObject).Destroy();
            float x = transform.LocalPosition.X, y = transform.LocalPosition.Y;
            if (transform.LocalPosition.X > GameConstants.PlayfieldSizeX) x = -GameConstants.PlayfieldSizeX;
            if (transform.LocalPosition.X < -GameConstants.PlayfieldSizeX) x = GameConstants.PlayfieldSizeX;
            if (transform.LocalPosition.Y > GameConstants.PlayfieldSizeY) y = -GameConstants.PlayfieldSizeY;
            if (transform.LocalPosition.Y < -GameConstants.PlayfieldSizeY) y = GameConstants.PlayfieldSizeY;
            transform.LocalPosition = new Vector3(x, y, 0);
        }

        public override void OnDestory() {
            // Spawn particle effect
            // for (int i = 0; i < 20; i++) {
            Particle particle = MainGameScreen.particleManager.getNext();
            particle.Position = transform.Position - Vector3.Backward * 100;
            particle.Velocity = new Vector3(MainGameScreen.random.Next(-5, 5), 2, MainGameScreen.random.Next(-50, 50));
            particle.Acceleration = new Vector3(0, 3, 0);
            particle.MaxAge = MainGameScreen.random.Next(1, 6);
            particle.Init();
            // }

            GameConstants.score += GameConstants.KillBonus;

            for (int i = 0; i < 20; i++) {
                MainGameScreen.soundInstance = MainGameScreen.gunSound.CreateInstance();
                MainGameScreen.soundInstance.Play();
            }

            c.collidedThisFrame = false;
            c = null;

            base.OnDestory();
        }

    }

}
