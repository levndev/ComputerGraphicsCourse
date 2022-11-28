using OpenTK.Mathematics;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6
{
    public struct Material
    {
        public Vector3 Ambient;
        public Vector3 Diffuse;
        public Vector3 Specular;
        public float Shininess;
        public Texture DiffuseMap;
    }
}
