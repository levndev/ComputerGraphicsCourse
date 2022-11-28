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
        private float objectRotationSpeed = 60;
        private float objectRotationSensitivity = 30;
        private float zoomSpeed = 60;
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            VSync = VSyncMode.On;
            GL.ClearColor(Color4.DarkGray);
            GL.Enable(EnableCap.DepthTest);
            DefaultShader = new Shader("Shaders/DefaultShader.vs", "Shaders/DefaultShader.fs");
            DefaultShader.Use();
            camera = new Camera(new Vector3(0, 1, 1.5f), 1);
            camera.Pitch = -40;
            //texture
            var tex1 = Texture.LoadFromFile("Textures/wall.jpg");
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
            ground.IgnoreUserRotation = true;
            var cube = new MeshRenderer(new Transform(), groundMaterial, cubeMesh);
            var pyramid = new MeshRenderer(new Transform(), material, pyramidMesh);
            var trapezoid = new MeshRenderer(new Transform(), material, trapezoidMesh);
            var octahedron = new MeshRenderer(new Transform(), material, octahedronMesh);
            var regularPyramid = new MeshRenderer(new Transform(), material, regularPyramidMesh);
            var cone = new MeshRenderer(new Transform(), material, coneMesh);
            var cylinder = new MeshRenderer(new Transform(), material, cylinderMesh);
            var sphere = new MeshRenderer(new Transform(), material, sphereMesh);
            sphere.IgnoreUserRotation = true;
            var helix = new MeshRenderer(new Transform(), material, helixMesh);
            var torus = new MeshRenderer(new Transform(), material, torusMesh);
            var regularDodecahedron = new MeshRenderer(new Transform(), material, regularDodecahedronMesh);
            var regularTetrahedron = new MeshRenderer(new Transform(), material, regularTetrahedronMesh);
            var regularIcosahedron = new MeshRenderer(new Transform(), material, regularIcosahedronMesh);
            //cube.Func = new Action<Transform>((t) =>
            //{
            //    t.Position = new Vector3(2 * MathF.Cos(TotalTime - MathF.PI / 2f), t.Position.Y, 2 * MathF.Sin(TotalTime - MathF.PI / 2f));
            //});
            //pyramid.Func = new Action<Transform>((t) =>
            //{
            //    t.Position = new Vector3(2 * MathF.Cos(TotalTime), t.Position.Y, 2 * MathF.Sin(TotalTime));
            //});
            //trapezoid.Func = new Action<Transform>((t) =>
            //{
            //    t.Position = new Vector3(2 * MathF.Cos(TotalTime + MathF.PI), t.Position.Y, 2 * MathF.Sin(TotalTime + MathF.PI));
            //});
            //octahedron.Func = new Action<Transform>((t) =>
            //{
            //    //t.Position = new Vector3(2 * MathF.Cos(TotalTime + MathF.PI / 2f), t.Position.Y, 2 * MathF.Sin(TotalTime + MathF.PI / 2f));
            //});
            //regularPyramid.Func = new Action<Transform>((t) =>
            //{
            //    t.Position = new Vector3(2 * MathF.Cos(TotalTime + MathF.PI / 2f), t.Position.Y, 2 * MathF.Sin(TotalTime + MathF.PI / 2f));
            //});
            sphere.Func = new Action<Transform>((t) =>
            {
                t.Position = new Vector3(MathF.Cos(TotalTime + MathF.PI / 2f), t.Position.Y, MathF.Sin(TotalTime + MathF.PI / 2f));
                //t.Rotation = new Vector3(0, TotalTime * 4, t.Rotation.Z);
            });
            //ak-12
            var akMetal = new Material
            {
                Ambient = new Vector3(0.3f, 0.3f, 0.3f),
                Diffuse = new Vector3(0.3f, 0.3f, 0.3f),
                Specular = new Vector3(0.5f, 0.5f, 0.5f),
                Shininess = 16,
            };
            var akPolymer = new Material
            {
                Ambient = new Vector3(0.2f, 0.2f, 0.2f),
                Diffuse = new Vector3(0.2f, 0.2f, 0.2f),
                Specular = new Vector3(0.2f, 0.2f, 0.2f),
                Shininess = 256.0f,
            };
            var akParts = new List<MeshRenderer>();
            akParts.Add(ground);
            var halfWidth = 0.05f;
            //stock
            {
                var length = 0.4f;
                var offset = -0.55f;
                var stockTop = new Vector3[]
                {
                    new Vector3(-length, 0.01f, halfWidth),
                    new Vector3(0, 0, halfWidth),
                    new Vector3(0, 0, -halfWidth),
                    new Vector3(-length, 0.01f, -halfWidth),
                };
                var stockBottom = new Vector3[]
                {
                    new Vector3(-length, -0.12f, halfWidth),
                    new Vector3(0, -0.12f, halfWidth),
                    new Vector3(0, -0.12f, -halfWidth),
                    new Vector3(-length, -0.12f, -halfWidth),
                };
                var stock = MeshGenerator.Parallelepiped(stockTop, stockBottom, new Vector3(offset, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akPolymer, stock));
            }
            // butt pad
            {
                var length = 0.1f;
                var offset = -0.85f;
                var buttPadTop = new Vector3[]
                {
                    new Vector3(-length, -0.12f, halfWidth),
                    new Vector3(0, -0.12f, halfWidth),
                    new Vector3(0, -0.12f, -halfWidth),
                    new Vector3(-length, -0.12f, -halfWidth),
                };
                var buttPadBottom = new Vector3[]
                {
                    new Vector3(-length, -0.3f, halfWidth),
                    new Vector3(-length / 2, -0.3f, halfWidth),
                    new Vector3(-length / 2, -0.3f, -halfWidth),
                    new Vector3(-length, -0.3f, -halfWidth),
                };
                var buttPad = MeshGenerator.Parallelepiped(buttPadTop, buttPadBottom, new Vector3(offset, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akPolymer, buttPad));
            }
            //grip
            {
                var length = 0.1f;
                var skew = -0.05f;
                var offset = -0.45f;
                var gripTop = new Vector3[]
                {
                    new Vector3(-length, -0.12f, halfWidth),
                    new Vector3(length / 2, -0.1f, halfWidth - 0.01f),
                    new Vector3(length / 2, -0.1f, -halfWidth - 0.01f),
                    new Vector3(-length, -0.12f, -halfWidth),
                };
                var gripMiddle = new Vector3[]
                {
                    new Vector3(-length / 2, -0.16f, halfWidth),
                    new Vector3(length / 2, -0.2f, halfWidth),
                    new Vector3(length / 2, -0.2f, -halfWidth),
                    new Vector3(-length / 2, -0.16f, -halfWidth),
                };
                var gripBottom = new Vector3[]
                {
                    new Vector3(-length + skew, -0.32f, halfWidth),
                    new Vector3(length / 2 + skew, -0.35f, halfWidth),
                    new Vector3(length / 2 + skew, -0.35f, -halfWidth),
                    new Vector3(-length + skew, -0.32f, -halfWidth),
                };
                var grip = MeshGenerator.Parallelepiped(gripTop, gripMiddle, new Vector3(offset, 0, 0));
                var grip1 = MeshGenerator.Parallelepiped(gripMiddle, gripBottom, new Vector3(offset, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akPolymer, grip));
                akParts.Add(new MeshRenderer(new Transform(), akPolymer, grip1));
            }
            //trigger guard
            {
                var length = 0.1f;
                var offset = -0.35f;
                var left = new Vector3[]
                {
                    new Vector3(-length / 2, -0.2f, halfWidth),
                    new Vector3(-length / 2, -0.18f, halfWidth),
                    new Vector3(-length / 2, -0.18f, -halfWidth),
                    new Vector3(-length / 2, -0.2f, -halfWidth),
                };
                var middle = new Vector3[]
                {
                    new Vector3(0, -0.22f, halfWidth),
                    new Vector3(0, -0.2f, halfWidth),
                    new Vector3(0, -0.2f, -halfWidth),
                    new Vector3(0, -0.22f, -halfWidth),
                };
                var right = new Vector3[]
                {
                    new Vector3(length / 2, -0.21f, halfWidth),
                    new Vector3(length / 2, -0.19f, halfWidth),
                    new Vector3(length / 2, -0.19f, -halfWidth),
                    new Vector3(length / 2, -0.21f, -halfWidth),
                };
                var top = new Vector3[]
                {
                    new Vector3(length / 2, -0.1f, halfWidth),
                    new Vector3(length, -0.1f, halfWidth),
                    new Vector3(length, -0.1f, -halfWidth),
                    new Vector3(length / 2, -0.1f, -halfWidth),
                };
                var bottom = new Vector3[]
                {
                    new Vector3(length / 2, -0.21f, halfWidth),
                    new Vector3(length, -0.19f, halfWidth),
                    new Vector3(length, -0.19f, -halfWidth),
                    new Vector3(length / 2, -0.21f, -halfWidth),
                };
                var triggerGuard = MeshGenerator.Parallelepiped(left, middle, new Vector3(offset, 0, 0));
                var triggerGuard1 = MeshGenerator.Parallelepiped(middle, right, new Vector3(offset, 0, 0));
                var triggerGuard2 = MeshGenerator.Parallelepiped(top, bottom, new Vector3(offset, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, triggerGuard));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, triggerGuard1));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, triggerGuard2));
            }
            //trigger
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
            //magazine
            {
                var length = 0.2f;
                var offset = -0.05f;
                var skew = 0.05f;
                var magazineTop = new Vector3[]
                {
                    new Vector3(-length, -0.1f, halfWidth - 0.01f),
                    new Vector3(0, -0.1f, halfWidth - 0.01f),
                    new Vector3(0, -0.1f, -halfWidth + 0.01f),
                    new Vector3(-length, -0.1f, -halfWidth + 0.01f),
                };
                var magazineBottom = new Vector3[]
                {
                    new Vector3(-length + skew, -0.45f, halfWidth - 0.01f),
                    new Vector3(skew, -0.4f, halfWidth - 0.01f),
                    new Vector3(skew, -0.4f, -halfWidth + 0.01f),
                    new Vector3(-length + skew, -0.45f, -halfWidth + 0.01f),
                };
                var magazine = MeshGenerator.Parallelepiped(magazineTop, magazineBottom, new Vector3(offset, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akPolymer, magazine));
            }
            //lower receiver
            {
                var mat = new Material
                {
                    Ambient = new Vector3(0.3f, 0.3f, 0.3f),
                    Diffuse = new Vector3(0.3f, 0.3f, 0.3f),
                    Specular = new Vector3(0.5f, 0.5f, 0.5f),
                    Shininess = 16,
                    DiffuseMap = Texture.LoadFromFile("Textures/ak_tex.png"),
                };
                var texCoords = new List<Vector2[]>();
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.23f, 0.15f),
                    Vector2.Zero,
                    new Vector2(0.8f, 0),
                    new Vector2(0.8f, 0.4f),
                });
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0, 0.4f),
                    Vector2.Zero,
                    new Vector2(0.8f, 0),
                    new Vector2(0.8f, 0.4f),
                });
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0.8f, 0.4f),
                    new Vector2(0.8f, 0),
                    Vector2.Zero,
                    new Vector2(0, 0.4f),
                });
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0, 0.4f),
                    Vector2.Zero,
                    new Vector2(0.8f, 0),
                    new Vector2(0.8f, 0.4f),
                });
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0, 0.4f),
                    Vector2.Zero,
                    new Vector2(0.8f, 0),
                    new Vector2(0.8f, 0.4f),
                });
                texCoords.Add(new Vector2[]
                {
                    new Vector2(0, 0.4f),
                    Vector2.Zero,
                    new Vector2(0.8f, 0),
                    new Vector2(0.8f, 0.4f),
                });
                var length = 0.55f;
                var bottom = -0.1f;
                var lowerReceiverTop = new Vector3[]
                {
                    new Vector3(-length, 0, halfWidth),
                    new Vector3(0, 0, halfWidth),
                    new Vector3(0, 0, -halfWidth),
                    new Vector3(-length, 0, -halfWidth),
                };
                var lowerReceiverBottom = new Vector3[]
                {
                    new Vector3(-length, bottom - 0.02f, halfWidth),
                    new Vector3(0, bottom, halfWidth),
                    new Vector3(0, bottom, -halfWidth),
                    new Vector3(-length, bottom - 0.02f, -halfWidth),
                };
                var lowerReceiver = MeshGenerator.Parallelepiped(lowerReceiverTop, lowerReceiverBottom, new Vector3(0, 0, 0), texCoords.ToArray());
                akParts.Add(new MeshRenderer(new Transform(), mat, lowerReceiver));
            }
            //dust cover
            {
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
                var dustCover = MeshGenerator.Parallelepiped(dustCoverTop, dustCoverBottom, new Vector3(0, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, dustCover));
            }
            //dust cover rail
            {
                var length = 0.5f;
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
                var railBase = MeshGenerator.Parallelepiped(baseTop, baseBottom, new Vector3(0, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, railBase));
                var segmentTop = new Vector3[]
                {
                    new Vector3(-step / 2, 0.01f, halfWidth / 3),
                    new Vector3(step / 2, 0.01f, halfWidth / 3),
                    new Vector3(step / 2, 0.01f, -halfWidth / 3),
                    new Vector3(-step / 2, 0.01f, -halfWidth / 3),
                };
                var segmentBottom = segmentTop.Select(i => new Vector3(i.X, 0, MathF.Sign(i.Z) * halfWidth / 1.5f)).ToArray();
                for (var x = -0.5f + step / 2; x < 0.34f; x += step * 2)
                {
                    var segment = MeshGenerator.Parallelepiped(segmentTop, segmentBottom, new Vector3(x, 0.09f, 0));
                    akParts.Add(new MeshRenderer(new Transform(), akMetal, segment));
                }
            }
            //handguard
            {
                var length = 0.34f;
                var handGuardTop = new Vector3[]
                {
                    new Vector3(0, 0.08f, halfWidth / 2f),
                    new Vector3(length, 0.08f, halfWidth / 2f),
                    new Vector3(length, 0.08f, -halfWidth / 2f),
                    new Vector3(0, 0.08f, -halfWidth / 2f),
                };
                var handGuardSection1 = new Vector3[]
                {
                    new Vector3(0, 0.07f, halfWidth / 1.2f),
                    new Vector3(length, 0.07f, halfWidth / 1.2f),
                    new Vector3(length, 0.07f, -halfWidth / 1.2f),
                    new Vector3(0, 0.07f, -halfWidth / 1.2f),
                };
                var handGuardSection2 = new Vector3[]
                {
                    new Vector3(0, -0.07f, halfWidth / 1.2f),
                    new Vector3(length, -0.07f, halfWidth / 1.2f),
                    new Vector3(length, -0.07f, -halfWidth / 1.2f),
                    new Vector3(0, -0.07f, -halfWidth / 1.2f),
                };
                var handGuardBottom = new Vector3[]
                {
                    new Vector3(0, -0.1f, halfWidth / 2f),
                    new Vector3(length, -0.1f, halfWidth / 2f),
                    new Vector3(length, -0.1f, -halfWidth / 2f),
                    new Vector3(0, -0.1f, -halfWidth / 2f),
                };
                var handGuard1 = MeshGenerator.Parallelepiped(handGuardTop, handGuardSection1, new Vector3(0, 0, 0));
                var handGuard2 = MeshGenerator.Parallelepiped(handGuardSection1, handGuardSection2, new Vector3(0, 0, 0));
                var handGuard3 = MeshGenerator.Parallelepiped(handGuardSection2, handGuardBottom, new Vector3(0, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akPolymer, handGuard1));
                akParts.Add(new MeshRenderer(new Transform(), akPolymer, handGuard2));
                akParts.Add(new MeshRenderer(new Transform(), akPolymer, handGuard3));
            }
            //barrel
            {
                var offset = 0.34f;
                var radius = 0.035f;
                var length = 0.55f;
                var left = new Vector3(offset, -0.05f, 0);
                var right = new Vector3(offset + length, -0.05f, 0);
                var barrel = MeshGenerator.CylinderX(40, radius, radius, left, right);
                akParts.Add(new MeshRenderer(new Transform(), akMetal, barrel));
            }
            //gas tube
            {
                var offset = 0.34f;
                var radius = 0.03f;
                var length = 0.25f;
                var left = new Vector3(offset, 0.055f, 0);
                var right = new Vector3(offset + length, 0.055f, 0);
                var gasTube = MeshGenerator.CylinderX(40, radius, radius, left, right);
                akParts.Add(new MeshRenderer(new Transform(), akMetal, gasTube));
            }
            //gas tube connector
            {
                var offset = 0.52f;
                var length = 0.1f;
                var top = new Vector3[]
                {
                    new Vector3(-length / 2, 0.12f, halfWidth / 2),
                    new Vector3(length / 2, 0.12f, halfWidth / 2),
                    new Vector3(length / 2, 0.12f, -halfWidth / 2),
                    new Vector3(-length / 2, 0.12f, -halfWidth / 2),
                };
                var bottom = top.Select(i => new Vector3(i.X, -0.055f, i.Z)).ToArray();
                var gasTubeConnector = MeshGenerator.Parallelepiped(top, bottom, new Vector3(offset, 0, 0));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, gasTubeConnector));
            }
            //muzzle break
            {
                var offset = 0.79f;
                var radius = 0.05f;
                var length = 0.15f;
                var left = new Vector3(offset, -0.05f, 0);
                var right = new Vector3(offset + length, -0.05f, 0);
                var muzzle = MeshGenerator.CylinderX(40, 0.035f, radius, left, left + new Vector3(0.03f, 0, 0));
                var muzzle1 = MeshGenerator.CylinderX(40, radius, radius, left + new Vector3(0.03f, 0, 0), right - new Vector3(0.04f, 0, 0));
                var muzzle2 = MeshGenerator.CylinderX(40, radius, 0.035f, right - new Vector3(0.04f, 0, 0), right);
                akParts.Add(new MeshRenderer(new Transform(), akMetal, muzzle));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, muzzle1));
                akParts.Add(new MeshRenderer(new Transform(), akMetal, muzzle2));
            }
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
            if (kInput.IsKeyDown(Keys.Q))
                RotateCurrentScene(new Vector3(0, 0, objectRotationSpeed * (float)e.Time));
            if (kInput.IsKeyDown(Keys.E))
                RotateCurrentScene(new Vector3(0, 0, -objectRotationSpeed * (float)e.Time));
            if (kInput.IsKeyDown(Keys.W))
                RotateCurrentScene(new Vector3(-objectRotationSpeed * (float)e.Time, 0, 0));
            if (kInput.IsKeyDown(Keys.S))
                RotateCurrentScene(new Vector3(objectRotationSpeed * (float)e.Time, 0, 0));
            if (kInput.IsKeyDown(Keys.A))
                RotateCurrentScene(new Vector3(0, -objectRotationSpeed * (float)e.Time, 0));
            if (kInput.IsKeyDown(Keys.D))
                RotateCurrentScene(new Vector3(0, objectRotationSpeed * (float)e.Time, 0));
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
                    if (mr.IgnoreUserRotation)
                        continue;
                    //Console.WriteLine(mr.Transform.Rotation);
                    mr.Transform.Rotation = mr.Transform.Rotation + diff;
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
