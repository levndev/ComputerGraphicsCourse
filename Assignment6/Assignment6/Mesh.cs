using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Assignment6
{
    internal class Mesh
    {
        private class SubMesh
        {
            public int vertexCount;
            public int VAO;
            public PrimitiveType PrimitiveType;
            private const int VertexArgumentCount = 8;
            public void Render()
            {
                GL.BindVertexArray(VAO);
                GL.DrawArrays(PrimitiveType, 0, vertexCount / VertexArgumentCount);
            }
        }
        private List<SubMesh> subMeshes;

        public Mesh(float[] vertices, PrimitiveType primitiveType = PrimitiveType.Triangles) : this(Tuple.Create(vertices, primitiveType)) { }

        public Mesh(params Tuple<float[], PrimitiveType>[] subMeshData)
        {
            subMeshes = new List<SubMesh>();
            foreach (var (vertices, type) in subMeshData)
            {
                var subMesh = new SubMesh();
                var vertexCount = vertices.Length;
                GL.GenVertexArrays(1, out int VAO);
                var VBO = GL.GenBuffer();
                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertexCount, vertices, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
                GL.EnableVertexAttribArray(2);
                subMesh.vertexCount = vertexCount;
                subMesh.VAO = VAO;
                subMesh.PrimitiveType = type;
                subMeshes.Add(subMesh);
            }
        }

        public void Render()
        {
            foreach (var subMesh in subMeshes)
            {
                subMesh.Render();
            }
        }
    }
}
