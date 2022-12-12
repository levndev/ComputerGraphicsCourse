using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment8
{
    internal class Sphere
    {
        public Vector3 Position;
        public float Radius;
        public Material Material;
        public Vector3 Color;
        public bool IgnoreLight = false;
    }
}
