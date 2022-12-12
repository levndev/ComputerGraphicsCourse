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
        private Shader ptShader;
        private Shader postProcessShader;
        private int VAO;
        private Framebuffer framebuffer;
        private List<object> spheres;
        private Stopwatch stopwatch;
        private CameraData Camera;
        private float CameraRotationSpeed = 1;
        private float MouseSensitivity = 0.1f;
        private Vector3 AmbientLight = new Vector3(1);
        private Vector2 CameraAngle = new Vector2(0, -90);
        private bool Debug = true;
        private bool AccumulateFrames = true;
        private int AccumulatedFrames = 0;
        private bool ClearAccumulator = false;
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            VSync = VSyncMode.Adaptive;
            ptShader = new Shader("Shaders/ptShader.vert", "Shaders/ptShader.frag");
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
            spheres = new List<object>();
            spheres.Add(new Sphere()
            {
                Position = new Vector3(-3, 0, -5),
                Radius = 1,
                Material = new Material()
                {
                    Emittance = new Vector3(1),
                    Reflectance = new Vector3(1),
                    Roughness = 0,
                    Opacity = 0
                }
            });
            spheres.Add(new Sphere()
            {
                Position = new Vector3(3, 0, -5),
                Radius = 2,
                Material = new Material()
                {
                    Emittance = new Vector3(0),
                    Reflectance = new Vector3(1),
                    Roughness = 0,
                    Opacity = 0
                }
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
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);
            //GL.Clear(ClearBufferMask.ColorBufferBit);
            framebuffer.Use();
            if (ClearAccumulator)
            {
                AccumulatedFrames = 0;
                ClearAccumulator = false;
                GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);
            }
            ptShader.Use();
            ptShader.SetStruct(Camera, "Camera");
            ptShader.SetStructArray(spheres, "Spheres", "SphereCount");
            ptShader.SetInt("Depth", 8);
            ptShader.SetInt("Samples", 4);
            ptShader.SetVector3("AmbientLight", AmbientLight);
            ptShader.SetVector3("BackgroundColor", new Vector3(1, 0, 0));
            ptShader.SetFloat("Time", (float)stopwatch.Elapsed.TotalSeconds);
            ptShader.SetBool("Debug", Debug);
            if (AccumulateFrames)
            {
                AccumulatedFrames++;
            }
            else
            {
                AccumulatedFrames = 1;
            }
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            // post processing pass
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero); 
            GL.Clear(ClearBufferMask.ColorBufferBit);
            postProcessShader.Use();
            postProcessShader.SetInt("Samples", AccumulatedFrames);
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
            var CameraPositionDelta = Vector3.Zero;
            if (kInput.IsKeyDown(Keys.LeftShift))
            {
                CameraMoveSpeed = 2f;
            }
            if (kInput.IsKeyDown(Keys.W))
            {
                CameraPositionDelta += Camera.Direction * CameraMoveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyDown(Keys.S))
            {
                CameraPositionDelta -= Camera.Direction * CameraMoveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyDown(Keys.A))
            {
                CameraPositionDelta -= Camera.Right() * CameraMoveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyDown(Keys.D))
            {
                CameraPositionDelta += Camera.Right() * CameraMoveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyDown(Keys.Space))
            {
                CameraPositionDelta.Y += CameraMoveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyDown(Keys.LeftControl))
            {
                CameraPositionDelta.Y -= CameraMoveSpeed * (float)e.Time;
            }
            
            var CameraRotationDelta = Vector2.Zero;
            if (mInput.Delta.X != 0)
            {
                CameraRotationDelta.Y = mInput.Delta.X * MouseSensitivity;
            }
            if (mInput.Delta.Y != 0)
            {
                CameraRotationDelta.X = - mInput.Delta.Y * MouseSensitivity;
            }
            if (CameraPositionDelta.Length > 0 || CameraRotationDelta.Length > 0)
            {
                if (CameraRotationDelta.Length > 0)
                {
                    CameraAngle.X = MathHelper.Clamp(CameraAngle.X + CameraRotationDelta.X, -89, 89);
                    CameraAngle.Y = CameraAngle.Y + CameraRotationDelta.Y;
                    Camera.SetRotation(CameraAngle);
                }
                if (CameraPositionDelta.Length > 0)
                {
                    Camera.Position += CameraPositionDelta;
                }
                if (AccumulateFrames)
                {
                    AccumulateFrames = false;
                    ClearAccumulator = true;
                }
            }
            else if (CameraPositionDelta.Length == 0 || CameraRotationDelta.Length == 0)
            {
                if (!AccumulateFrames)
                {
                    AccumulateFrames = true;
                    ClearAccumulator = true;
                }
            }
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            GenerateFrameBuffer();
            ClearAccumulator = true;
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
