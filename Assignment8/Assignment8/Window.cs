using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Diagnostics;
using Dear_ImGui_Sample;
using ImGuiNET;
using System.Drawing;

namespace Assignment8
{
    public class Window : GameWindow
    {
        private Shader ptShader;
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
        ImGuiController _controller;
        private bool ShowSettings = true;
        private const int DefaultSamples = 8;
        private const int DefaultMoveSamples = 4;
        private int Samples = DefaultSamples;
        private int MoveSamples = DefaultMoveSamples;
        DateTime PepeLaugh = new DateTime(2022, 12, 17);
        private bool HighQualityMode = false;
        private const int DefaultHighQualityModeIterations = 10;
        private int HighQualityModeIterations = DefaultHighQualityModeIterations;
        private int HighQualityModeSamples = 128;
        private int HighQualityModeDepth = 32;
        private DateTime HighQualityModeDate;
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
            postProcessShader = new Shader("Shaders/postProcess.vert", "Shaders/postProcess.frag");
            //StbImage.stbi_set_flip_vertically_on_load(1);
            var defaultSkybox = Texture.LoadCubemap(new string[] { "Textures/right.jpg",
                                                        "Textures/left.jpg",
                                                        "Textures/top.jpg",
                                                        "Textures/bottom.jpg",
                                                        "Textures/front.jpg",
                                                        "Textures/back.jpg", });
            var Powerlines = Texture.LoadCubemap(new string[] { "Textures/night-skyboxes/Powerlines/posx.jpg",
                                                        "Textures/night-skyboxes/Powerlines/negx.jpg",
                                                        "Textures/night-skyboxes/Powerlines/posy.jpg",
                                                        "Textures/night-skyboxes/Powerlines/negy.jpg",
                                                        "Textures/night-skyboxes/Powerlines/posz.jpg",
                                                        "Textures/night-skyboxes/Powerlines/negz.jpg", });
            var SpaceSkybox = Texture.LoadCubemap(new string[] { "Textures/ulukai/corona_ft.png",
                                                        "Textures/ulukai/corona_bk.png",
                                                        "Textures/ulukai/corona_up.png",
                                                        "Textures/ulukai/corona_dn.png",
                                                        "Textures/ulukai/corona_rt.png",
                                                        "Textures/ulukai/corona_lf.png", });
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
                #region Scene0
                new Scene
                {
                    Skybox = defaultSkybox,
                    OnGUI = s =>
                    {
                        bool result = false;
                        if (ImGui.Button("Lock light sliders"))
                        {
                            s.CustomSettings["Lock light sliders"] = !(bool)s.CustomSettings["Lock light sliders"];
                        }
                        if ((bool)s.CustomSettings["Lock light sliders"])
                        {
                            if (ImGui.SliderFloat("Light strength", ref (s.Boxes[0] as Box).Material.Emittance.X, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light strength", ref (s.Boxes[0] as Box).Material.Emittance.Y, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light strength", ref (s.Boxes[0] as Box).Material.Emittance.Z, 0, 30))
                                result = true;
                        }
                        else
                        {
                            if (ImGui.SliderFloat("Light strength R", ref (s.Boxes[0] as Box).Material.Emittance.X, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light strength G", ref (s.Boxes[0] as Box).Material.Emittance.Y, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light strength B", ref (s.Boxes[0] as Box).Material.Emittance.Z, 0, 30))
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
                            Position = new Vector3(2.5f, -4, 3),
                            Radius = 1.5f,
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 1,
                                Transparency = 0,
                            },
                        },
                        //left
                        new Sphere
                        {
                            Position = new Vector3(-2.5f, -4, 3),
                            Radius = 1,
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 1,
                                Transparency = 1,
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
                                Reflectance = new Vector3(0, 0, 1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //middle left
                        new Box
                        {
                            Position = new Vector3(-2.0f, -2.0f, -1.0f),
                            Rotation = new Matrix3(
                                new Vector3(0.6f, 0.0f, 0.6f),
                                new Vector3(0.0f, 1.0f, 0.0f),
                                new Vector3(-0.6f, 0.0f, 0.6f)
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
                            Position = new Vector3(2.5f, -3.5f, -1.0f),
                            Rotation = new Matrix3(
                                new Vector3(0.6f, 0.0f, 0.6f),
                                new Vector3(0.0f, 1.0f, 0.0f),
                                new Vector3(-0.6f, 0.0f, 0.6f)
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
                #region Scene1
                new Scene
                {
                    Skybox = Powerlines,
                    SkyboxColorMultiplier = 0.05f,
                    OnGUI = s =>
                    {
                        bool result = false;
                        if (ImGui.Button("Lock light sliders"))
                        {
                            s.CustomSettings["Lock light sliders"] = !(bool)s.CustomSettings["Lock light sliders"];
                        }
                        if ((bool)s.CustomSettings["Lock light sliders"])
                        {
                            if (ImGui.SliderFloat("Light strength", ref (s.Boxes[0] as Box).Material.Emittance.X, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light strength", ref (s.Boxes[0] as Box).Material.Emittance.Y, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light strength", ref (s.Boxes[0] as Box).Material.Emittance.Z, 0, 30))
                                result = true;
                        }
                        else
                        {
                            if (ImGui.SliderFloat("Light strength R", ref (s.Boxes[0] as Box).Material.Emittance.X, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light strength G", ref (s.Boxes[0] as Box).Material.Emittance.Y, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light strength B", ref (s.Boxes[0] as Box).Material.Emittance.Z, 0, 30))
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
                            Position = new Vector3(2.5f, -4, 3),
                            Radius = 1.5f,
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 1,
                                Transparency = 0,
                            },
                        },
                        //left
                        new Sphere
                        {
                            Position = new Vector3(-2.5f, -4, 3),
                            Radius = 1,
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 1,
                                Transparency = 1,
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
                                Reflectance = new Vector3(0, 0, 1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //middle left
                        new Box
                        {
                            Position = new Vector3(-2.0f, -2.0f, -1.0f),
                            Rotation = new Matrix3(
                                new Vector3(0.6f, 0.0f, 0.6f),
                                new Vector3(0.0f, 1.0f, 0.0f),
                                new Vector3(-0.6f, 0.0f, 0.6f)
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
                            Position = new Vector3(2.5f, -3.5f, -1.0f),
                            Rotation = new Matrix3(
                                new Vector3(0.6f, 0.0f, 0.6f),
                                new Vector3(0.0f, 1.0f, 0.0f),
                                new Vector3(-0.6f, 0.0f, 0.6f)
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
                #region Scene3
                new Scene
                {
                    Skybox = SpaceSkybox,//
                    //Mesh = Mesh.LoadFromFile("Meshes/cylinder.fbx", new Vector3(1.5f, 2, 0), new Material
                    //{
                    //    Emittance = new Vector3(0),
                    //    Reflectance = new Vector3(1),
                    //    Smoothness = 0,
                    //    Transparency = 0,
                    //}),
                    OnGUI = s =>
                    {
                        bool result = false;
                        if (ImGui.Button("Lock light sliders"))
                        {
                            s.CustomSettings["Lock light sliders"] = !(bool)s.CustomSettings["Lock light sliders"];
                        }
                        if ((bool)s.CustomSettings["Lock light sliders"])
                        {
                            if (ImGui.SliderFloat("Light strength", ref (s.Boxes[0] as Box).Material.Emittance.X, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light strength", ref (s.Boxes[0] as Box).Material.Emittance.Y, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light strength", ref (s.Boxes[0] as Box).Material.Emittance.Z, 0, 30))
                                result = true;
                        }
                        else
                        {
                            if (ImGui.SliderFloat("Light strength R", ref (s.Boxes[0] as Box).Material.Emittance.X, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light strength G", ref (s.Boxes[0] as Box).Material.Emittance.Y, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light strength B", ref (s.Boxes[0] as Box).Material.Emittance.Z, 0, 30))
                                result = true;
                        }
                        if (ImGui.Button("Lock light 2 sliders"))
                        {
                            s.CustomSettings["Lock light 2 sliders"] = !(bool)s.CustomSettings["Lock light 2 sliders"];
                        }
                        if ((bool)s.CustomSettings["Lock light 2 sliders"])
                        {
                            if (ImGui.SliderFloat("Light 2 strength", ref (s.Boxes[1] as Box).Material.Emittance.X, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light 2 strength", ref (s.Boxes[1] as Box).Material.Emittance.Y, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light 2 strength", ref (s.Boxes[1] as Box).Material.Emittance.Z, 0, 30))
                                result = true;
                        }
                        else
                        {
                            if (ImGui.SliderFloat("Light 2 strength R", ref (s.Boxes[1] as Box).Material.Emittance.X, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light 2 strength G", ref (s.Boxes[1] as Box).Material.Emittance.Y, 0, 30))
                                result = true;
                            if (ImGui.SliderFloat("Light 2 strength B", ref (s.Boxes[1] as Box).Material.Emittance.Z, 0, 30))
                                result = true;
                        }
                        return result;
                    },
                    CustomSettings =
                    {
                        { "Lock light sliders", true },
                        { "Lock light 2 sliders", true },
                    },
                    Spheres = new List<object>
                    {
                    },
                    Boxes = new List<object>
                    {
                        //light
                        new Box
                        {
                            Position = new Vector3(1.5f, 5.9f, -4),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 0.1f, 0.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(8),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //light
                        new Box
                        {
                            Position = new Vector3(1.5f, 5.9f, 4),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 0.1f, 0.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(8),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //bottom
                        new Box
                        {
                            Position = new Vector3(0, -0.5f, 0),
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
                        //Pole
                        new Box
                        {
                            Position = new Vector3(4.5f, 3, -4),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 3, 0.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //Pole top
                        new Box
                        {
                            Position = new Vector3(3f, 6.5f, -4),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(2f, 0.5f, 0.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //Pole
                        new Box
                        {
                            Position = new Vector3(4.5f, 3, 4),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 3, 0.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //Pole top
                        new Box
                        {
                            Position = new Vector3(3f, 6.5f, 4),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(2f, 0.5f, 0.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        
                        //red box
                        new Box
                        {
                            Position = new Vector3(1.5f, 0.5f, -3),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 0.5f, 0.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1, 0, 0),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //green box
                        new Box
                        {
                            Position = new Vector3(1.5f, 0.5f, 0),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 0.5f, 0.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(0, 1, 0),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //blue box
                        new Box
                        {
                            Position = new Vector3(1.5f, 0.5f, 3),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 0.5f, 0.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(0, 0, 1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                    },
                },
                #endregion
                #region Scene4
                new Scene
                {
                    Skybox = SpaceSkybox,//
                    //Mesh = Mesh.LoadFromFile("Meshes/cylinder.fbx", new Vector3(1.5f, 2, 0), new Material
                    //{
                    //    Emittance = new Vector3(0),
                    //    Reflectance = new Vector3(1),
                    //    Smoothness = 0,
                    //    Transparency = 0,
                    //}),
                    Spheres = new List<object>
                    {
                    },
                    Boxes = new List<object>
                    {
                        //bottom
                        new Box
                        {
                            Position = new Vector3(0, -5, 0),
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
                        //top
                        new Box
                        {
                            Position = new Vector3(0, 5, 0),
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
                        //left
                        new Box
                        {
                            Position = new Vector3(-5, 0, 0),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 5, 5),
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
                            Position = new Vector3(5, 0, 0),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 5f, 5),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //front
                        new Box
                        {
                            Position = new Vector3(0, 0, -5),
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
                        //back
                        new Box
                        {
                            Position = new Vector3(0, 0, 5),
                            Rotation = Matrix3.Identity,
                            //Rotation = new Matrix3(
                            //    new Vector3(0.6f, 0.0f, 0.6f),
                            //    new Vector3(0.0f, 1.0f, 0.0f),
                            //    new Vector3(-0.6f, 0.0f, 0.6f)
                            //),
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
                            Position = new Vector3(0, 0, 0),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 0.5f, 0.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(12),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //box1
                        new Box
                        {
                            Position = new Vector3(-2, 0, 0),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(1),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(0, 1, 0),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //box1
                        new Box
                        {
                            Position = new Vector3(2, 0, 0),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(1),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1, 0, 0),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                    },
                },
                #endregion
                #region Scene5
                new Scene
                {
                    Skybox = SpaceSkybox,//
                    Spheres = new List<object>
                    {
                        new Sphere
                        {
                            Position = new Vector3(0, 0, -2),
                            Radius = 0.5f,
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 1,
                                Transparency = 1,
                            },
                        },
                    },
                    Boxes = new List<object>
                    {
                        //bottom
                        new Box
                        {
                            Position = new Vector3(0, -5, -2.5f),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(5, 0.5f, 7.5f),
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
                            Position = new Vector3(0, 5, -2.5f),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(5, 0.5f, 7.5f),
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
                            Position = new Vector3(-5, 0, -2.5f),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 5, 7.5f),
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
                            Position = new Vector3(5, 0, -2.5f),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 5f, 7.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //front
                        new Box
                        {
                            Position = new Vector3(0, 0, -10),
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
                        //back
                        new Box
                        {
                            Position = new Vector3(0, 0, 5),
                            Rotation = Matrix3.Identity,
                            //Rotation = new Matrix3(
                            //    new Vector3(0.6f, 0.0f, 0.6f),
                            //    new Vector3(0.0f, 1.0f, 0.0f),
                            //    new Vector3(-0.6f, 0.0f, 0.6f)
                            //),
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
                            Position = new Vector3(0, 0, 4.5f),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f, 0.5f, 0.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(12),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //box1
                        new Box
                        {
                            Position = new Vector3(-1, 0, 4),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.4f, 0.6f, 3),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(0.2f),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //box2
                        new Box
                        {
                            Position = new Vector3(1, 0, 4),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.4f, 0.6f, 3),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(0.2f),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //box3
                        new Box
                        {
                            Position = new Vector3(0, 1, 4),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.6f, 0.4f, 3),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(0.2f),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //box4
                        new Box
                        {
                            Position = new Vector3(0, -1, 4),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.6f, 0.4f, 3),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(0.2f),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //box5
                        new Box
                        {
                            Position = new Vector3(-1, 0, -2),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //box6
                        new Box
                        {
                            Position = new Vector3(1, 0, -2),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(0.5f),
                            Material = new Material
                            {
                                Emittance = new Vector3(0),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                        //box7
                        //new Box
                        //{
                        //    Position = new Vector3(0, 0, -2),
                        //    Rotation = Matrix3.Identity,
                        //    HalfSize = new Vector3(0.5f),
                        //    Material = new Material
                        //    {
                        //        Emittance = new Vector3(0),
                        //        Reflectance = new Vector3(1),
                        //        Smoothness = 1,
                        //        Transparency = 1,
                        //    },
                        //},
                    },
                },
                #endregion
                #region Scene5
                new Scene
                {
                    Skybox = defaultSkybox,//
                    Mesh = Mesh.LoadFromFile("Meshes/test2.fbx", new Vector3(0), new Material
                    {
                        Emittance = new Vector3(0),
                        Reflectance = new Vector3(1),
                        Smoothness = 0,
                        Transparency = 0,
                    }),
                    Spheres = new List<object>
                    {
                        new Sphere
                        {
                            Position = new Vector3(0, 0, 4),
                            Radius = 1,
                            Material = new Material
                            {
                                Emittance = new Vector3(6),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                    },
                    Boxes = new List<object>
                    {
                        new Box
                        {
                            Position = new Vector3(0, 0, -4),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(3, 3, 0.5f),
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
                #region Scene6
                new Scene
                {
                    Skybox = defaultSkybox,//
                    Mesh = Mesh.LoadFromFile("Meshes/cylinder.fbx", new Vector3(0), new Material
                    {
                        Emittance = new Vector3(0),
                        Reflectance = new Vector3(1),
                        Smoothness = 0,
                        Transparency = 0,
                    }),
                    Spheres = new List<object>
                    {
                        new Sphere
                        {
                            Position = new Vector3(0, 0, 4),
                            Radius = 1,
                            Material = new Material
                            {
                                Emittance = new Vector3(6),
                                Reflectance = new Vector3(1),
                                Smoothness = 0,
                                Transparency = 0,
                            },
                        },
                    },
                    Boxes = new List<object>
                    {
                        new Box
                        {
                            Position = new Vector3(0, 0, -4),
                            Rotation = Matrix3.Identity,
                            HalfSize = new Vector3(3, 3, 0.5f),
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
            ptShader.Use();
            ptShader.SetStruct(Camera, "Camera");
            ptShader.SetStruct(PathTracingSettings, "Settings");
            ptShader.SetInt("Samples", samples);
            var timeSeed = (float)stopwatch.Elapsed.TotalSeconds;
            Title = timeSeed.ToString();
            ptShader.SetFloat("Time", timeSeed);
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
            if (HighQualityMode)
            {
                Directory.CreateDirectory("Screenshots/" + HighQualityModeDate.ToString($"HH-mm-ss_dd-MM-yyyy"));
                SaveScreenshot(HighQualityModeDate.ToString($"HH-mm-ss_dd-MM-yyyy'/HQ_'{HighQualityModeIterations}_"));
                if (--HighQualityModeIterations <= 0)
                {
                    HighQualityMode = false;
                    Samples = DefaultSamples;
                    PathTracingSettings.Depth = PathTracingSettings.DefaultDepth;
                    HighQualityModeIterations = DefaultHighQualityModeIterations;
                }
            }
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
                ImGui.Text("C - show/hide cursor\nEsc - quit\nWASD, ctrl, space - move\nleft | right - change scenes");
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
                ImGui.SliderInt("Gamma", ref PostProcessingSettings.Gamma, 1, 40, "%d", ImGuiSliderFlags.AlwaysClamp);
                if (ImGui.Button("Skybox light"))
                {
                    PathTracingSettings.UseSkyboxForLighting = !PathTracingSettings.UseSkyboxForLighting;
                    ClearAccumulator = true;
                }
                
                ImGui.Text("High quality mode");
                ImGui.SliderInt("High quality mode samples", ref HighQualityModeSamples, 1, 500);
                ImGui.SliderInt("High quality mode depth", ref HighQualityModeDepth, 1, 64, "%d");
                ImGui.SliderInt("High quality mode iterations", ref HighQualityModeIterations, 1, 1000, "%d");
                //ImGui.SliderInt3("LOL", ref HighQualityModeSamples, 1, 1000, "%d");
                if (ImGui.Button("Enable high quality mode"))
                {
                    HighQualityMode = true;
                    Samples = HighQualityModeSamples;
                    PathTracingSettings.Depth = HighQualityModeDepth;
                    ClearAccumulator = true;
                    HighQualityModeDate = DateTime.Now;
                }
                ImGui.BeginGroup();
                ImGui.Text("Misc");
                if (ImGui.Button("Reset frame accumulator"))
                    ClearAccumulator = true;
                if (ImGui.Button("Fullscreen"))
                {
                    if (WindowState != WindowState.Fullscreen)
                        WindowState = WindowState.Fullscreen;
                    else
                        WindowState = WindowState.Maximized;
                }
                if (ImGui.Button("Save image"))
                {
                    SaveScreenshot(DateTime.Now.ToString("HH-mm-ss_dd-MM-yyyy"));
                }
                if (ImGui.Button("Close app"))
                    Close();
                ImGui.EndGroup();
                if (ImGui.Button("Previous scene"))
                {
                    ChangeSceneLeft();
                }
                ImGui.SameLine();
                if (ImGui.Button("Next Scene"))
                {
                    ChangeSceneRight();
                }
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
        private void SaveScreenshot(string name)
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
            bmp.Save($"Screenshots/{name}.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
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
            else
            {
                if (!AccumulateFrames)
                {
                    AccumulateFrames = true;
                    AccumulatedFrames = 0;
                    ClearAccumulator = true;
                }
            }
            //Title = AccumulatedFrames.ToString();
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            Camera.ViewportSize = ClientSize;
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
