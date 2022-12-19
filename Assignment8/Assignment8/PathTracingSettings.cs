using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment8
{
    internal class PathTracingSettings
    {
        [IgnoreInShader]
        public const int DefaultDepth = 8;
        public int Depth = DefaultDepth;
        public float EnvironmentRefractiveIndex = 1;
        public bool UseSkyboxForLighting = true;
    }
}
