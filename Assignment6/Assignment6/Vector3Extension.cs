using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6
{
    public static class Vector3Extension
    {
        public static float[] ToArray(this Vector3 vec)
        {
            return new float[] { vec.X, vec.Y, vec.Z };
        }
    }
}
