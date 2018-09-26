using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class Camera
    {
        private float fov = 1;
        public float FieldOfView { get=>fov; set => fov = (value < Math.PI && value > 0) ? value : fov; }

        private float aspect = 16/9;
        public float AspectRatio { get => aspect; set => aspect = value; }

        float near = 1;
        public float NearPlane { get => near; set => near = (value < far) ? far : value; }

        float far = 100;
        public float FarPlane { get => far; set => far = (value > near) ? far : value; }

        public Transform Transform { get; set; }

        public Matrix Projection
        {
            get { return Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane); }
        }

        public Matrix View
        {
            get => Matrix.CreateLookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up);
        }

        public Camera() => Transform = new Transform();

    }
}
