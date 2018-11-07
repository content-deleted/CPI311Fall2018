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
        public static Effect effect;

        public Vector3 lightPosition = Vector3.Zero;
        public float shininess = 20f;
        public Vector3 ambientColor = new Vector3(0.2f, 0.2f, 0.2f);
        public Vector3 diffuseColor = new Vector3(0.5f, 0, 0);
        public Vector3 specularColor = new Vector3(0, 0, 0.5f);

        public Texture2D texture;

        public bool useTexture = true;

        public override void Render(Camera c, Transform t, Model m, GraphicsDevice g) {
            Matrix view = c.View;
            Matrix projection = c.Projection;

            effect.CurrentTechnique = effect.Techniques[0]; 
            effect.Parameters["World"].SetValue(t.World);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["LightPosition"].SetValue(lightPosition);
            effect.Parameters["CameraPosition"].SetValue(c.Transform.Position);
            effect.Parameters["Shininess"].SetValue(shininess);
            effect.Parameters["AmbientColor"].SetValue(ambientColor);
            effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
            effect.Parameters["SpecularColor"].SetValue(specularColor);
            effect.Parameters["DiffuseTexture"].SetValue(texture);
            effect.Parameters["UseTexture"].SetValue(useTexture);

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

    public class TerrainRenderer : Material {
        public static Effect effect;

        public GameObject3d ourObject; // LITERALLY A HACK
        private Texture2D HeightMap;
        public Texture2D NormalMap;

        public Vector2 size;
        private VertexPositionTexture[] Vertices { get; set; }

        private int[] Indices { get; set; }
        private float[] Heights { get; set; }

        public TerrainRenderer(Texture2D texture, Vector2 size, Vector2 res) {
            HeightMap = texture;
            this.size = size;

            createHeight(); //  Heights data is crated 

            // We should also save the value of size somewhere
            int rows = (int)res.Y + 1;
            int cols = (int)res.X + 1;

            Vector3 offset = new Vector3(-size.X / 2, 0, -size.Y / 2);
            float stepX = size.X / res.X;
            float stepZ = size.Y / res.Y;
            Vertices = new VertexPositionTexture[rows * cols];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    Vertices[r * cols + c] = new VertexPositionTexture(
                        offset + new Vector3(c * stepX, GetHeight( new Vector2 ( c/res.X, c/res.Y)), r * stepZ),
                        new Vector2(c / res.X, r / res.Y) );

            Indices = new int[(rows - 1) * (cols - 1) * 6];
            int index = 0;
            for (int r = 0; r < rows - 1; r++)
                for (int c = 0; c < cols - 1; c++) {
                    Indices[index++] = r * cols + c;
                    Indices[index++] = r * cols + c + 1;
                    Indices[index++] = (r + 1) * cols + c;

                    Indices[index++] = (r + 1) * cols + c;
                    Indices[index++] = r * cols + c + 1;
                    Indices[index++] = (r + 1) * cols + c + 1;
                }
        }

        private void createHeight() {
            Color[] data = new Color[HeightMap.Width * HeightMap.Height];
            HeightMap.GetData(data);
            Heights = new float[HeightMap.Width * HeightMap.Height];
            for (int i = 0; i < Heights.Length; i++)
                Heights[i] = data[i].G / 255f;
        }

        public float GetHeight(Vector2 tex) {
            // First, scale it to dimensions of the image
            tex = Vector2.Clamp(tex, Vector2.Zero, Vector2.One) * new Vector2(HeightMap.Width - 1, HeightMap.Height - 1);
            int x = (int)tex.X; float u = tex.X - x;
            int y = (int)tex.Y; float v = tex.Y - y;
            return Heights[y * HeightMap.Width + x] * (1 - u) * (1 - v) +
                 Heights[y * HeightMap.Width + MathHelper.Min(x + 1, HeightMap.Width - 1)] * u * (1 - v) +
                 Heights[MathHelper.Min(y + 1, HeightMap.Height - 1) * HeightMap.Width + x ] * (1 - u ) * v +
                 Heights[MathHelper.Min(y + 1, HeightMap.Height - 1) * HeightMap.Width + MathHelper.Min(x + 1, HeightMap.Width - 1)] * u * v;
        }

        public float GetAltitude(Vector3 position) {
            position = Vector3.Transform(position, Matrix.Invert(ourObject.transform.World));
            if (position.X > -size.X / 2 && position.X < size.X / 2 && position.Z > -size.Y / 2 && position.Z < size.Y / 2)
                return GetHeight(new Vector2 ( (position.X + size.X / 2) / size.X, (position.Z + size.Y / 2)/ size.Y )) * ourObject.transform.LocalScale.Y;
            return -1;
        }

        Vector3 lightPosition = new Vector3(1, 5, 9);
        float shininess = 0.1f;

        Vector3 ambientColor = new Vector3(0.1f, 0.1f, 0.1f);
        Vector3 diffuseColor = new Vector3(0.1f, 0.1f, 0.1f);
        Vector3 specularColor = new Vector3(0.1f, 0.1f, 0.1f);

        public override void Render(Camera c, Transform t, Model m, GraphicsDevice g) {
            Matrix view = c.View;
            Matrix projection = c.Projection;

            // Setup custom shader etc.
            effect.CurrentTechnique = effect.Techniques[0];
            effect.Parameters["World"].SetValue(t.World);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["LightPosition"].SetValue(lightPosition);
            effect.Parameters["CameraPosition"].SetValue(c.Transform.Position);
            effect.Parameters["Shininess"].SetValue(shininess);
            effect.Parameters["AmbientColor"].SetValue(ambientColor);
            effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
            effect.Parameters["SpecularColor"].SetValue(specularColor);
            effect.Parameters["NormalMap"].SetValue(NormalMap);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                g.DrawUserIndexedPrimitives
                    <VertexPositionTexture>(PrimitiveType.TriangleList,
                    Vertices, 0, Vertices.Length,
                    Indices, 0, Indices.Length / 3);
            }
        }

    }

    public class SpeedAndCollideEffect : Material {
        public static Effect effect;

        public float shininess = 20f;
        public Vector3 ambientColor = new Vector3(0.5f, 0.5f, 0.5f);
        public Vector3 diffuseColor = new Vector3(0, 0, 0);
        public Vector3 specularColor = new Vector3(0, 0, 0.5f);

        //public Texture2D texture;
        public static Texture2D disperseSample;
        public float timeSinceCol = 0;

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
            // effect.Parameters["DiffuseTexture"].SetValue(texture);
            effect.Parameters["timeSinceCol"].SetValue(timeSinceCol);
            effect.Parameters["DisperseTexture"].SetValue(disperseSample);

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
