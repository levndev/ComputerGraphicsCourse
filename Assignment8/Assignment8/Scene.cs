using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment8
{
    internal class Scene
    {
        public Func<Scene, bool> OnGUI;
        public List<object> Spheres;
        public List<object> Boxes;
        public Mesh Mesh;
        public Texture Skybox;
        public Dictionary<string, object> CustomSettings = new Dictionary<string, object>();
        public void Use(Shader shader)
        {
            shader.Use();
            if (Spheres != null) 
                shader.SetStructArray(Spheres, "Spheres", "SphereCount");
            else
                shader.SetInt("SphereCount", 0);
            if (Boxes != null)
                shader.SetStructArray(Boxes, "Boxes", "BoxCount");
            else
                shader.SetInt("BoxCount", 0);
            if (Mesh != null)
            {
                shader.SetInt("MeshSize", Mesh.Vertices.Count);
                //shader.SetVector3("MeshPosition", Mesh.Position);
                shader.SetStruct(Mesh.Material, "MeshMaterial");
                for (var i = 0; i < Mesh.Vertices.Count; i++)
                {
                    shader.SetVector3($"MeshVertices[{i}]", Mesh.Vertices[i]);
                }
            }
            else
            {
                shader.SetInt("MeshSize", 0);
            }
            Skybox.Use(TextureUnit.Texture0, TextureTarget.TextureCubeMap);
        }
    }
}
