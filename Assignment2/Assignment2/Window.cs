using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Windowing;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Diagnostics;

namespace Assignment2
{
    public class Window : GameWindow
    {
        private Stopwatch Stopwatch;
        private bool Wireframe = false;
        public List<Renderer> Renderers = new List<Renderer>();
        public static Shader DefaultShader;
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color4.DarkGray);
            DefaultShader = new Shader("Shaders/DefaultShader.vs", "Shaders/DefaultShader.fs");
            DefaultShader.Use();
            Matrix4 projection;
            Matrix4.CreateOrthographicOffCenter(0, 800, 800, 0, -1, 1, out projection);
            DefaultShader.SetUniformMatrix4("Projection", projection);

            //
            var Quad1 = new QuadRenderer(new Vector2(400, 350), new Vector2(100, 100), new Vector2(0.5f, 0.5f), 0, Color4.DarkBlue, (quad, time) =>
            {
                quad.Angle = (float)time.TotalMilliseconds / 16;
                quad.Position = new Vector2(400 + (float)Math.Sin(time.TotalSeconds) * 400, 350 + (float)Math.Sin(8 * time.TotalSeconds) * 75);
            });
            var Quad2 = new QuadRenderer(new Vector2(400, 650), new Vector2(100, 100), new Vector2(0,0), 0, Color4.DarkBlue, (quad, time) =>
            {
                quad.Angle = (float)time.TotalMilliseconds / 16;
                quad.Size = (Math.Abs((float)Math.Sin(time.TotalSeconds)) * 100 + 50, Math.Abs((float)Math.Sin(time.TotalSeconds)) * 100 + 50);
            });
            Renderers.Add(Quad1);
            Renderers.Add(Quad2);
            Renderers.Add(new QuadRenderer(new Vector2(400, 10), new Vector2(10, 10), new Vector2(0, 0), 0, Color4.Yellow));
            Renderers.Add(new RegularPolygonRenderer(8, 100, 0, new Vector2(600, 100), Color4.DarkGreen));
            Renderers.Add(new CircleRenderer(5, 50, new Vector2(300, 100), Color4.DarkRed));
            Renderers.Add(new CircleRenderer(20, 50, new Vector2(200, 100), Color4.DarkRed));
            Renderers.Add(new CircleRenderer(30, 50, new Vector2(100, 100), Color4.DarkRed));
            Renderers.Add(new CircleRenderer(40, 50, new Vector2(50, 100), Color4.DarkRed));
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            foreach (var renderer in Renderers)
            {
                renderer.Render();
            }
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var input = KeyboardState;

            if (input.IsKeyPressed(Keys.Escape))
            {
                Close();
            }

            if (input.IsKeyPressed(Keys.Space))
            {
                if (Wireframe)
                {
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                    Wireframe = false;
                }
                else
                {
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                    Wireframe = true;
                }
            }
            foreach (var renderer in Renderers)
            {
                renderer.Update();
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            DefaultShader.Use();
            Matrix4 projection;
            Matrix4.CreateOrthographicOffCenter(0, Size.X, Size.Y, 0, -1, 1, out projection);
            DefaultShader.SetUniformMatrix4("Projection", projection);
        }
    }
}
