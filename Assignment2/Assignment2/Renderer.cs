using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    public abstract class Renderer
    {
        public Renderer(Shader shader)
        {
            Shader = shader;
        }
        public abstract void Render();
        public abstract void Update();

        public Shader Shader;
    }
}
