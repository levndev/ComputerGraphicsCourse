using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StbImageSharp;
namespace Assignment6
{
    internal class MeshRenderer : Renderer
    {
        public bool IgnoreUserTransformations = false;
        public Action<Transform> Func;
        public Transform Transform;
        private Matrix4 Model;
        private Material material;
        private Mesh mesh;
        public MeshRenderer(Transform transform, Material material, Mesh mesh) : base(Window.DefaultShader)
        {
            this.mesh = mesh;
            Transform = transform;
            this.material = material;
            Model = Transform.GenerateModel();
        }

        public override void Render(double deltaTime)
        {
            Shader.Use();
            if (Transform.Dirty)
            {
                Model = Transform.GenerateModel();
                Transform.Dirty = false;
            }
            Shader.SetUniformMatrix4("Model", Model);
            Shader.SetMaterial(material);
            mesh.Render();
        }

        public override void Update(double deltaTime)
        {
            Func?.Invoke(Transform);
            if (Transform.Dirty)
            {
                Model = Transform.GenerateModel();
                Transform.Dirty = false;
            }
        }
    }
}
