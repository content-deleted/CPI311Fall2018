﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class Camera
    {
        public const float standardAspc = 16f / 9f;
        private float fov = 1;
        public float FieldOfView { get=>fov; set => fov = (value < Math.PI && value > 0) ? value : fov; }

        private float aspect = standardAspc;
        public float AspectRatio { get => aspect; set => aspect = value; }

        private float near = 1;
        public float NearPlane { get => near; set => near = (value < far) ? far : value; }

        private float far = 1000;
        public float FarPlane { get => far; set => far = (value > near) ? far : value; }

        public Transform Transform { get; set; }

        public Matrix Projection
        {
            get => Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);
        }

        public Matrix View
        {
            get => Matrix.CreateLookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up);
        }

        public Camera() => Transform = new Transform();

        public Vector2 Position = Vector2.Zero;
        public Vector2 Size = Vector2.One;

        public Viewport Viewport {
            get {
                return new Viewport((int)(ScreenManager.Width * Position.X),
                            (int)(ScreenManager.Height * Position.Y),
                            (int)(ScreenManager.Width * Size.X),
                            (int)(ScreenManager.Height * Size.Y));
            }
        }

        public Ray ScreenPointToWorldRay(Vector2 position) {
            Vector3 start = Viewport.Unproject(new Vector3(position, 0), Projection, View, Matrix.Identity);
            Vector3 end = Viewport.Unproject(new Vector3(position, 1), Projection, View, Matrix.Identity);
            return new Ray(start, end - start);
        }


    }
}
