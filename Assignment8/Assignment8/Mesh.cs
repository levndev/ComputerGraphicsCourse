using Assimp;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment8
{
    internal class Mesh
    {
        public Material Material;
        public Vector3 Position;
        public List<Vector3> Vertices = new List<Vector3>();
        public static Mesh LoadFromFile(string path, Vector3 position, Material material)
        {
            var result = new Mesh();
            result.Material = material;
            result.Position = position;
            AssimpContext importer = new AssimpContext();
            var s = importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.SortByPrimitiveType);
            if (s == null)
                throw new FileNotFoundException();
            foreach (var mesh in s.Meshes)
            {
                result.Vertices.AddRange(mesh.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)));
            }
            return result;
        }
    }
}
