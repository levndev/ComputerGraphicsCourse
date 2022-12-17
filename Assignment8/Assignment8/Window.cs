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
using StbImageSharp;
using System.IO;
using Dear_ImGui_Sample;
using ImGuiNET;
using System.Drawing;
using System.Net.WebSockets;
using Assimp;
using Assimp.Configs;
using Assimp.Unmanaged;
using static System.Formats.Asn1.AsnWriter;
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Assignment8
{
    public class Window : GameWindow
    {
        private Shader ptShader;
        private Shader debugShader;
        private Shader postProcessShader;
        private int VAO;
        private Framebuffer framebuffer;
        private List<Scene> Scenes;
        private int CurrentScene = 0;
        private Stopwatch stopwatch;
        private CameraData Camera;
        private int MouseSensitivity = 3;
        private float CameraMoveSpeed = 3f;
        private Vector2 CameraAngle = new Vector2(0, -90);
        private bool AccumulateFrames = true;
        private int AccumulatedFrames = 0;
        private bool ClearAccumulator = false;
        private bool Debug = false;
        private Texture Skybox;
        ImGuiController _controller;
        private bool ShowSettings = true;
        private int Samples = 8;
        private int MoveSamples = 4;
        DateTime PepeLaugh = new DateTime(2022, 12, 17);

        private PostProcessingSettings PostProcessingSettings;
        private PathTracingSettings PathTracingSettings;
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            VSync = VSyncMode.Adaptive;
            ptShader = new Shader("Shaders/ptShader.vert", "Shaders/ptShader.frag");
            debugShader = new Shader("Shaders/debug.vert", "Shaders/debug.frag");
            postProcessShader = new Shader("Shaders/postProcess.vert", "Shaders/postProcess.frag");
            //StbImage.stbi_set_flip_vertically_on_load(1);

            Skybox = Texture.LoadCubemap(new string[] { "Textures/right.jpg",
                                                        "Textures/left.jpg",
                                                        "Textures/top.jpg",
                                                        "Textures/bottom.jpg",
                                                        "Textures/front.jpg",
                                                        "Textures/back.jpg", });
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
            stopwatch = new Stopwatch();
            stopwatch.Start();
            Camera = new CameraData
            {
                Position = Vector3.UnitZ * 9,
                ViewportSize = new Vector2(Size.X, Size.Y),
                FOV = 90,
                Direction = -Vector3.UnitZ,
                Up = Vector3.UnitY,
            };
            Camera.SetRotation(CameraAngle);
            CursorState = CursorState.Grabbed;
            if (_controller == null)
                _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
            PostProcessingSettings = new PostProcessingSettings();
            PostProcessingSettings.KernelOffsetDivisor = 20;
            PathTracingSettings = new PathTracingSettings();
            PathTracingSettings.Depth = 8;
            PathTracingSettings.EnvironmentRefractiveIndex = 1f;
            Scenes = new List<Scene>
            {
                #region Scene1
                new Scene
                {
                    Skybox = Texture.LoadCubemap(new string[] { "Textures/night-skyboxes/Powerlines/posx.jpg",
                                                        "Textures/night-skyboxes/Powerlines/negx.jpg",
                                                        "Textures/night-skyboxes/Powerlines/posy.jpg",
                                                        "Textures/night-skyboxes/Powerlines/negy.jpg",
                                                        "Textures/night-skyboxes/Powerlines/posz.jpg",
                                                        "Textures/night-skyboxes/Powerlines/negz.jpg", }),
                    OnGUI = s =>
                    {
                        bool result = false;
                        if (ImGui.Button("Lock light sliders"))
                        {
                            s.CustomSettings["Lock light sliders"] = !(bool)s.CustomSettings["Lock light sliders"];
                        }
                        if ((bool)s.CustomSettings["Lock light sliders"])
                        {
                            if (ImGui.SliderFloat("Light strength", ref (s.Boxes[0] as Box).Material.Emittance.X, 0, 15))
                                result = true;
                            if (ImGui.SliderFloat("Light strength", ref (s.Boxes[0] as Box).Material.Emittance.Y, 0, 15))
                                result = true;
                            if (ImGui.SliderFloat("Light strength", ref (s.Boxes[0] as Box).Material.Emittance.Z, 0, 15))
                                result = true;
                        }
                        else
                        {
                            if (ImGui.SliderFloat("Light strength R", ref (s.Boxes[0] as Box).Material.Emittance.X, 0, 15))
                                result = true;
                            if (ImGui.SliderFloat("Light strength G", ref (s.Boxes[0] as Box).Material.Emittance.Y, 0, 15))
                                result = true;
                            if (ImGui.SliderFloat("Light strength B", ref (s.Boxes[0] as Box).Material.Emittance.Z, 0, 15))
                                result = true;
                        }
                        
                        return result;
                    },
                    CustomSettings =
                    {
                        { "Lock light sliders", true },
                    },
                    //Meshes = new List<object>
                    //{
                    //    new Mesh
                    //    {
                    //        Position = new Vector3(2.5f, 1.5f, -1.5f),
                    //        Triangles = new List<object>
                    //        {
                    //            new Triangle
                    //            {
                    //                v0 = new TriangleVertex{
                    //                    Position = new Vector3(-1, 0, 4),
                    //                },
                    //                v1 = new TriangleVertex{
                    //                    Position = new Vector3(0, 1, 4),
                    //                },
                    //                v2 = new TriangleVertex{
                    //                    Position = new Vector3(1, 0, 4),
                    //                },
                    //            }
                    //        },
                    //        Material = new Material
                    //        {
                    //            Emittance = new Vector3(0),
                    //            Reflectance = new Vector3(1, 0, 0),
                    //            Smoothness = 0,
                    //            Transparency = 0,
                    //        },
                    //    }
                    //},
                    Spheres = new List<object>
                    {
                        //right
                        new Sphere
                        {
                            Position = new Vector3(2.5f, 1.5f, -1.5f),
                            Radius = 1.5f,
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1, 0, 0),
                                Smoothness = 1,
                                Transparency = 0,
                            },
                        },
                        //left
                        new Sphere
                        {
                            Position = new Vector3(-2.5f, 2.5f, -1.0f),
                            Radius = 1,
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1, 0.4f, 0),
                                Smoothness = 0.8f,
                                Transparency = 0,
                            },
                        },
                        //center
                        new Sphere
                        {
                            Position = new Vector3(0.5f, -4.0f, 3.0f),
                            Radius = 1,
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 1f,
                                Transparency = 1,
                                RefractiveIndex = 1,
                            },
                        },
                    },
                    Boxes = new List<object>
                    {
                        //light
                        new Box
                        {
                            Position = new Vector3(0.0f, 4.8f, 0.0f),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(2.5f, 0.2f, 2.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(2),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //top
                        new Box
                        {
                            Position = new Vector3(0, 5.5f, 0),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(5, 0.5f, 5),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //bottom
                        new Box
                        {
                            Position = new Vector3(0, -5.5f, 0),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(5, 0.5f, 5),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0.3f,
                                Transparency = 0,
                            },
                        },
                        //left
                        new Box
                        {
                            Position = new Vector3(-5.5f, 0, 0),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 5, 5),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1, 0, 0),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //right
                        new Box
                        {
                            Position = new Vector3(5.5f, 0, 0),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 5, 5),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(0, 1, 0),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //back
                        new Box
                        {
                            Position = new Vector3(0, 0, -5.5f),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(5, 5, 0.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 1,
                                Transparency = 0,
                            },
                        },
                        //middle left
                        new Box
                        {
                            Position = new Vector3(-2.0f, -2.0f, -0.0f),
                            Rotation = new Matrix3(
                                new Vector3(0.7f, 0.0f, 0.7f),
                                new Vector3(0.0f, 1.0f, 0.0f),
                                new Vector3(-0.7f, 0.0f, 0.7f)
                            ),
                            HalfSize = new Vector3(1.5f, 3.0f, 1.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //middle right
                        new Box
                        {
                            Position = new Vector3(2.5f, -3.5f, -0.0f),
                            Rotation = new Matrix3(
                                new Vector3(0.7f, 0.0f, 0.7f),
                                new Vector3(0.0f, 1.0f, 0.0f),
                                new Vector3(-0.7f, 0.0f, 0.7f)
                            ),
                            HalfSize = new Vector3(1.0f, 1.5f, 1.0f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                    },
                },
                #endregion
                #region Scene2
                new Scene
                {
                    Skybox = Texture.LoadCubemap(new string[] { "Textures/field-skyboxes/Meadow/posx.jpg",
                                                        "Textures/field-skyboxes/Meadow/negx.jpg",
                                                        "Textures/field-skyboxes/Meadow/posy.jpg",
                                                        "Textures/field-skyboxes/Meadow/negy.jpg",
                                                        "Textures/field-skyboxes/Meadow/posz.jpg",
                                                        "Textures/field-skyboxes/Meadow/negz.jpg", }),
                    Spheres = new List<object>
                    {
                        //right
                        new Sphere
                        {
                            Position = new Vector3(2.5f, 1.5f, -1.5f),
                            Radius = 1.5f,
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1, 0, 0),
                                Smoothness = 1,
                                Transparency = 0,
                            },
                        },
                        //left
                        new Sphere
                        {
                            Position = new Vector3(-2.5f, 2.5f, -1.0f),
                            Radius = 1,
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1, 0.4f, 0),
                                Smoothness = 0.8f,
                                Transparency = 0,
                            },
                        },
                        //center
                        new Sphere
                        {
                            Position = new Vector3(0.5f, -4.0f, 3.0f),
                            Radius = 1,
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 1,
                            },
                        },
                    },
                    Boxes = new List<object>
                    {
                        //top
                        new Box
                        {
                            Position = new Vector3(0, 5.5f, 0),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(5, 0.5f, 5),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //bottom
                        new Box
                        {
                            Position = new Vector3(0, -5.5f, 0),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(5, 0.5f, 5),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0.3f,
                                Transparency = 0,
                            },
                        },
                        //left
                        new Box
                        {
                            Position = new Vector3(-5.5f, 0, 0),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 5, 5),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 1,
                                Transparency = 0,
                            },
                        },
                        //right
                        new Box
                        {
                            Position = new Vector3(5.5f, 0, 0),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 5, 5),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 1,
                                Transparency = 0,
                            },
                        },
                        //back
                        new Box
                        {
                            Position = new Vector3(0, 0, -5.5f),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(5, 5, 0.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //light
                        new Box
                        {
                            Position = new Vector3(0.0f, 4.8f, 0.0f),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(2.5f, 0.2f, 2.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //middle left
                        new Box
                        {
                            Position = new Vector3(-2.0f, -2.0f, -0.0f),
                            Rotation = new Matrix3(
                                new Vector3(0.7f, 0.0f, 0.7f),
                                new Vector3(0.0f, 1.0f, 0.0f),
                                new Vector3(-0.7f, 0.0f, 0.7f)
                            ),
                            HalfSize = new Vector3(1.5f, 3.0f, 1.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //middle right
                        new Box
                        {
                            Position = new Vector3(2.5f, -3.5f, -0.0f),
                            Rotation = new Matrix3(
                                new Vector3(0.7f, 0.0f, 0.7f),
                                new Vector3(0.0f, 1.0f, 0.0f),
                                new Vector3(-0.7f, 0.0f, 0.7f)
                            ),
                            HalfSize = new Vector3(1.0f, 1.5f, 1.0f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                    },
                },
                #endregion
            };

            var vList = new List<Vector3>();
            AssimpContext importer = new AssimpContext();
            var s = importer.ImportFile("Meshes/test2.fbx", PostProcessSteps.Triangulate | PostProcessSteps.SortByPrimitiveType);
            if (s == null)
                throw new FileNotFoundException();
            foreach (var mesh in s.Meshes)
            {
                vList.AddRange(mesh.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)));
            }
            #region Scene3
            Scenes.Add(new Scene
            {
                Skybox = Skybox,
                Mesh = new Mesh
                {
                    Material = new Material
                    {
                        Emittance = Vector3.Zero,
                        Reflectance = Vector3.One,
                        Smoothness = 0,
                        Transparency = 0,
                    },
                    Position = Vector3.Zero,
                    Vertices = vList,
                },
                Boxes = new List<object>
                {
                    //bottom
                    new Box
                    {
                        Position = new Vector3(0, -1.5f, 0),
                        Rotation = Matrix3.Identity,
                        HalfSize = new Vector3(10, 0.5f, 10),
                        Material = new Material
                        {
                            Emittance = new Vector3(0),
                            Reflectance = new Vector3(1),
                            Smoothness = 0,
                            Transparency = 0,
                        },
                    },
                    //top
                    new Box
                    {
                        Position = new Vector3(0, 4.5f, 0),
                        Rotation = Matrix3.Identity,
                        HalfSize = new Vector3(10, 0.5f, 10),
                        Material = new Material
                        {
                            Emittance = new Vector3(0),
                            Reflectance = new Vector3(1),
                            Smoothness = 0,
                            Transparency = 0,
                        },
                    },
                    //left
                    new Box
                    {
                        Position = new Vector3(-10.5f, 1.5f, 0),
                        Rotation = Matrix3.Identity,
                        HalfSize = new Vector3(0.5f, 3.5f, 10),
                        Material = new Material
                        {
                            Emittance = new Vector3(0),
                            Reflectance = new Vector3(1),
                            Smoothness = 0,
                            Transparency = 0,
                        },
                    },
                    //right
                    new Box
                    {
                        Position = new Vector3(10.5f, 1.5f, 0),
                        Rotation = Matrix3.Identity,
                        HalfSize = new Vector3(0.5f, 3.5f, 10),
                        Material = new Material
                        {
                            Emittance = new Vector3(0),
                            Reflectance = new Vector3(1),
                            Smoothness = 0,
                            Transparency = 0,
                        },
                    },
                    //back
                    new Box
                    {
                        Position = new Vector3(0, 1.5f, -10.5f),
                        Rotation = Matrix3.Identity,
                        HalfSize = new Vector3(10, 3.5f, 0.5f),
                        Material = new Material
                        {
                            Emittance = new Vector3(0),
                            Reflectance = new Vector3(1),
                            Smoothness = 1,
                            Transparency = 0,
                        },
                    },
                    //front
                    new Box
                    {
                        Position = new Vector3(0, 1.5f, 10.5f),
                        Rotation = Matrix3.Identity,
                        HalfSize = new Vector3(10, 3.5f, 0.5f),
                        Material = new Material
                        {
                            Emittance = new Vector3(0),
                            Reflectance = new Vector3(1),
                            Smoothness = 0,
                            Transparency = 0,
                        },
                    },
                    //light
                    new Box
                    {
                        Position = new Vector3(0, 3.9f, 0),
                        Rotation = Matrix3.Identity,
                        HalfSize = new Vector3(1, 0.1f, 1),
                        Material = new Material
                        {
                            Emittance = new Vector3(12),
                            Reflectance = new Vector3(0),
                            Smoothness = 0,
                            Transparency = 0,
                        },
                    },
                    new Box
                    {
                        Position = new Vector3(-2, -0.5f, 0),
                        Rotation = Matrix3.Identity,
                        HalfSize = new Vector3(0.5f),
                        Material = new Material
                        {
                            Emittance = new Vector3(0),
                            Reflectance = new Vector3(0, 1, 0),
                            Smoothness = 0,
                            Transparency = 0,
                        },
                    },
                    new Box
                    {
                        Position = new Vector3(2, -0.5f, 0),
                        Rotation = Matrix3.Identity,
                        HalfSize = new Vector3(0.5f),
                        Material = new Material
                        {
                            Emittance = new Vector3(0),
                            Reflectance = new Vector3(1, 0, 0),
                            Smoothness = 0,
                            Transparency = 0,
                        },
                    },
                },
            });
            #endregion
            //CurrentScene = Scenes.Count - 1;
            Scenes[CurrentScene].Use(ptShader);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);
            //GL.Clear(ClearBufferMask.ColorBufferBit);
            framebuffer.Use();
            var samples = Samples;
            //var Depth = 8;
            if (ClearAccumulator)
            {
                AccumulatedFrames = 0;
                GL.Clear(ClearBufferMask.ColorBufferBit);
                ClearAccumulator = false;
            }
            if (AccumulateFrames)
            {
                AccumulatedFrames++;
            }
            else
            {
                AccumulatedFrames = 1;
                GL.Clear(ClearBufferMask.ColorBufferBit);
                samples = MoveSamples;
            }
            if (Debug)
            {
                debugShader.Use();
                debugShader.SetVector3("uPosition", Camera.Position);
                debugShader.SetVector2("uViewportSize", Size);
                debugShader.SetVector3("uDirection", Camera.Direction);
                debugShader.SetVector3("uUp", Camera.Up);
                debugShader.SetFloat("uFOV", Camera.FOV);
                debugShader.SetInt("uSamples", samples);
                debugShader.SetFloat("uTime", (float)stopwatch.Elapsed.TotalSeconds);
            }
            else
            {
                ptShader.Use();
                ptShader.SetStruct(Camera, "Camera");
                ptShader.SetStruct(PathTracingSettings, "Settings");
                ptShader.SetInt("Samples", samples);
                var timeSeed = (float)stopwatch.Elapsed.TotalSeconds;
                Title = timeSeed.ToString();
                ptShader.SetFloat("Time", timeSeed);
            }
            GL.BindVertexArray(VAO);
            GL.DrawArrays(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, 0, 6);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            // post processing pass
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            postProcessShader.Use();
            postProcessShader.SetStruct(PostProcessingSettings, "Settings");
            postProcessShader.SetInt("Samples", AccumulatedFrames);
            framebuffer.TargetTexture.Use(TextureUnit.Texture0);
            GL.BindVertexArray(VAO);
            GL.DrawArrays(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, 0, 6);
            GUI((float)e.Time);
            SwapBuffers();
        }

        private void GUI(float time)
        {
            _controller.Update(this, time);
            if (ShowSettings)
            {
                var windowFlags = ImGuiWindowFlags.AlwaysAutoResize;
                if (CursorState == CursorState.Grabbed)
                    windowFlags |= ImGuiWindowFlags.NoMouseInputs;
                ImGui.Begin("Settings", windowFlags);
                ImGui.Text("C - show/hide cursor");
                ImGui.Text($"FPS - {(1f / time).ToString("0.0")}");
                ImGui.SliderInt("Mouse sensitivity", ref MouseSensitivity, 1, 10);
                ImGui.SliderFloat("Movement speed", ref CameraMoveSpeed, 0.1f, 10f);
                if (ImGui.SliderInt("Samples", ref Samples, 1, 1000) |
                    ImGui.SliderInt("Path tracing depth", ref PathTracingSettings.Depth, 1, 64, "%d", ImGuiSliderFlags.AlwaysClamp))
                {
                    ClearAccumulator = true;
                }
                ImGui.SliderInt("Samples when moving", ref MoveSamples, 1, 32);
                ImGui.SliderInt("Blur Kernel offset", ref PostProcessingSettings.KernelOffsetDivisor, 1, 30, "%d", ImGuiSliderFlags.AlwaysClamp);
                if (ImGui.Button("Reset frame accumulator"))
                    ClearAccumulator = true;
                ImGui.BeginGroup();
                if (ImGui.Button("Previous scene"))
                {
                    ChangeSceneLeft();
                }
                ImGui.SameLine();
                if (ImGui.Button("Next Scene"))
                {
                    ChangeSceneRight();
                }
                ImGui.EndGroup();
                if (ImGui.Button("Skybox light"))
                {
                    PathTracingSettings.UseSkyboxForLighting = !PathTracingSettings.UseSkyboxForLighting;
                    ClearAccumulator = true;
                }
                if (ImGui.Button("Debug"))
                {
                    Debug = !Debug;
                    ClearAccumulator = true;
                }
                if (ImGui.Button("Save image"))
                {
                    SaveScreenshot();
                }
                if (ImGui.Button("Close app"))
                    Close();
                if (Scenes[CurrentScene].OnGUI != null)
                {
                    ImGui.Text("Current scene settings");
                    ImGui.BeginGroup();
                    if (Scenes[CurrentScene].OnGUI.Invoke(Scenes[CurrentScene]))
                    {
                        ClearAccumulator = true;
                        Scenes[CurrentScene].Use(ptShader);
                    }
                    ImGui.EndGroup();
                }
                ImGui.End();
                if (!ShowSettings)
                    CursorState = CursorState.Grabbed;
            }
            _controller.Render();
            ImGuiController.CheckGLError("End of frame");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<DGAF>")]
        private void SaveScreenshot()
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
            var w = framebuffer.TargetTexture.Size.X;
            var h = framebuffer.TargetTexture.Size.Y;
            var bmp = new Bitmap(w, h);
            var data = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, w, h, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            Directory.CreateDirectory("Screenshots");
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            bmp.Save($"Screenshots/{DateTime.Now.ToString("HH-mm-ss-dd-MM-yyyy")}.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        private void ChangeSceneLeft()
        {
            if (--CurrentScene < 0)
            {
                CurrentScene = Scenes.Count - 1;
            }
            ClearAccumulator = true;
            Scenes[CurrentScene].Use(ptShader);

        }

        private void ChangeSceneRight()
        {
            if (++CurrentScene > Scenes.Count - 1)
            {
                CurrentScene = 0;
            }
            ClearAccumulator = true;
            Scenes[CurrentScene].Use(ptShader);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var kInput = KeyboardState;
            var mInput = MouseState;
            if (kInput.IsKeyPressed(Keys.Escape))
                Close();
            var KeyboardDelta = Vector3.Zero;
            var moveSpeed = CameraMoveSpeed;
            if (kInput.IsKeyDown(Keys.LeftShift))
            {
                moveSpeed = 2 * moveSpeed;
            }
            if (kInput.IsKeyDown(Keys.W))
            {
                KeyboardDelta += Camera.Direction * moveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyDown(Keys.S))
            {
                KeyboardDelta -= Camera.Direction * moveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyDown(Keys.A))
            {
                KeyboardDelta -= Camera.Right() * moveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyDown(Keys.D))
            {
                KeyboardDelta += Camera.Right() * moveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyDown(Keys.Space))
            {
                KeyboardDelta.Y += moveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyDown(Keys.LeftControl))
            {
                KeyboardDelta.Y -= moveSpeed * (float)e.Time;
            }
            if (kInput.IsKeyPressed(Keys.Left))
            {
                ChangeSceneLeft();
            }
            if (kInput.IsKeyPressed(Keys.Right))
            {
                ChangeSceneRight();
            }

            if (kInput.IsKeyPressed(Keys.C))
            {
                if (CursorState == CursorState.Normal)
                    CursorState = CursorState.Grabbed;
                else
                    CursorState = CursorState.Normal;
            }
            var MouseDelta = Vector2.Zero;
            if (mInput.Delta.X != 0)
            {
                MouseDelta.Y = mInput.Delta.X * (MouseSensitivity / 10f - 0.09f);
            }
            if (mInput.Delta.Y != 0)
            {
                MouseDelta.X = -mInput.Delta.Y * (MouseSensitivity / 10f - 0.09f);
            }
            if (CursorState == CursorState.Grabbed)
            {
                if (KeyboardDelta.Length > 0 || MouseDelta.Length > 0)
                {
                    if (MouseDelta.Length > 0)
                    {
                        CameraAngle.X = MathHelper.Clamp(CameraAngle.X + MouseDelta.X, -89, 89);
                        CameraAngle.Y = CameraAngle.Y + MouseDelta.Y;
                        Camera.SetRotation(CameraAngle);
                    }
                    if (KeyboardDelta.Length > 0)
                    {
                        Camera.Position += KeyboardDelta;
                    }
                    if (AccumulateFrames)
                    {
                        AccumulateFrames = false;
                    }
                }
                else if (KeyboardDelta.Length == 0 || MouseDelta.Length == 0)
                {
                    if (!AccumulateFrames)
                    {
                        AccumulateFrames = true;
                        AccumulatedFrames = 0;
                        ClearAccumulator = true;
                    }
                }
            }
            //Title = AccumulatedFrames.ToString();
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            if (_controller == null)
                _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
            _controller.WindowResized(ClientSize.X, ClientSize.Y);
            GenerateFrameBuffer();
            ClearAccumulator = true;
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            _controller.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _controller.MouseScroll(e.Offset);
        }

        private void GenerateFrameBuffer()
        {
            if (framebuffer != null)
                framebuffer.Dispose();
            int texHandle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb32f, ClientSize.X, ClientSize.Y, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.MirroredRepeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.MirroredRepeat);
            var texture = new Texture(texHandle, new Vector2i(ClientSize.X, ClientSize.Y));
            framebuffer = new Framebuffer(texture);
        }
    }
}
