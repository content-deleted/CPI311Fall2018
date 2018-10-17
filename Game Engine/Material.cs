using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine {
    public abstract class Material  {
        public abstract void Render(Camera c, Transform t, Model m, GraphicsDevice g);
    }

    public class DefaultMaterial : Material {
        public override void Render(Camera c, Transform t, Model m, GraphicsDevice g) {
            m.Draw(t.World, c.View, c.Projection);
        }
    }

    public class StandardLightingMaterial : Material {
        public Effect effect;

        public float shininess = 20f;
        public Vector3 ambientColor = new Vector3(0.2f, 0.2f, 0.2f);
        public Vector3 diffuseColor = new Vector3(0.5f, 0, 0);
        public Vector3 specularColor = new Vector3(0, 0, 0.5f);

        public Texture2D texture;

        public override void Render(Camera c, Transform t, Model m, GraphicsDevice g) {
            Matrix view = c.View;
            Matrix projection = c.Projection;

            effect.CurrentTechnique = effect.Techniques[0]; 
            effect.Parameters["World"].SetValue(t.World);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 + Vector3.Right * 5);
            effect.Parameters["CameraPosition"].SetValue(c.Transform.Position);
            effect.Parameters["Shininess"].SetValue(shininess);
            effect.Parameters["AmbientColor"].SetValue(ambientColor);
            effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
            effect.Parameters["SpecularColor"].SetValue(specularColor);
            effect.Parameters["DiffuseTexture"].SetValue(texture);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                foreach (ModelMesh mesh in m.Meshes)
                    foreach (ModelMeshPart part in mesh.MeshParts) {
                        g.SetVertexBuffer(part.VertexBuffer);
                        g.Indices = part.IndexBuffer;
                        g.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList, part.VertexOffset, 0,
                            part.NumVertices, part.StartIndex, part.PrimitiveCount);
                    }
            }
        }
    }
}
