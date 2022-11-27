using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6
{
    public abstract class Renderer
    {
        public Shader Shader;
        public Renderer(Shader shader)
        {
            Shader = shader;
        }
        public abstract void Render(double deltaTime);
        public abstract void Update(double deltaTime);
    }
}
