using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Assignment2
{
    public class QuadRenderer : Renderer
    {
        private Stopwatch Timer;
        private int VAO;
        private bool Dirty = false;
        private float[] Vertices = new float[]
        {
            1, 1,  // top right
            1, 0,  // bottom right
            0, 0, // bottom left
            0, 1,   // top left 
        };
        private uint[] Indices = new uint[]
        {  
            0, 1, 3,   // first triangle
            1, 2, 3    // second triangle
        };
        private Vector2 _Position;
        private Vector2 _Size;
        private Vector2 _Origin;
        private float _Angle;
        private Matrix4 Model;
        public Vector2 Position
        {
            get => _Position;
            set
            {
                Dirty = true;
                _Position = value;
            }
        }
        public Vector2 Size
        {
            get => _Size;
            set
            {
                Dirty = true;
                _Size = value;
            }
        }
        public Vector2 Origin
        {
            get => _Origin;
            set
            {
                Dirty = true;
                _Origin = value;
            }
        }
        public float Angle
        {
            get => _Angle;
            set
            {
                Dirty = true;
                _Angle = value;
            }
        }
        public Color4 Color = new Color4();
        private Action<QuadRenderer, TimeSpan>? Function;
        public QuadRenderer(Vector2 position, Vector2 size, Vector2 origin, float angle, Color4 color, Action<QuadRenderer, TimeSpan>? function = null) : base(Window.DefaultShader)
        {
            _Position = position;
            _Size = size;
            Color = color;
            _Origin = origin;
            _Angle = angle;
            Function = function;
            //
            //
            GL.GenVertexArrays(1, out VAO);
            var VBO = GL.GenBuffer();
            var EBO = GL.GenBuffer();
            //
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * Vertices.Length, Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * Indices.Length, Indices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GenerateModel();
            Timer = new Stopwatch();
            Timer.Start();
        }

        public override void Render()
        {
            if (Dirty)
            {
                GenerateModel();
            }
            Shader.Use();
            Shader.SetUniformMatrix4("Model", Model);
            Shader.SetUniform4("Color", Color);
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }

        private void GenerateModel()
        {
            Model = Matrix4.Identity;
            Model = Model * Matrix4.CreateScale(new Vector3(_Size.X, _Size.Y, 1.0f));
            Model = Model * Matrix4.CreateTranslation(new Vector3(-_Origin.X * _Size.X, -_Origin.Y * _Size.Y, 0.0f));
            Model = Model * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_Angle));
            Model = Model * Matrix4.CreateTranslation(new Vector3(_Position.X, _Position.Y, 0.0f));
            Dirty = false;
        }

        public override void Update()
        {
            Function?.Invoke(this, Timer.Elapsed);
        }
    }
}
