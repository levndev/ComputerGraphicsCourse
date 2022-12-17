using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment8
{
    internal class Material
    {
        public Vector3 Emittance;
        public Vector3 Reflectance;
        public float Smoothness;
        public float Transparency;
        public float RefractiveIndex = 1;
    }
}
