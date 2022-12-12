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

namespace Assignment8
{
    public class Window : GameWindow
    {
        private Shader rtShader;
        private Shader postProcessShader;
        private int VAO;
        private Framebuffer framebuffer;
        private List<object> spheres;
        private List<object> planes;
        private List<object> lights;
        private Stopwatch stopwatch;
        private CameraData Camera;
        private float CameraRotationSpeed = 1;
        private float MouseSensitivity = 0.1f;
        private Vector3 AmbientLight = new Vector3(1);
        private Vector2 CameraAngle = new Vector2(0, -90);
        private bool Debug = true;
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            VSync = VSyncMode.Adaptive;
            rtShader = new Shader("Shaders/rtShader.vert", "Shaders/rtShader.frag");
            postProcessShader = new Shader("Shaders/postProcess.vert", "Shaders/postProcess.frag");
            var vertices = new float[]
            {
                -1, -1, 0, 0,
                1, -1, 1, 0,
                1, 1, 1, 1,
                1, 1, 1, 1,
                -1, 1, 0, 1,
                -1, -1, 0, 0,
            };
            GL.GenVertexArrays(1, out VAO);
            var VBO = GL.GenBuffer();
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Count(), vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GenerateFrameBuffer();
            var material = new Material()
            {
                Ambient = new Vector3(0.3f),
                Diffuse = new Vector3(1.0f, 0.5f, 0.31f),
                Specular = new Vector3(0.5f, 0.5f, 0.5f),
                Shininess = 32.0f,
                Reflection = 0,
                Refraction = 0,
            };
            spheres = new List<object>();
            planes = new List<object>();
            spheres.Add(new Sphere()
            {
                // Color = ((Vector4)Color4.Blue).Xyz,
                Position = new Vector3(0, 6, -8),
                Radius = 1,
                Material = material,
                IgnoreLight = true,
                Color = new Vector3(1, 0, 0),
            });
            spheres.Add(new Sphere()
            {
                // Color = ((Vector4)Color4.Blue).Xyz,
                Position = new Vector3(0, 0, -8),
                Radius = 1,
                Material = new Material()
                {
                    Ambient = new Vector3(0.3f),
                    Diffuse = new Vector3(1.0f, 0.5f, 0.31f),
                    Specular = new Vector3(0.5f, 0.5f, 0.5f),
                    Shininess = 32.0f,
                    Reflection = 0.7f,
                    Refraction = 0,
                    RefractionIndex = 0,
                }
            });
            spheres.Add(new Sphere()
            {
                //Color = ((Vector4)Color4.Blue).Xyz,
                Position = new Vector3(3, 0, -8),
                Radius = 1,
                Material = material,
            });
            spheres.Add(new Sphere()
            {
                //Color = ((Vector4)Color4.White).Xyz,
                Position = new Vector3(-3, 0, -8),
                Radius = 1,
                Material = new Material()
                {
                    Ambient = new Vector3(0, 0, 1),
                    Diffuse = new Vector3(0, 0, 1),
                    Specular = new Vector3(0.5f, 0.5f, 0.5f),
                    Shininess = 32.0f,
                    Reflection = 0,
                    Refraction = 0,
                    RefractionIndex = 0,
                }
            });
            spheres.Add(new Sphere()
            {
                //Color = new Vector3(0, 1, 0),
                Position = new Vector3(0, -3, -8),
                Radius = 1,
                Material = new Material()
                {
                    Ambient = new Vector3(0.3f),
                    Diffuse = new Vector3(1.0f, 0.5f, 0.31f),
                    Specular = new Vector3(0.5f, 0.5f, 0.5f),
                    Shininess = 32.0f,
                    Reflection = 0,
                    Refraction = 1,
                    RefractionIndex = 1.5f,
                }
            });
            spheres.Add(new Sphere()
            {
                //Color = new Vector3(0, 1, 0),
                Position = new Vector3(3, -3, -8),
                Radius = 1,
                Material = material,
            });
            spheres.Add(new Sphere()
            {
                //Color = new Vector3(0, 1, 0),
                Position = new Vector3(-3, -3, -8),
                Radius = 1,
                Material = new Material()
                {
                    Ambient = new Vector3(0, 1, 0),
                    Diffuse = new Vector3(0, 1, 0),
                    Specular = new Vector3(0.5f, 0.5f, 0.5f),
                    Shininess = 32.0f,
                    Reflection = 0,
                    Refraction = 0,
                    RefractionIndex = 0f,
                }
            });
            lights = new List<object>();
            //lights.Add(new Light
            //{
            //    Position = new Vector3(-3, 3, 2),
            //    Diffuse = new Vector3(0.8f),
            //    Specular = new Vector3(0.5f),
            //});
            lights.Add(new Light
            {
                Position = new Vector3(0, 4, -8),
                Diffuse = new Vector3(0.8f),
                Specular = new Vector3(0.5f),
            });
            stopwatch = new Stopwatch();
            stopwatch.Start();
            Camera = new CameraData
            {
                ViewportSize = new Vector2(Size.X, Size.Y),
                FOV = 90,
                Direction = -Vector3.UnitZ,
                Up = Vector3.UnitY,
            };
            Camera.SetRotation(CameraAngle);
            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.ClearColor(Color4.Black);
            //GL.Clear(ClearBufferMask.ColorBufferBit);
            framebuffer.Use();
            rtShader.Use();
            rtShader.SetStruct(Camera, "Camera");
            rtShader.SetStructArray(spheres, "Spheres", "SphereCount");
            rtShader.SetStructArray(planes, "Planes", "PlaneCount");
            rtShader.SetStructArray(lights, "Lights", "LightCount");
            rtShader.SetInt("Depth", 16);
            rtShader.SetVector3("AmbientLight", AmbientLight);
            rtShader.SetVector3("BackgroundColor", new Vector3(0.2f, 0.2f, 0.2f));
            rtShader.SetFloat("Time", (float)stopwatch.Elapsed.TotalSeconds);
            rtShader.SetBool("Debug", Debug);
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            // post processing pass
            postProcessShader.Use();
            framebuffer.TargetTexture.Use(TextureUnit.Texture0);
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var kInput = KeyboardState;
            var mInput = MouseState;
            if (kInput.IsKeyPressed(Keys.Escape))
                Close();
            if (kInput.IsKeyPressed(Keys.F))
            {
                Debug = !Debug;
            }
            var CameraMoveSpeed = 1f;
            if (kInput.IsKeyDown(Keys.LeftShift))
            {
                CameraMoveSpeed = 2f;
            }
            if (kInput.IsKeyDown(Keys.W))
            {
                Camera.Position += Camera.Direction * CameraMoveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyDown(Keys.S))
            {
                Camera.Position -= Camera.Direction * CameraMoveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyDown(Keys.A))
            {
                Camera.Position -= Camera.Right() * CameraMoveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyDown(Keys.D))
            {
                Camera.Position += Camera.Right() * CameraMoveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyDown(Keys.Space))
            {
                Camera.Position.Y += CameraMoveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyDown(Keys.LeftControl))
            {
                Camera.Position.Y -= CameraMoveSpeed * (float)e.Time;
            }
            if (mInput.Delta.X != 0)
            {
                CameraAngle.Y = CameraAngle.Y + mInput.Delta.X * MouseSensitivity;
                Camera.SetRotation(CameraAngle);
            }
            if (mInput.Delta.Y != 0)
            {
                CameraAngle.X = MathHelper.Clamp(CameraAngle.X - mInput.Delta.Y * MouseSensitivity, -89, 89);
                Camera.SetRotation(CameraAngle);
            }
            var totalTime = stopwatch.Elapsed.TotalSeconds;
            var x = MathF.Sin((float)totalTime / 2) * 6;
            //(lights[0] as Light).Position.X = x;
            //(spheres[0] as Sphere).Position.X = x;
            //Title = x.ToString();
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            GenerateFrameBuffer();
        }

        private void GenerateFrameBuffer()
        {
            int texHandle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb32f, Size.X, Size.Y, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            var texture = new Texture(texHandle);
            framebuffer = new Framebuffer(texture);
        }
    }
}
