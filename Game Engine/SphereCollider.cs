using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace CPI311.GameEngine {
    public class SphereCollider : Collider {
        public float Radius { get; set; }

        public override bool Collides(Collider other, out Vector3 normal) {
            if (other is SphereCollider) {
                SphereCollider collider = other as SphereCollider;
                float dist = (transform.Position - collider.transform.Position).LengthSquared();
                double pow = Math.Pow(Radius + collider.Radius, 2);
                if ( dist < pow ) {
                    normal = Vector3.Normalize(transform.Position - collider.transform.Position);
                    return true;
                }
            }
            else if (other is BoxCollider) return other.Collides(this, out normal);

            return base.Collides(other, out normal);
        }
    }
}