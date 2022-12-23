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
using LearnOpenTK.Common;
using System.Text.RegularExpressions;
using StbImageSharp;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Assignment6
{
    public class Window : GameWindow
    {
        private static Stopwatch Stopwatch;
        private bool Wireframe = false;
        private bool CullFace = false;
        private List<Scene> Scenes = new List<Scene>();
        private int CurrentScene;
        public static Shader DefaultShader;
        public static float TotalTime;
        private Camera camera;
        private float objectMoveSpeed = 0.5f;
        private float objectRotationSensitivity = 30;
        private float objectScaleSpeed = 0.5f;
        private float zoomSpeed = 60;
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            VSync = VSyncMode.Adaptive;
            GL.ClearColor(Color4.DarkGray);
            GL.Enable(EnableCap.DepthTest);
            DefaultShader = new Shader("Shaders/DefaultShader.vs", "Shaders/DefaultShader.fs");
            DefaultShader.Use();
            camera = new Camera(new Vector3(0, 0, 4), 1);
            camera.Fov = 70;
            //camera.Pitch = -40;
            //texture
            var tex1 = Texture.LoadFromFile("Textures/grass.jpg");
            //camera.Yaw = -125;
            var light = new Light
            {
                Position = new Vector3(-2, 2, 3),
                Ambient = new Vector3(0.5f),
                Diffuse = new Vector3(0.5f),
                Specular = new Vector3(1),
            };
            var light2 = new Light
            {
                Position = new Vector3(0),
                Ambient = new Vector3(0.5f),
                Diffuse = new Vector3(0.5f),
                Specular = new Vector3(1),
            };
            //
            var groundMaterial = new Material
            {
                Ambient = new Vector3(0.027f, 0.278f, 0.054f),
                Diffuse = new Vector3(0.027f, 0.278f, 0.054f),
                Specular = new Vector3(0),
                Shininess = 32.0f,
                DiffuseMap = tex1,
            };
            var material = new Material
            {
                Ambient = new Vector3(1.0f, 0.5f, 0.31f),
                Diffuse = new Vector3(1.0f, 0.5f, 0.31f),
                Specular = new Vector3(0.5f, 0.5f, 0.5f),
                Shininess = 32.0f,
            };
            var material3 = new Material
            {
                Ambient = new Vector3(0, 1, 0),
                Diffuse = new Vector3(0, 1, 0),
                Specular = new Vector3(0.5f, 0.5f, 0.5f),
                Shininess = 32.0f,
            };
            var cubeMesh = MeshGenerator.Cube();
            var pyramidMesh = MeshGenerator.Pyramid();
            var trapezoidMesh = MeshGenerator.Trapezoid();
            var octahedronMesh = MeshGenerator.Octahedron();
            var regularPyramidMesh = MeshGenerator.Cone(12);
            var coneMesh = MeshGenerator.Cone(100);
            var cylinderMesh = MeshGenerator.Cylinder(40, 0.25f, 0.5f);
            var sphereMesh = MeshGenerator.Sphere(30, 30);
            var helixMesh = MeshGenerator.Helix(20, 0.005f);
            var torusMesh = MeshGenerator.Torus(20, 80, 0.5f, 0.2f);
            //var torusMesh = MeshGenerator.CustomTorus(10, 0.5f, new List<Vector3>
            //{
            //    new Vector3(0, 0.15f, 0),
            //    new Vector3(0, 0, 0.15f),
            //    new Vector3(0, -0.15f, 0),
            //    //new Vector3(0, -0.15f, 0.15f),
            //});
            var regularDodecahedronMesh = MeshGenerator.RegularDodecahedron();
            var regularTetrahedronMesh = MeshGenerator.RegularTetrahedron();
            var regularIcosahedronMesh = MeshGenerator.RegularIcosahedron();
            var ground = new MeshRenderer(new Transform(position: new Vector3(0, -2, 0), scale: new Vector3(10, 0.1f, 10)), groundMaterial, cubeMesh);
            ground.IgnoreUserTransformations = true;
            var cube = new MeshRenderer(new Transform(), groundMaterial, cubeMesh);
            var pyramid = new MeshRenderer(new Transform(), material, pyramidMesh);
            var trapezoid = new MeshRenderer(new Transform(), material, trapezoidMesh);
            var octahedron = new MeshRenderer(new Transform(), material, octahedronMesh);
            var regularPyramid = new MeshRenderer(new Transform(), material, regularPyramidMesh);
            var cone = new MeshRenderer(new Transform(), material, coneMesh);
            var cylinder = new MeshRenderer(new Transform(), material, cylinderMesh);
            var sphere = new MeshRenderer(new Transform(), material, sphereMesh);
            sphere.IgnoreUserTransformations = true;
            var helix = new MeshRenderer(new Transform(), material, helixMesh);
            var torus = new MeshRenderer(new Transform(), material, torusMesh);
            var regularDodecahedron = new MeshRenderer(new Transform(), material, regularDodecahedronMesh);
            var regularTetrahedron = new MeshRenderer(new Transform(), material, regularTetrahedronMesh);
            var regularIcosahedron = new MeshRenderer(new Transform(), material, regularIcosahedronMesh);
            sphere.Func = new Action<Transform>((t) =>
            {
                t.Position = new Vector3(MathF.Cos(TotalTime + MathF.PI / 2f), t.Position.Y, MathF.Sin(TotalTime + MathF.PI / 2f));
                //t.Rotation = new Vector3(0, TotalTime * 4, t.Rotation.Z);
            });
            //ak-12
            var akTexture = Texture.LoadFromFile("Textures/ak_tex.png");
            var akTexturedMetal = new Material
            {
                Ambient = new Vector3(0.3f, 0.3f, 0.3f),
                Diffuse = new Vector3(0.3f, 0.3f, 0.3f),
                Specular = new Vector3(0.5f, 0.5f, 0.5f),
                Shininess = 16,
                DiffuseMap = akTexture,
            };
            var akTexturedPolymer = new Material
            {
                Ambient = new Vector3(0.3f, 0.3f, 0.3f),
                Diffuse = new Vector3(0.3f, 0.3f, 0.3f),
                Specular = new Vector3(0.1f, 0.1f, 0.1f),
                Shininess = 256.0f,
                DiffuseMap = akTexture,
            };
            var akMetal = new Material
            {
                //27 31 31
                Ambient = new Vector3(27f / 255f, 31f / 255f, 31f / 255f),
                Diffuse = new Vector3(27f / 255f, 31f / 255f, 31f / 255f),
                Specular = new Vector3(0.5f, 0.5f, 0.5f),
                Shininess = 16,
            };
            var akPolymer = new Material
            {
                //92 100 105
                Ambient = new Vector3(92f / 255f, 100f / 255f, 105f / 255f),
                Diffuse = new Vector3(92f / 255f, 100f / 255f, 105f / 255f),
                Specular = new Vector3(0.2f, 0.2f, 0.2f),
                Shininess = 256.0f,
            };
            var metalRectCoords = new Vector2[]
            {
                new Vector2(0, 0.048f),
                new Vector2(0, 0),
                new Vector2(0.09f, 0),
                new Vector2(0.09f, 0.048f),
            };
            var polymerRectCoords = new Vector2[]
            {
                new Vector2(0, 0.376f),
                new Vector2(0, 0.334f),
                new Vector2(0.07f, 0.334f),
                new Vector2(0.07f, 0.376f),
            };
            var akParts = new List<MeshRenderer>();
            akParts.Add(ground);
            var halfWidth = 0.05f;
            #region stock
            {
                var texCoords = new List<Vector2[]>();
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.604f, 0.132f),
                    new Vector2(0.6f, 0.097f),
                    new Vector2(0.496f, 0.101f),
                    new Vector2(0.499f, 0.133f),
                });
                texCoords.Add(polymerRectCoords);
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.499f, 0.133f),
                    new Vector2(0.496f, 0.101f),
                    new Vector2(0.6f, 0.097f),
                    new Vector2(0.604f, 0.132f),
                });
                texCoords.Add(polymerRectCoords);
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.0537f, 0.2036f),
                    new Vector2(0.1436f, 0.2012f),
                    new Vector2(0.1431f, 0.1855f),
                    new Vector2(0.0537f, 0.1865f),
                });
                texCoords.Add(polymerRectCoords);
                var length = 0.2f;
                var offset = -0.55f;
                var stockTop = new Vector3[]
                {
                    new Vector3(-length-0.04f, 0.01f-0.03f, halfWidth-0.01f),
                    new Vector3(0, -0.025f, halfWidth),
                    new Vector3(0, -0.025f, -halfWidth),
                    new Vector3(-length-0.04f, 0.01f-0.03f, -halfWidth+0.01f),
                };
                var stockBottom = new Vector3[]
                {
                    new Vector3(-length-0.04f, -0.12f, halfWidth-0.01f),
                    new Vector3(0, -0.12f, halfWidth),
                    new Vector3(0, -0.12f, -halfWidth),
                    new Vector3(-length-0.04f, -0.12f, -halfWidth+0.01f),
                };
                var stock = MeshGenerator.Parallelepiped(stockTop, stockBottom, new Vector3(offset, 0, 0), texCoords.ToArray());
                akParts.Add(new MeshRenderer(new Transform(), akTexturedPolymer, stock));
            }
            #endregion
            #region butt pad
            {
                var texCoords = new List<Vector2[]>();
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.016f, 0.325f),
                    new Vector2(0.021f, 0.24f),
                    new Vector2(0.051f, 0.24f),
                    new Vector2(0.062f, 0.325f),
                });
                texCoords.Add(polymerRectCoords);
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.593f, 0.132f),
                    new Vector2(0.606f, 0.036f),
                    new Vector2(0.640f, 0.036f),
                    new Vector2(0.645f, 0.132f),
                });
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.667f, 0.318f),
                    new Vector2(0.683f, 0.035f),
                    new Vector2(0.740f, 0.035f),
                    new Vector2(0.756f, 0.318f),
                });
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.0283f, 0.2036f),
                    new Vector2(0.0537f, 0.2036f),
                    new Vector2(0.0537f, 0.1865f),
                    new Vector2(0.0283f, 0.1865f),
                });
                texCoords.Add(polymerRectCoords);
                var length = 0.2f;
                var offset = -0.75f;
                var buttPadTop = new Vector3[]
                {
                    new Vector3(-length, 0.01f-0.025f, halfWidth),
                    new Vector3(0, 0.01f-0.025f, halfWidth),
                    new Vector3(0, 0.01f-0.025f, -halfWidth),
                    new Vector3(-length, 0.01f-0.025f, -halfWidth),
                };
                var buttPadBottom = new Vector3[]
                {
                    new Vector3(-length, -0.3f, halfWidth-0.02f),
                    new Vector3(-length / 2, -0.3f, halfWidth-0.02f),
                    new Vector3(-length / 2, -0.3f, -halfWidth+0.02f),
                    new Vector3(-length, -0.3f, -halfWidth+0.02f),
                };
                var buttPad = MeshGenerator.Parallelepiped(buttPadTop, buttPadBottom, new Vector3(offset, 0, 0), texCoords.ToArray());
                akParts.Add(new MeshRenderer(new Transform(), akTexturedPolymer, buttPad));
            }
            #endregion
            #region grip
            {
                var texCoords = new List<Vector2[]>();
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.1572f, 0.2954f),
                    new Vector2(0.1665f, 0.2783f),
                    new Vector2(0.189f, 0.2725f),
                    new Vector2(0.1978f, 0.2964f),
                });
                texCoords.Add(polymerRectCoords);
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.4487f, 0.0996f),
                    new Vector2(0.4551f, 0.0723f),
                    new Vector2(0.481f, 0.0786f),
                    new Vector2(0.4873f, 0.0996f),
                });
                texCoords.Add(polymerRectCoords);
                texCoords.Add(polymerRectCoords);
                texCoords.Add(polymerRectCoords);
                var texCoords1 = new List<Vector2[]>();
                texCoords1.Add(new Vector2[]
                {
                    new Vector2(0.1665f, 0.2783f),
                    new Vector2(0.1421f, 0.2275f),
                    new Vector2(0.1753f, 0.2188f),
                    new Vector2(0.189f, 0.2725f),
                });
                texCoords1.Add(polymerRectCoords);
                texCoords1.Add(new Vector2[]
                {
                    new Vector2(0.4551f, 0.0723f),
                    new Vector2(0.4702f, 0.022f),
                    new Vector2(0.5044f, 0.0283f),
                    new Vector2(0.481f, 0.0786f),
                });
                texCoords1.Add(polymerRectCoords);
                texCoords1.Add(polymerRectCoords);
                texCoords1.Add(polymerRectCoords);
                var length = 0.1f;
                var skew = -0.05f;
                var offset = -0.45f;
                var gripTop = new Vector3[]
                {
                    new Vector3(-length, -0.12f, halfWidth - 0.01f),
                    new Vector3(length / 2, -0.1f, halfWidth - 0.01f),
                    new Vector3(length / 2, -0.1f, -halfWidth + 0.01f),
                    new Vector3(-length, -0.12f, -halfWidth + 0.01f),
                };
                var gripMiddle = new Vector3[]
                {
                    new Vector3(-length / 2, -0.16f, halfWidth - 0.01f),
                    new Vector3(length / 2, -0.2f, halfWidth - 0.01f),
                    new Vector3(length / 2, -0.2f, -halfWidth + 0.01f),
                    new Vector3(-length / 2, -0.16f, -halfWidth + 0.01f),
                };
                var gripBottom = new Vector3[]
                {
                    new Vector3(-length + skew, -0.32f, halfWidth - 0.01f),
                    new Vector3(length / 2 + skew, -0.35f, halfWidth - 0.01f),
                    new Vector3(length / 2 + skew, -0.35f, -halfWidth + 0.01f),
                    new Vector3(-length + skew, -0.32f, -halfWidth + 0.01f),
                };
                var grip = MeshGenerator.Parallelepiped(gripTop, gripMiddle, new Vector3(offset, 0, 0), texCoords.ToArray());
                var grip1 = MeshGenerator.Parallelepiped(gripMiddle, gripBottom, new Vector3(offset, 0, 0), texCoords1.ToArray());
                akParts.Add(new MeshRenderer(new Transform(), akTexturedPolymer, grip));
                akParts.Add(new MeshRenderer(new Transform(), akTexturedPolymer, grip1));
            }
            #endregion
            #region trigger guard
            {
                var length = 0.1f;
                var offset = -0.35f;
                var left = new Vector3[]
                {
                    new Vector3(-length / 2, -0.2f, halfWidth / 2),
                    new Vector3(-length / 2, -0.18f, halfWidth / 2),
                    new Vector3(-length / 2, -0.18f, -halfWidth / 2),
                    new Vector3(-length / 2, -0.2f, -halfWidth / 2),
                };
                var middle = new Vector3[]
                {
                    new Vector3(0, -0.22f, halfWidth / 2),
                    new Vector3(0, -0.2f, halfWidth / 2),
                    new Vector3(0, -0.2f, -halfWidth / 2),
                    new Vector3(0, -0.22f, -halfWidth / 2),
                };
                var right = new Vector3[]
                {
                    new Vector3(length / 2, -0.21f, halfWidth / 2),
                    new Vector3(length / 2, -0.19f, halfWidth / 2),
                    new Vector3(length / 2, -0.19f, -halfWidth / 2),
                    new Vector3(length / 2, -0.21f, -halfWidth / 2),
                };
                var top = new Vector3[]
                {
                    new Vector3(length / 2, -0.1f, halfWidth / 2),
                    new Vector3(length, -0.1f, halfWidth / 2),
                    new Vector3(length, -0.1f, -halfWidth / 2),
                    new Vector3(length / 2, -0.1f, -halfWidth / 2),
                };
                var bottom = new Vector3[]
                {
                    new Vector3(length / 2, -0.21f, halfWidth / 2),
                    new Vector3(length, -0.19f, halfWidth / 2),
                    new Vector3(length, -0.19f, -halfWidth / 2),
                    new Vector3(length / 2, -0.21f, -halfWidth / 2),
                };
                var triggerGuard = MeshGenerator.Parallelepiped(left, middle, new Vector3(offset, 0, 0));
                var triggerGuard1 = MeshGenerator.Parallelepiped(middle, right, new Vector3(offset, 0, 0));
                var triggerGuard2 = MeshGenerator.Parallelepiped(top, bottom, new Vector3(offset, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, triggerGuard));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, triggerGuard1));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, triggerGuard2));
            }
            #endregion
            #region trigger
            {
                var length = 0.04f;
                var offset = -0.37f;
                var top = new Vector3[]
                {
                    new Vector3(-length / 2, -0.1f, halfWidth / 3),
                    new Vector3(0, -0.1f, halfWidth / 3),
                    new Vector3(0, -0.1f, -halfWidth / 3),
                    new Vector3(-length / 2, -0.1f, -halfWidth / 3),
                };
                var section1 = top.Select(i => i + new Vector3(0.01f, -0.02f, 0)).ToArray();
                var section2 = section1.Select(i => i + new Vector3(0.01f, -0.02f, 0)).ToArray();
                var section3 = section2.Select(i => i + new Vector3(0.01f, -0.02f, 0)).ToArray();
                var bottom = section3.Select(i => i + new Vector3(0.02f, -0.02f, 0)).ToArray();
                var trigger = MeshGenerator.Parallelepiped(top, section1, new Vector3(offset, 0, 0));
                var trigger1 = MeshGenerator.Parallelepiped(section1, section2, new Vector3(offset, 0, 0));
                var trigger2 = MeshGenerator.Parallelepiped(section2, section3, new Vector3(offset, 0, 0));
                var trigger3 = MeshGenerator.Parallelepiped(section3, bottom, new Vector3(offset, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, trigger));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, trigger1));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, trigger2));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, trigger3));
            }
            #endregion
            #region magazine
            {
                var texCoords = new List<Vector2[]>();
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.25f, 0.30f),
                    new Vector2(0.27f, 0.2f),
                    new Vector2(0.319f, 0.213f),
                    new Vector2(0.305f, 0.302f),
                });
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.340f, 0.295f),
                    new Vector2(0.340f, 0.180f),
                    new Vector2(0.365f, 0.180f),
                    new Vector2(0.365f, 0.295f),
                });
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.335f, 0.104f),
                    new Vector2(0.32f, 0.014f),
                    new Vector2(0.374f, 0.002f),
                    new Vector2(0.389f, 0.102f),
                });
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.340f, 0.295f),
                    new Vector2(0.340f, 0.180f),
                    new Vector2(0.365f, 0.180f),
                    new Vector2(0.365f, 0.295f),
                });
                texCoords.Add(polymerRectCoords);
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.379f, 0.291f),
                    new Vector2(0.387f, 0.186f),
                    new Vector2(0.416f, 0.186f),
                    new Vector2(0.42f, 0.291f),
                });
                var length = 0.2f;
                var offset = -0.05f;
                var skew = 0.05f;
                var magazineTop = new Vector3[]
                {
                    new Vector3(-length, -0.1f, halfWidth - 0.01f),
                    new Vector3(0, -0.1f, halfWidth - 0.03f),
                    new Vector3(0, -0.1f, -halfWidth + 0.03f),
                    new Vector3(-length, -0.1f, -halfWidth + 0.01f),
                };
                var magazineBottom = new Vector3[]
                {
                    new Vector3(-length + skew, -0.45f, halfWidth - 0.01f),
                    new Vector3(skew, -0.4f, halfWidth - 0.03f),
                    new Vector3(skew, -0.4f, -halfWidth + 0.03f),
                    new Vector3(-length + skew, -0.45f, -halfWidth + 0.01f),
                };
                var magazine = MeshGenerator.Parallelepiped(magazineTop, magazineBottom, new Vector3(offset, 0, 0), texCoords.ToArray());
                akParts.Add(new MeshRenderer(new Transform(), akTexturedPolymer, magazine));
            }
            #endregion
            #region lower receiver
            {
                var texCoords = new List<Vector2[]>();
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.156f, 0.325f),
                    new Vector2(0.156f, 0.296f),
                    new Vector2(0.336f, 0.305f),
                    new Vector2(0.336f, 0.325f),
                });
                texCoords.Add(metalRectCoords);
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.304f, 0.131f),
                    new Vector2(0.304f, 0.108f),
                    new Vector2(0.497f, 0.101f),
                    new Vector2(0.497f, 0.131f),
                });
                texCoords.Add(metalRectCoords);
                texCoords.Add(metalRectCoords);
                texCoords.Add(metalRectCoords);
                var length = 0.55f;
                var bottom = -0.1f;
                var top = -0.025f;
                var lowerReceiverTop = new Vector3[]
                {
                    new Vector3(-length, top, halfWidth),
                    new Vector3(0, top, halfWidth),
                    new Vector3(0, top, -halfWidth),
                    new Vector3(-length, top, -halfWidth),
                };
                var lowerReceiverBottom = new Vector3[]
                {
                    new Vector3(-length, bottom - 0.02f, halfWidth),
                    new Vector3(0, bottom, halfWidth),
                    new Vector3(0, bottom, -halfWidth),
                    new Vector3(-length, bottom - 0.02f, -halfWidth),
                };
                var lowerReceiver = MeshGenerator.Parallelepiped(lowerReceiverTop, lowerReceiverBottom, new Vector3(0, 0, 0), texCoords.ToArray());
                akParts.Add(new MeshRenderer(new Transform(), akTexturedMetal, lowerReceiver));
            }
            #endregion
            #region dust cover
            {
                var texCoords = new List<Vector2[]>();
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.17f, 0.345f),
                    new Vector2(0.156f, 0.325f),
                    new Vector2(0.336f, 0.325f),
                    new Vector2(0.336f, 0.345f),
                });
                texCoords.Add(metalRectCoords);
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.304f, 0.154f),
                    new Vector2(0.304f, 0.131f),
                    new Vector2(0.497f, 0.131f),
                    new Vector2(0.475f, 0.154f),
                });
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.52f, 0.199f),
                    new Vector2(0.507f, 0.143f),
                    new Vector2(0.553f, 0.143f),
                    new Vector2(0.543f, 0.199f),
                });
                texCoords.Add(metalRectCoords);
                texCoords.Add(metalRectCoords);
                var length = 0.55f;
                var dustCoverTop = new Vector3[]
                {
                    new Vector3(-length + 0.05f, 0.08f, halfWidth / 2),
                    new Vector3(0, 0.08f, halfWidth / 2),
                    new Vector3(0, 0.08f, -halfWidth / 2),
                    new Vector3(-length + 0.05f, 0.08f, -halfWidth / 2),
                };
                var dustCoverBottom = new Vector3[]
                {
                    new Vector3(-length, 0, halfWidth),
                    new Vector3(0, 0, halfWidth),
                    new Vector3(0, 0, -halfWidth),
                    new Vector3(-length, 0, -halfWidth),
                };
                var dustCover = MeshGenerator.Parallelepiped(dustCoverTop, dustCoverBottom, new Vector3(0, -0.025f, 0), texCoords.ToArray());
                akParts.Add(new MeshRenderer(new Transform(), akTexturedMetal, dustCover));
            }
            #endregion
            #region rear sight
            {
                var length = 0.1f;
                var offset = -0.4f;
                var baseTop = new Vector3[]
                {
                    new Vector3(-length / 2, 0.09f, halfWidth / 2),
                    new Vector3(0, 0.075f, halfWidth / 2),
                    new Vector3(0, 0.075f, -halfWidth / 2),
                    new Vector3(-length / 2, 0.09f, -halfWidth / 2),
                };
                var baseBottom = new Vector3[]
                {
                    new Vector3(-length, 0.055f, halfWidth / 2),
                    new Vector3(0, 0.055f, halfWidth / 2),
                    new Vector3(0, 0.055f, -halfWidth / 2),
                    new Vector3(-length, 0.055f, -halfWidth / 2),
                };
                var sightBase = MeshGenerator.Parallelepiped(baseTop, baseBottom, new Vector3(offset, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, sightBase));
                var sightBase2 = MeshGenerator.Cylinder(20, 0.025f, 0.025f, 0.092f, 0.055f, new Vector3(offset - 0.075f, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, sightBase2));
                var sightRing = MeshGenerator.TorusYZ(20, 20, 0.01f, 0.005f, new Vector3(offset - 0.075f, 0.1f, 0));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, sightRing));
            }
            #endregion
            #region dust cover rail
            {
                var length = 0.4f;
                var step = 0.01f;
                var baseTop = new Vector3[]
                {
                    new Vector3(-length, 0.09f, halfWidth / 1.5f),
                    new Vector3(0.34f, 0.09f, halfWidth / 1.5f),
                    new Vector3(0.34f, 0.09f, -halfWidth / 1.5f),
                    new Vector3(-length, 0.09f, -halfWidth / 1.5f),
                };
                var baseBottom = new Vector3[]
                {
                    new Vector3(-length, 0.08f, halfWidth / 2),
                    new Vector3(0.34f, 0.08f, halfWidth / 2),
                    new Vector3(0.34f, 0.08f, -halfWidth / 2),
                    new Vector3(-length, 0.08f, -halfWidth / 2),
                };
                var railBase = MeshGenerator.Parallelepiped(baseTop, baseBottom, new Vector3(0, -0.025f, 0));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, railBase));
                var segmentTop = new Vector3[]
                {
                    new Vector3(-step / 2, 0.01f, halfWidth / 3),
                    new Vector3(step / 2, 0.01f, halfWidth / 3),
                    new Vector3(step / 2, 0.01f, -halfWidth / 3),
                    new Vector3(-step / 2, 0.01f, -halfWidth / 3),
                };
                var segmentBottom = segmentTop.Select(i => new Vector3(i.X, 0, MathF.Sign(i.Z) * halfWidth / 1.5f)).ToArray();
                for (var x = -length + step / 2; x < 0.34f; x += step * 2)
                {
                    var segment = MeshGenerator.Parallelepiped(segmentTop, segmentBottom, new Vector3(x, 0.09f - 0.025f, 0));
                    akParts.Add(new MeshRenderer(new Transform(), akMetal, segment));
                }
            }
            #endregion
            #region handguard
            {
                var texCoords = new List<Vector2[]>();
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.336f, 0.35f),
                    new Vector2(0.336f, 0.304f),
                    new Vector2(0.447f, 0.304f),
                    new Vector2(0.447f, 0.35f),
                });
                texCoords.Add(polymerRectCoords);
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.194f, 0.152f),
                    new Vector2(0.194f, 0.107f),
                    new Vector2(0.303f, 0.107f),
                    new Vector2(0.303f, 0.152f),
                });
                texCoords.Add(polymerRectCoords);
                texCoords.Add(polymerRectCoords);
                texCoords.Add(polymerRectCoords);
                var length = 0.34f;
                var handGuardTop = new Vector3[]
                {
                    new Vector3(0, 0.08f-0.025f, halfWidth / 2),
                    new Vector3(length, 0.08f-0.025f, halfWidth / 2),
                    new Vector3(length, 0.08f-0.025f, -halfWidth / 2),
                    new Vector3(0, 0.08f-0.025f, -halfWidth / 2),
                };
                var handGuardBottom = new Vector3[]
                {
                    new Vector3(0, -0.1f, halfWidth),
                    new Vector3(length, -0.1f, halfWidth),
                    new Vector3(length, -0.1f, -halfWidth),
                    new Vector3(0, -0.1f, -halfWidth),
                };
                var handGuardBottom2 = new Vector3[]
                {
                    new Vector3(0 + 0.01f, -0.12f, halfWidth / 2),
                    new Vector3(length - 0.01f, -0.12f, halfWidth / 2),
                    new Vector3(length - 0.01f, -0.12f, -halfWidth / 2),
                    new Vector3(0 + 0.01f, -0.12f, -halfWidth / 2),
                };
                var handGuard = MeshGenerator.Parallelepiped(handGuardTop, handGuardBottom, new Vector3(0, 0, 0), texCoords.ToArray());
                akParts.Add(new MeshRenderer(new Transform(), akTexturedPolymer, handGuard));
                var handGuard1 = MeshGenerator.Parallelepiped(handGuardBottom, handGuardBottom2, new Vector3(0, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akPolymer, handGuard1));
            }
            #endregion
            #region bottom rail
            {
                var length = 0.34f;
                var step = 0.01f;
                
                var baseTop = new Vector3[]
                {
                    new Vector3(0 + 0.01f, -0.12f, halfWidth / 2),
                    new Vector3(length - 0.01f, -0.12f, halfWidth / 2),
                    new Vector3(length - 0.01f, -0.12f, -halfWidth / 2),
                    new Vector3(0 + 0.01f, -0.12f, -halfWidth / 2),
                };
                var baseBottom = new Vector3[]
                {
                    new Vector3(0 + 0.01f, -0.13f, halfWidth / 1.5f),
                    new Vector3(length - 0.01f, -0.13f, halfWidth / 1.5f),
                    new Vector3(length - 0.01f, -0.13f, -halfWidth / 1.5f),
                    new Vector3(0 + 0.01f, -0.13f, -halfWidth / 1.5f),
                };
                var railBase = MeshGenerator.Parallelepiped(baseTop, baseBottom, new Vector3(0, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, railBase));
                var segmentTop = new Vector3[]
                {
                    new Vector3(-step / 2, 0, halfWidth / 1.5f),
                    new Vector3(step / 2, 0, halfWidth / 1.5f),
                    new Vector3(step / 2, 0, -halfWidth / 1.5f),
                    new Vector3(-step / 2, 0, -halfWidth / 1.5f),
                };
                var segmentBottom = segmentTop.Select(i => new Vector3(i.X, -0.01f, MathF.Sign(i.Z) * halfWidth / 3)).ToArray();
                for (var x = 0 + 0.01f + step / 2; x < length - 0.01f; x += step * 2)
                {
                    var segment = MeshGenerator.Parallelepiped(segmentTop, segmentBottom, new Vector3(x, -0.13f, 0));
                    akParts.Add(new MeshRenderer(new Transform(), akMetal, segment));
                }
            }
            #endregion
            #region barrel
            {
                var offset = 0.34f;
                var radius = 0.035f;
                var length = 0.45f;
                var left = new Vector3(offset, -0.05f, 0);
                var right = new Vector3(offset + length, -0.05f, 0);
                var barrel = MeshGenerator.CylinderX(40, radius, radius, left, right);
                akParts.Add(new MeshRenderer(new Transform(), akMetal, barrel));
            }
            #endregion
            #region gas tube
            {
                var offset = 0.34f;
                var radius = 0.03f;
                var length = 0.25f;
                var left = new Vector3(offset, 0.055f - 0.032f, 0);
                var right = new Vector3(offset + length, 0.055f - 0.032f, 0);
                var gasTube = MeshGenerator.CylinderX(6, radius, radius, left, right);
                akParts.Add(new MeshRenderer(new Transform(), akMetal, gasTube));
            }
            #endregion
            #region gas tube connector
            {
                var offset = 0.52f;
                var length = 0.1f;
                var top = new Vector3[]
                {
                    new Vector3(-length / 2, 0.08f, halfWidth / 2 - 0.01f),
                    new Vector3(length / 2, 0.08f, halfWidth / 2 - 0.01f),
                    new Vector3(length / 2, 0.08f, -halfWidth / 2 + 0.01f),
                    new Vector3(-length / 2, 0.08f, -halfWidth / 2 + 0.01f),
                };
                var bottom = top.Select(i => new Vector3(i.X, -0.055f, i.Z)).ToArray();
                var gasTubeConnector = MeshGenerator.Parallelepiped(top, bottom, new Vector3(offset, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, gasTubeConnector));
            }
            #endregion
            #region front sight
            {
                var offset = 0.52f;
                var length = 0.1f;
                var bottomLeft = new Vector3[]
                {
                    new Vector3(-length / 2, 0.08f, -halfWidth / 2 + 0.02f),
                    new Vector3(length / 2, 0.08f, -halfWidth / 2 + 0.02f),
                    new Vector3(length / 2, 0.08f, -halfWidth / 2 + 0.01f),
                    new Vector3(-length / 2, 0.08f, -halfWidth / 2 + 0.01f),
                };
                var topLeft = bottomLeft.Select(i => new Vector3(i.X, 0.1f, i.Z + MathF.CopySign(0.01f, i.Z))).ToArray();
                var bottomRight = bottomLeft.Select(i => new Vector3(i.X, i.Y, -i.Z)).Reverse().ToArray();
                var topRight = bottomRight.Select(i => new Vector3(i.X, 0.1f, i.Z + MathF.CopySign(0.01f, i.Z))).ToArray();
                var middleBottom = new Vector3[]
                {
                    new Vector3(-length / 2 + 0.03f, 0.08f, halfWidth / 2 - 0.02f),
                    new Vector3(length / 2 - 0.03f, 0.08f, halfWidth / 2 - 0.02f),
                    new Vector3(length / 2 - 0.03f, 0.08f, -halfWidth / 2 + 0.02f),
                    new Vector3(-length / 2 + 0.03f, 0.08f, -halfWidth / 2 + 0.02f),
                };
                var middleTop = middleBottom.Select(i => new Vector3(i.X, 0.1f, i.Z)).ToArray();
                var frontSightLeft = MeshGenerator.Parallelepiped(topLeft, bottomLeft, new Vector3(offset, 0, 0));
                var frontSightRight = MeshGenerator.Parallelepiped(topRight, bottomRight, new Vector3(offset, 0, 0));
                var frontSightMiddle = MeshGenerator.Parallelepiped(middleTop, middleBottom, new Vector3(offset, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, frontSightLeft));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, frontSightRight));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, frontSightMiddle));
            }
            #endregion
            #region muzzle break
            {
                var offset = 0.69f;
                var radius = 0.05f;
                var length = 0.25f;
                var left = new Vector3(offset, -0.05f, 0);
                var right = new Vector3(offset + length, -0.05f, 0);
                var muzzle = MeshGenerator.CylinderX(40, 0.035f, radius, left, left + new Vector3(0.03f, 0, 0));
                var muzzle1 = MeshGenerator.CylinderX(40, radius, radius, left + new Vector3(0.03f, 0, 0), right - new Vector3(0.04f, 0, 0));
                var muzzle2 = MeshGenerator.CylinderX(40, radius, 0.035f, right - new Vector3(0.04f, 0, 0), right);
                akParts.Add(new MeshRenderer(new Transform(), akMetal, muzzle));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, muzzle1));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, muzzle2));
            }
            #endregion
            Scenes.Add(new Scene(light, camera, akParts.ToArray()));
            //
            Scenes.Add(new Scene(light, camera, ground, cube));
            Scenes.Add(new Scene(light, camera, ground, pyramid));
            Scenes.Add(new Scene(light, camera, ground, trapezoid));
            Scenes.Add(new Scene(light, camera, ground, octahedron));
            Scenes.Add(new Scene(light, camera, ground, regularPyramid));
            Scenes.Add(new Scene(light, camera, ground, cone));
            Scenes.Add(new Scene(light, camera, ground, cylinder));
            Scenes.Add(new Scene(light2, camera, ground, sphere));
            Scenes.Add(new Scene(light, camera, ground, helix));
            Scenes.Add(new Scene(light, camera, ground, torus));
            Scenes.Add(new Scene(light, camera, ground, regularDodecahedron));
            Scenes.Add(new Scene(light, camera, ground, regularTetrahedron));
            Scenes.Add(new Scene(light, camera, ground, regularIcosahedron));
            Scenes[CurrentScene].Load();
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }

        public static float GetTotalTime()
        {
            return (float)Stopwatch.Elapsed.TotalSeconds;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Scenes[CurrentScene].Render(e.Time);
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var kInput = KeyboardState;
            var mInput = MouseState;
            TotalTime = (float)Stopwatch.Elapsed.TotalSeconds;
            if (kInput.IsKeyPressed(Keys.Escape))
                Close();
            if (kInput.IsKeyPressed(Keys.F))
            {
                if (CullFace)
                    GL.Disable(EnableCap.CullFace);
                else
                    GL.Enable(EnableCap.CullFace);
                CullFace = !CullFace;
            }
            if (kInput.IsKeyPressed(Keys.Space))
            {
                if (Wireframe)
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                else
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                Wireframe = !Wireframe;
            }
            if (kInput.IsKeyDown(Keys.LeftShift))
                ScaleCurrentScene(new Vector3(objectScaleSpeed) * (float)e.Time);
            if (kInput.IsKeyDown(Keys.LeftControl))
                ScaleCurrentScene(new Vector3(-objectScaleSpeed) * (float)e.Time);
            if (kInput.IsKeyDown(Keys.Q))
                TranslateCurrentScene(new Vector3(0, 0, objectMoveSpeed * (float)e.Time));
            if (kInput.IsKeyDown(Keys.E))
                TranslateCurrentScene(new Vector3(0, 0, -objectMoveSpeed * (float)e.Time));
            if (kInput.IsKeyDown(Keys.W))
                TranslateCurrentScene(new Vector3(0, objectMoveSpeed * (float)e.Time, 0));
            if (kInput.IsKeyDown(Keys.S))
                TranslateCurrentScene(new Vector3(0, -objectMoveSpeed * (float)e.Time, 0));
            if (kInput.IsKeyDown(Keys.A))
                TranslateCurrentScene(new Vector3(-objectMoveSpeed * (float)e.Time, 0, 0));
            if (kInput.IsKeyDown(Keys.D))
                TranslateCurrentScene(new Vector3(objectMoveSpeed * (float)e.Time, 0, 0));
            if (kInput.IsKeyPressed(Keys.Left))
            {
                Scenes[CurrentScene].UnLoad();
                CurrentScene--;
                if (CurrentScene < 0)
                    CurrentScene = Scenes.Count - 1;
                Scenes[CurrentScene].Load();
            }
            if (kInput.IsKeyPressed(Keys.Right))
            {
                Scenes[CurrentScene].UnLoad();
                CurrentScene++;
                if (CurrentScene >= Scenes.Count)
                    CurrentScene = 0;
                Scenes[CurrentScene].Load();
            }
            if (mInput.IsButtonDown(MouseButton.Left))
                RotateCurrentScene(new Vector3(mInput.Delta.Y * objectRotationSensitivity * (float)e.Time, mInput.Delta.X * objectRotationSensitivity * (float)e.Time, 0));
            else if (mInput.IsButtonDown(MouseButton.Right))
                RotateCurrentScene(new Vector3(0, mInput.Delta.X * objectRotationSensitivity * (float)e.Time, mInput.Delta.Y * objectRotationSensitivity * (float)e.Time));
            if (mInput.ScrollDelta.Y != 0)
                Zoom(-mInput.ScrollDelta.Y * zoomSpeed * (float)e.Time);
            Scenes[CurrentScene].Update(e.Time);
        }
        private void Zoom(float diff)
        {
            Scenes[CurrentScene].Camera.Fov += diff;
            Scenes[CurrentScene].Load();
        }
        private void RotateCurrentScene(Vector3 diff)
        {
            foreach (var r in Scenes[CurrentScene].renderers)
            {
                if (r is MeshRenderer)
                {
                    var mr = (MeshRenderer)r;
                    if (mr.IgnoreUserTransformations)
                        continue;
                    //Console.WriteLine(mr.Transform.Rotation);
                    mr.Transform.Rotation = mr.Transform.Rotation + diff;
                }
            }
        }

        private void ScaleCurrentScene(Vector3 diff)
        {
            foreach (var r in Scenes[CurrentScene].renderers)
            {
                if (r is MeshRenderer)
                {
                    var mr = (MeshRenderer)r;
                    if (mr.IgnoreUserTransformations)
                        continue;
                    //Console.WriteLine(mr.Transform.Rotation);
                    mr.Transform.Scale = mr.Transform.Scale + diff;
                }
            }
        }

        private void TranslateCurrentScene(Vector3 diff)
        {
            foreach (var r in Scenes[CurrentScene].renderers)
            {
                if (r is MeshRenderer)
                {
                    var mr = (MeshRenderer)r;
                    if (mr.IgnoreUserTransformations)
                        continue;
                    //Console.WriteLine(mr.Transform.Rotation);
                    mr.Transform.Position = mr.Transform.Position + diff;
                }
            }
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            foreach (var scene in Scenes)
            {
                scene.OnResize((float)Size.X / (float)Size.Y);
            }
        }
    }
}
