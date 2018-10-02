using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{

    public class Transform
    {
        private Vector3 localPosition;
        private Quaternion localRotation;
        private Vector3 localScale;
        private Matrix world;

        private void UpdateWorld()
        {
            world = Matrix.CreateScale(localScale) * Matrix.CreateFromQuaternion(localRotation) * Matrix.CreateTranslation(localPosition);
            if (parent != null)  world *= parent.World;
            foreach (Transform child in Children) child.UpdateWorld();

        }
        public Matrix World { get => world; }

        public Vector3 LocalPosition
        {
            get => localPosition;
            set { localPosition = value; UpdateWorld(); }
        }

        public Vector3 LocalScale
        {
            get => localScale;
            set { localScale = value; UpdateWorld(); }
        }

        public Quaternion LocalRotation
        {
            get => localRotation;
            set { localRotation = value; UpdateWorld(); }
        }

        public Quaternion Rotation {
            get { return Quaternion.CreateFromRotationMatrix(World); }
            set {
                if (Parent == null) LocalRotation = value;
                else {
                    Vector3 scale, pos; Quaternion rot;
                    world.Decompose(out scale, out rot, out pos);

                    Matrix total = Matrix.CreateScale(scale) *
                          Matrix.CreateFromQuaternion(value) *
                          Matrix.CreateTranslation(pos);

                    LocalRotation = Quaternion.CreateFromRotationMatrix(
                         Matrix.Invert(Matrix.CreateScale(LocalScale)) * total *
                         Matrix.Invert(Matrix.CreateTranslation(LocalPosition)
                         * Parent.world));
                }
            }
        }


        public Transform()
        {
            localPosition = Vector3.Zero;
            localRotation = Quaternion.Identity;
            localScale = Vector3.One;
            UpdateWorld();
        }

        private Transform parent;

        public Transform Parent
        {
            get => parent;
            set
            {
                parent?.Children.Remove(this);
                parent = value;
                parent?.Children.Add(this);
                UpdateWorld();
            }
        }

        private List<Transform> children = new List<Transform>();

        private List<Transform> Children { get => children; }

        public void Rotate(Vector3 axis, float angle) => LocalRotation *= Quaternion.CreateFromAxisAngle(axis, angle);

        public Vector3 Position { get => World.Translation; }

        public Vector3 Forward { get => World.Forward; }

        public Vector3 Backward { get => World.Backward; }

        public Vector3 Up { get => World.Up; }

        public Vector3 Down { get => World.Down; }

        public Vector3 Left { get => World.Left; }

        public Vector3 Right { get => World.Right; }
    }
}
