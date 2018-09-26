using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace CPI311.GameEngine
{
    public class GameObject2d : GameObject
    {
        public static List<GameObject2d> activeGameObjects = new List<GameObject2d>();

        //new protected List<Behavior2d> behaviors = new List<Behavior2d>();

        public new Behavior2d GetBehavior<T>() => behaviors.Where(x => x is T).First() as Behavior2d;

        public Sprite sprite;

        public GameObject2d() => sprite = new Sprite(null);

        public static GameObject2d Initialize()
        {
            GameObject2d g = new GameObject2d();
            activeGameObjects.Add(g);
            return g;
        }

        public static void UpdateObjects() {
            lock (activeGameObjects) {
                foreach (GameObject2d gameObject in activeGameObjects.ToList()) gameObject.Update();

                foreach (GameObject2d gameObject in activeGameObjects.ToList()) gameObject.LateUpdate();
            }
        }

        override public void Start() {
            foreach (Behavior2d behavior in behaviors) behavior.Start();
        }
        override public void Update() {
            foreach (Behavior2d behavior in behaviors) behavior.Update();
        }
        override public void LateUpdate() {
            foreach (Behavior2d behavior in behaviors) behavior.LateUpdate();
        }

        override public void Render(dynamic Renderer) => sprite.Draw(Renderer as SpriteBatch);

        override public void Destroy()
        {
            foreach(Behavior2d behavior in behaviors) behavior.OnDestory();
            activeGameObjects.Remove(this);
        }
    }
}
