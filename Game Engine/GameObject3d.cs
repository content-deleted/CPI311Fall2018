﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace CPI311.GameEngine
{
    public class GameObject3d : GameObject
    {
        public bool drawable = true;

        public static List<GameObject3d> activeGameObjects = new List<GameObject3d>();

        public new Behavior3d GetBehavior<T>() => behaviors.Where(x => x is T).First() as Behavior3d;

        public Transform transform;

        public Model mesh;

        public GameObject3d() => transform = new Transform();

        public static GameObject3d Initialize()
        {
            GameObject3d g = new GameObject3d();

            g.transform = new Transform();

            activeGameObjects.Add(g);
            return g;
        }

        public static void UpdateObjects() {
            lock (activeGameObjects) {
                foreach (GameObject3d gameObject in activeGameObjects.ToList()) gameObject.Update();

                foreach (GameObject3d gameObject in activeGameObjects.ToList()) gameObject.LateUpdate();
            }
        }

        override public void Start() {
            foreach (Behavior3d behavior in behaviors) behavior.Start();
        }
        override public void Update() {
            foreach (Behavior3d behavior in behaviors) behavior.Update();
        }
        override public void LateUpdate() {
            foreach (Behavior3d behavior in behaviors) behavior.LateUpdate();
        }

        override public void Render(dynamic Renderer) {
            if (!drawable) return;
            Camera c = (Renderer as Camera);
            mesh.Draw(transform.World, c.View, c.Projection);
        }

        override public void Destroy()
        {
            foreach(Behavior3d behavior in behaviors) behavior.OnDestory();
            activeGameObjects.Remove(this);
        }
    }
}
