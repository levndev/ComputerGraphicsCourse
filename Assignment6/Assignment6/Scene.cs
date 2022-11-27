using LearnOpenTK.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6
{
    internal class Scene
    {
        public List<Renderer> renderers= new List<Renderer>();
        public Light LightData;
        public Camera Camera;
        public bool Active
        {
            get;
            private set;
        }
        public Scene(Light lightData, Camera camera, params Renderer[] renderers)
        {
            LightData = lightData;
            Camera = camera;
            this.renderers.AddRange(renderers);
        }
        public void Load()
        {
            Window.DefaultShader.SetLight(LightData);
            Window.DefaultShader.SetUniformMatrix4("Projection", Camera.GetProjectionMatrix());
            Window.DefaultShader.SetUniformMatrix4("View", Camera.GetViewMatrix());
            Window.DefaultShader.SetUniform3("viewPos", Camera.Position);
            Active = true;
        }
        public void UnLoad()
        {
            Active = false;
        }
        public void Update(double deltaTime)
        {
            foreach (var r in renderers)
            {
                r.Update(deltaTime);
            }
        }
        public void Render(double deltaTime)
        {
            foreach (var r in renderers)
            {
                r.Render(deltaTime);
            }
        }

        public void OnResize(float aspectRatio)
        {
            Camera.AspectRatio = aspectRatio;
            if (Active)
            {
                Load();
            }
        }
    }
}
