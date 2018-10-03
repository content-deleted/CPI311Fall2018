using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace CPI311.GameEngine {
    public class Collider : Behavior3d {
        public static List<Collider> colliders = new List<Collider>();

        public Rigidbody rigidbody;

        public override void  Start() {
            colliders.Add(this);
            rigidbody = obj.GetBehavior<Rigidbody>() as Rigidbody;
        }

        public virtual bool Collides(Collider other, out Vector3 normal) {
            normal = Vector3.Zero;
            return false;
        }

        public static int numberCollisions;

        public static void Update(GameTime gameTime) {
            Vector3 normal;

            foreach (Collider outer in colliders) {
                foreach (Collider inner in colliders) {
                    if (inner == outer) continue;
                    if (outer.Collides(inner, out normal)) {
                        numberCollisions++;
                        if (Vector3.Dot(normal, inner.rigidbody.Velocity) < 0)
                            inner.rigidbody.Impulse +=  Vector3.Dot(normal, inner.rigidbody.Velocity) * -2 * normal;
                        

                        Vector3 velocityNormal = Vector3.Dot(normal,
                            inner.rigidbody.Velocity - outer.rigidbody.Velocity) * -2
                               * normal * inner.rigidbody.Mass * outer.rigidbody.Mass;

                        inner.rigidbody.Impulse += velocityNormal / 2;
                        outer.rigidbody.Impulse += -velocityNormal / 2;
                    }
                }
            } 
        }
    }
}
