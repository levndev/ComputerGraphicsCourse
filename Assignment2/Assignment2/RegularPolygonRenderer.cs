using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Reflection;

namespace Assignment2
{
    public class RegularPolygonRenderer : Renderer
    {
        private int VAO;
        private int VertexCount;
        private Matrix4 Model;
        public double Radius;
        public int Sides;
        public double Angle;
        public Vector2 Position;
        public Color4 Color;

        public RegularPolygonRenderer(int sides, double radius, double angle, Vector2 position, Color4 color) : base(Window.DefaultShader)
        {
            Sides = sides;
            Radius = radius;
            Angle = angle;
            Position = position;
            Color = color;
            //
            GL.GenVertexArrays(1, out VAO);
            var VBO = GL.GenBuffer();
            //
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GenerateData();
        }

        public override void Render()
        {
            Shader.Use();
            Shader.SetUniform4("Color", Color);
            Shader.SetUniformMatrix4("Model", Model);
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, VertexCount);
        }

        private static Vector2 GetVertexCoordinates(int index, int sides, Vector2 center, double radius, double angle)
        {
            return new Vector2((float)(center.X + radius * Math.Cos((Math.PI / 180) * angle + 2 * Math.PI * index / sides)),
                             (float)(center.Y + radius * Math.Sin((Math.PI / 180) * angle + 2 * Math.PI * index / sides)));
        }

        private void GenerateData()
        {
            var center = Vector2.Zero;
            var vertices = new List<float>
            {
                center.X, center.Y
            };
            for (var i = 0; i <= Sides; i++)
            {
                var vertex = GetVertexCoordinates(i, Sides, center, 1, Angle);
                vertices.Add(vertex.X);
                vertices.Add(vertex.Y);
            }
            VertexCount = vertices.Count;
            Model = Matrix4.Identity * Matrix4.CreateScale((float)Radius);
            Model = Model * Matrix4.CreateTranslation(new Vector3(Position.X, Position.Y, 0.0f));
            GL.BindVertexArray(VAO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Count, vertices.ToArray(), BufferUsageHint.StreamDraw);
        }

        public override void Update()
        {
        }
    }
}
