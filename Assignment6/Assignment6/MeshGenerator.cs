using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6
{
    internal static class MeshGenerator
    {
        public static Mesh RegularIcosahedron()
        {
            var result = new List<float>();
            var phi = (1f + MathF.Sqrt(5)) / 2f;
            var r = 0.5f;
            var points = new List<Vector3>();
            foreach (var i in new int[] { -1, 1 })
            {
                foreach (var j in new int[] { -1, 1 })
                {
                    points.Add(new Vector3(0, i, phi * j) * r);
                    points.Add(new Vector3(i, phi * j, 0) * r);
                    points.Add(new Vector3(phi * j, 0, i) * r);
                }
            }
            var faces = new List<int[]>()
            {
                new int[]{ 0, 7, 1, },
                new int[]{ 3, 1, 7,},
                new int[]{ 5, 7, 0, },
                new int[]{ 0, 1, 2, },
                new int[]{ 5, 0, 6, },
                new int[]{ 0, 2, 6, },
                new int[]{ 8, 1, 3, },
                new int[]{ 3, 7, 11, },
                new int[]{ 8, 3, 9, },
                new int[]{ 9, 3, 11,},
                new int[]{ 2, 1, 8, },
                new int[]{ 11, 7, 5, },
                new int[]{ 6, 2, 4, },
                new int[]{ 4, 2, 8, },
                new int[]{ 4, 8, 9, },
                new int[]{ 9, 11, 10, },
                new int[]{ 10, 11, 5, },
                new int[]{ 10, 5, 6, }, 
                new int[]{ 4, 9, 10, }, 
                new int[]{ 10, 6, 4, }, 
            };
            foreach (var f in faces)
            {
                var p1 = points[f[0]];
                var p2 = points[f[1]];
                var p3 = points[f[2]];
                result.AddRange(MakeTriangle(p1, p2, p3));
            }
            return new Mesh(result.ToArray());
        }
        public static Mesh RegularTetrahedron()
        {
            var result = new List<float>();
            var r = 1;
            var bottom = -1f / 3f;
            var points = new List<Vector3>
            {
                new Vector3(0, 1, 0) * r,
                new Vector3(MathF.Sqrt(8f / 9f), bottom, 0) * r,
                new Vector3(-MathF.Sqrt(2f / 9f), bottom, MathF.Sqrt(2f / 3f)) * r,
                new Vector3(-MathF.Sqrt(2f / 9f), bottom, -MathF.Sqrt(2f / 3f)) * r,
            };
            result.AddRange(MakeTriangle(points[0], points[2], points[1]));
            result.AddRange(MakeTriangle(points[0], points[3], points[2]));
            result.AddRange(MakeTriangle(points[0], points[1], points[3]));
            result.AddRange(MakeTriangle(points[1], points[2], points[3]));
            return new Mesh(result.ToArray());
        }
        public static Mesh RegularDodecahedron()
        {
            //(±1, ±1, ±1)
            //(0, ±ϕ, ±1 / ϕ)
            //(±1 / ϕ, 0, ±ϕ)
            //(±ϕ, ±1 / ϕ, 0)
            //            0,2,7,1,4,
            //2,7,11,14,10,
            //1,7,11,18,8
            //10,2,0,3,13,
            //0,4,9,5,3,
            //4,1,8,6,9,
            //11,14,19,16,18,
            //10,13,15,19,14,
            //13,3,5,12,15,
            //8,18,16,17,6,
            //9,6,17,12,5,
            //19,15,12,17,16,
            var result = new List<float>();
            var phi = (1f + MathF.Sqrt(5)) / 2f;
            var r = 0.5f;
            var points = new List<Vector3>();
            foreach (var i in new int[] { -1, 1 })
            {
                foreach (var j in new int[] { -1, 1 })
                {
                    foreach (var k in new int[] { -1, 1 })
                    {
                        points.Add(new Vector3(i, j, k) * r); // orange
                    }
                    // коэфы вообще не так должны быть но уже пофиг
                    points.Add(new Vector3(0, phi * i, 1 / phi * j) * r); // green
                    points.Add(new Vector3(1 / phi * i, 0, phi * j) * r); // blue
                    points.Add(new Vector3(phi * i, 1 / phi * j, 0) * r); // red
                }
            }
            //var previous = points[0];
            //var current = GetClosestPoints(previous)[0];
            //var looping = true;
            //var pentagon = new List<Vector3>(points);
            //pentagon.Add(previous);
            //pentagon.Add(current);
            //while(looping)
            //{
            //    foreach (var next in GetClosestPoints(current))
            //    {
            //        if (next == previous)
            //            continue;
            //        if (GetWinding(previous, current, next))
            //        {
            //            pentagon.Add(next);
            //            if (next == points[0])
            //            {
            //                looping = false;
            //                break;
            //            }
            //            previous = current;
            //            current = next;
            //        }
            //    }
            //}
            var faces = new List<int[]>()
            {
                new int[]{ 0, 2, 7, 1, 4 },
                new int[]{ 2, 10, 14, 11, 7 },
                new int[]{ 1, 7, 11, 18, 8 },
                new int[]{ 10, 2, 0, 3, 13 },
                new int[]{ 0, 4, 9, 5, 3 },
                new int[]{ 4, 1, 8, 6, 9 },
                new int[]{ 11, 14, 19, 16, 18 },
                new int[]{ 10, 13, 15, 19, 14 },
                new int[]{ 13, 3, 5, 12, 15 },
                new int[]{ 8, 18, 16, 17, 6 },
                new int[]{ 9, 6, 17, 12, 5 },
                new int[]{ 19, 15, 12, 17, 16 },
            };
            foreach (var f in faces)
            {
                var p1 = points[f[0]];
                var p2 = points[f[1]];
                var p3 = points[f[2]];
                var p4 = points[f[3]];
                var p5 = points[f[4]];
                var center = (p1 + p2 + p3 + p4 + p5) / 5;
                result.AddRange(MakeTriangle(center, p1, p2));
                result.AddRange(MakeTriangle(center, p2, p3));
                result.AddRange(MakeTriangle(center, p3, p4));
                result.AddRange(MakeTriangle(center, p4, p5));
                result.AddRange(MakeTriangle(center, p5, p1));
            }
            return new Mesh(result.ToArray());
            //List<Vector3> GetClosestPoints(Vector3 v)
            //{
            //    var result = new List<Vector3>();
            //    foreach (var p in points)
            //    {
            //        var d = (p - v).Length;
            //        //0.618034
            //        if (d < 0.62f && d > 0.1f)
            //        {
            //            result.Add(p);
            //        }
            //    }
            //    return result;
            //}
        }
        public static Mesh Torus(int sides, int sections, float radius, float thickness)
        {
            var r = radius;
            var circle = new List<Vector3>();
            for (var i = 0; i < sides; i++)
            {
                circle.Add(GetCircleVertexYZ(Vector3.Zero, thickness, i, sides));
            }
            var result = new List<float>();
            for (var i = 0; i < sections; i++)
            {
                var t = 2 * MathF.PI * i / sections;
                var tNext = 2 * MathF.PI * (i + 1) / sections;
                var current = new Vector3(MathF.Sin(t) * r, 0, MathF.Cos(t) * r);
                var next = new Vector3(MathF.Sin(tNext) * r, 0, MathF.Cos(tNext) * r);
                Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(new Vector3(0, t, 0)), out var rotationMatrix);
                Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(new Vector3(0, tNext, 0)), out var rotationMatrix2);
                var cModel = Matrix4.Identity;
                cModel *= rotationMatrix;
                cModel *= Matrix4.CreateTranslation(current);
                var nModel = Matrix4.Identity;
                nModel *= rotationMatrix2;
                nModel *= Matrix4.CreateTranslation(next);
                for (var j = 0; j < circle.Count; j++)
                {
                    var c1 = circle[j];
                    var c2 = j < circle.Count - 1 ? circle[j + 1] : circle[0];
                    var p1 = new Vector4(c1.X, c1.Y, c1.Z, 1) * cModel;
                    var p2 = new Vector4(c2.X, c2.Y, c2.Z, 1) * cModel;
                    var p3 = new Vector4(c2.X, c2.Y, c2.Z, 1) * nModel;
                    var p4 = new Vector4(c1.X, c1.Y, c1.Z, 1) * nModel;
                    result.AddRange(p1.Xyz.ToArray());
                    result.AddRange((p1.Xyz - current).Normalized().ToArray());
                    result.AddRange(p2.Xyz.ToArray());
                    result.AddRange((p2.Xyz - current).Normalized().ToArray());
                    result.AddRange(p3.Xyz.ToArray());
                    result.AddRange((p3.Xyz - next).Normalized().ToArray());
                    result.AddRange(p1.Xyz.ToArray());
                    result.AddRange((p1.Xyz - current).Normalized().ToArray());
                    result.AddRange(p3.Xyz.ToArray());
                    result.AddRange((p3.Xyz - next).Normalized().ToArray());
                    result.AddRange(p4.Xyz.ToArray());
                    result.AddRange((p4.Xyz - next).Normalized().ToArray());
                }
            }
            return new Mesh(result.ToArray());
        }

        public static Mesh CustomTorusXY(int sections, Vector2 radius, List<Vector3> shape, Vector3 offset)
        {
            var r = radius;
            var result = new List<float>();
            for (var i = 0; i < sections; i++)
            {
                var t = 2 * MathF.PI * i / sections;
                var tNext = 2 * MathF.PI * (i + 1) / sections;
                var current = new Vector3(MathF.Cos(t) * r.X, MathF.Sin(t) * r.Y, 0) + offset;
                var next = new Vector3(MathF.Cos(tNext) * r.X, MathF.Sin(tNext) * r.Y, 0) + offset;
                Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(new Vector3(0, 0, t)), out var rotationMatrix);
                Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(new Vector3(0, 0, tNext)), out var rotationMatrix2);
                var cModel = Matrix4.Identity;
                cModel *= rotationMatrix;
                cModel *= Matrix4.CreateTranslation(current);
                var nModel = Matrix4.Identity;
                nModel *= rotationMatrix2;
                nModel *= Matrix4.CreateTranslation(next);
                for (var j = 0; j < shape.Count; j++)
                {
                    var c1 = shape[j];
                    var c2 = j < shape.Count - 1 ? shape[j + 1] : shape[0];
                    var p1 = new Vector4(c1.X, c1.Y, c1.Z, 1) * cModel;
                    var p2 = new Vector4(c2.X, c2.Y, c2.Z, 1) * cModel;
                    var p3 = new Vector4(c2.X, c2.Y, c2.Z, 1) * nModel;
                    var p4 = new Vector4(c1.X, c1.Y, c1.Z, 1) * nModel;
                    result.AddRange(p1.Xyz.ToArray());
                    result.AddRange((p1.Xyz - current).Normalized().ToArray());
                    result.AddRange(p2.Xyz.ToArray());
                    result.AddRange((p2.Xyz - current).Normalized().ToArray());
                    result.AddRange(p3.Xyz.ToArray());
                    result.AddRange((p3.Xyz - next).Normalized().ToArray());
                    result.AddRange(p1.Xyz.ToArray());
                    result.AddRange((p1.Xyz - current).Normalized().ToArray());
                    result.AddRange(p3.Xyz.ToArray());
                    result.AddRange((p3.Xyz - next).Normalized().ToArray());
                    result.AddRange(p4.Xyz.ToArray());
                    result.AddRange((p4.Xyz - next).Normalized().ToArray());
                }
            }
            return new Mesh(result.ToArray());
        }
        public static Mesh Helix(int sides, float step)
        {
            var bottom = -0.5f;
            var top = 0.5f;
            var h = 24;
            var r = 0.5f;
            var circle = new List<Vector3>();
            for (var i = 0; i < sides; i++)
            {
                circle.Add(GetCircleVertexYZ(Vector3.Zero, 0.1f, i, sides));
            }
            var result = new List<float>();
            for (var y = bottom; y <= top; y += step)
            {
                var t = y;
                var tNext = y + step;
                var current = new Vector3(MathF.Sin(t * h) * r, t, MathF.Cos(t * h) * r);
                var next = new Vector3(MathF.Sin(tNext * h) * r, tNext, MathF.Cos(tNext * h) * r);
                Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(new Vector3(0, t * h, 0)), out var rotationMatrix);
                Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(new Vector3(0, tNext * h, 0)), out var rotationMatrix2);
                var cModel = Matrix4.Identity;
                cModel *= rotationMatrix;
                cModel *= Matrix4.CreateTranslation(current);
                var nModel = Matrix4.Identity;
                nModel *= rotationMatrix2;
                nModel *= Matrix4.CreateTranslation(next);
                for (var i = 0; i < circle.Count; i++)
                {
                    var c1 = circle[i];
                    var c2 = i < circle.Count - 1 ? circle[i + 1] : circle[0];
                    var p1 = new Vector4(c1.X, c1.Y, c1.Z, 1) * cModel;
                    var p2 = new Vector4(c2.X, c2.Y, c2.Z, 1) * cModel;
                    var p3 = new Vector4(c2.X, c2.Y, c2.Z, 1) * nModel;
                    var p4 = new Vector4(c1.X, c1.Y, c1.Z, 1) * nModel;
                    result.AddRange(p1.Xyz.ToArray());
                    result.AddRange((p1.Xyz - current).Normalized().ToArray());
                    result.AddRange(p2.Xyz.ToArray());
                    result.AddRange((p2.Xyz - current).Normalized().ToArray());
                    result.AddRange(p3.Xyz.ToArray());
                    result.AddRange((p3.Xyz - next).Normalized().ToArray());
                    result.AddRange(p1.Xyz.ToArray());
                    result.AddRange((p1.Xyz - current).Normalized().ToArray());
                    result.AddRange(p3.Xyz.ToArray());
                    result.AddRange((p3.Xyz - next).Normalized().ToArray());
                    result.AddRange(p4.Xyz.ToArray());
                    result.AddRange((p4.Xyz - next).Normalized().ToArray());
                    if (y == bottom)
                    {
                        result.AddRange(MakeTriangle(current, p2.Xyz, p1.Xyz));
                    }
                    if (y + step >= top)
                    {
                        result.AddRange(MakeTriangle(next, p4.Xyz, p3.Xyz));
                    }
                }
            }
            return new Mesh(result.ToArray());
        }

        public static Mesh Sphere(float longResolution, int latResolution)
        {
            //https://stackoverflow.com/a/31326534
            var result = new List<float>();
            var r = 0.5f;
            var startU = 0;
            var startV = 0;
            var endU = MathF.PI * 2;
            var endV = MathF.PI;
            var stepU = (endU - startU) / longResolution; // step size between U-points on the grid
            var stepV = (endV - startV) / latResolution; // step size between V-points on the grid
            for (var i = 0; i < longResolution; i++)
            { // U-points
                for (var j = 0; j < latResolution; j++)
                { // V-points
                    var u = i * stepU + startU;
                    var v = j * stepV + startV;
                    var un = (i + 1 == longResolution) ? endU : (i + 1) * stepU + startU;
                    var vn = (j + 1 == latResolution) ? endV : (j + 1) * stepV + startV;
                    // Find the four points of the grid
                    // square by evaluating the parametric
                    // surface function
                    var p0 = F(u, v);
                    var p1 = F(u, vn);
                    var p2 = F(un, v);
                    var p3 = F(un, vn);
                    // NOTE: For spheres, the normal is just the normalized
                    // version of each vertex point; this generally won't be the case for
                    // other parametric surfaces.
                    result.AddRange(MakeTriangleForSphere(p0, p2, p1));
                    result.AddRange(MakeTriangleForSphere(p3, p1, p2));
                }
            }
            return new Mesh(result.ToArray());
            Vector3 F(float u, float v)
            {
                return new Vector3(MathF.Cos(u) * MathF.Sin(v) * r, MathF.Cos(v) * r, MathF.Sin(u) * MathF.Sin(v) * r);
            }
        }
        public static Mesh Cylinder(int sides, float topRadius, float bottomRadius)
        {
            var top = new Vector3(0, 0.5f, 0);
            var bottom = new Vector3(0, -0.5f, 0);
            var result = new List<float>();
            for (var i = 0; i < sides; i++)
            {
                var p1 = GetCircleVertexXZ(top, topRadius, i + 1, sides);
                var p2 = GetCircleVertexXZ(top, topRadius, i, sides);
                var p3 = GetCircleVertexXZ(bottom, bottomRadius, i, sides);
                var p4 = GetCircleVertexXZ(bottom, bottomRadius, i + 1, sides);
                result.AddRange(MakeRectangle(p4, p3, p2, p1));
                result.AddRange(MakeTriangle(top, p1, p2));
                result.AddRange(MakeTriangle(bottom, p3, p4));
            }
            return new Mesh(result.ToArray());
        }
        public static Mesh CylinderX(int sides, float leftRadius, float rightRadius, Vector3 right, Vector3 left)
        {
            var result = new List<float>();
            for (var i = 0; i < sides; i++)
            {
                var p1 = GetCircleVertexYZ(right, leftRadius, i + 1, sides);
                var p2 = GetCircleVertexYZ(right, leftRadius, i, sides);
                var p3 = GetCircleVertexYZ(left, rightRadius, i, sides);
                var p4 = GetCircleVertexYZ(left, rightRadius, i + 1, sides);
                result.AddRange(MakeRectangle(p4, p3, p2, p1));
                result.AddRange(MakeTriangle(right, p1, p2));
                result.AddRange(MakeTriangle(left, p3, p4));
            }
            return new Mesh(result.ToArray());
        }
        public static Mesh Cone(int sides)
        {
            var top = new Vector3(0, 0.5f, 0);
            var bottomCenter = new Vector3(0, -0.5f, 0);
            var result = new List<float>();
            for (var i = 0; i < sides; i++)
            {
                var v1 = GetCircleVertexXZ(bottomCenter, 0.5f, i, sides);
                var v2 = GetCircleVertexXZ(bottomCenter, 0.5f, i + 1, sides);
                result.AddRange(MakeTriangle(top, v2, v1));
                result.AddRange(MakeTriangle(bottomCenter, v1, v2));
            }
            return new Mesh(result.ToArray());
        }

        public static Mesh Pyramid()
        {
            var result = new List<float>();
            var top = new Vector3(0, 0.5f, 0);
            var bottom = new Vector3[] {
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
            };
            for (var i = 0; i < 4; i++)
            {
                var p1 = top;
                var p2 = bottom[i];
                var p3 = i > 0 ? bottom[i - 1] : bottom[3];
                result.AddRange(MakeTriangle(p1, p2, p3));
            }
            result.AddRange(MakeRectangle(bottom));
            return new Mesh(result.ToArray());
        }

        public static Mesh Octahedron()
        {
            var result = new List<float>();
            var top = new Vector3(0, 0.5f, 0);
            var middle = new Vector3[] {
                new Vector3(0.5f, 0, 0.5f),
                new Vector3(0.5f, 0, -0.5f),
                new Vector3(-0.5f, 0, -0.5f),
                new Vector3(-0.5f, 0, 0.5f),
            };
            var bottom = new Vector3(0, -0.5f, 0);
            for (var i = 0; i < 4; i++)
            {
                var p2 = middle[i];
                var p3 = i < 3 ? middle[i + 1] : middle[0];
                result.AddRange(MakeTriangle(top, p2, p3));
                result.AddRange(MakeTriangle(bottom, p3, p2));
            }
            return new Mesh(result.ToArray());
        }

        public static Mesh Trapezoid()
        {
            var top = new Vector3[] {
                new Vector3(0.25f, 0.5f, 0.25f),
                new Vector3(0.25f, 0.5f, -0.25f),
                new Vector3(-0.25f, 0.5f, -0.25f),
                new Vector3(-0.25f, 0.5f, 0.25f),
            };
            var bottom = top.Select(x => x * new Vector3(2, -1, 2)).ToArray();
            return new Mesh(Cuboid(top, bottom).ToArray());
        }

        public static Mesh Cube()
        {
            var top = new Vector3[] {
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
            };
            var bottom = top.Select(x => x * new Vector3(1, -1, 1)).ToArray();
            return new Mesh(Cuboid(top, bottom).ToArray());
        }

        public static Mesh Parallelepiped(Vector3[] top, Vector3[] bottom, Vector3 offset)
        {
            return new Mesh(Cuboid(top, bottom, offset).ToArray());
        }

        private static float[] Cuboid(Vector3[] top, Vector3[] bottom, Vector3? offset = null)
        {
            if (offset == null)
                offset = new Vector3(0, 0, 0);
            top = top.Select(i => i + offset.Value).ToArray();
            bottom = bottom.Select(i => i + offset.Value).ToArray();
            var result = new List<float>();
            for (var i = 0; i < 4; i++)
            {
                var p1 = top[i];
                var p2 = bottom[i];
                var p3 = i < 3 ? bottom[i + 1] : bottom[0];
                var p4 = i < 3 ? top[i + 1] : top[0];
                result.AddRange(MakeRectangle(p1, p2, p3, p4));
            }
            result.AddRange(MakeRectangle(top));
            result.AddRange(MakeRectangle(bottom.Reverse().ToArray()));
            return result.ToArray();
        }

        private static float[] MakeTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var result = new List<float>();
            var normal = Vector3.Cross(p2 - p1, p3 - p1);
            result.AddRange(p1.ToArray());
            result.AddRange(normal.ToArray());
            result.AddRange(p2.ToArray());
            result.AddRange(normal.ToArray());
            result.AddRange(p3.ToArray());
            result.AddRange(normal.ToArray());
            return result.ToArray();
        }

        private static float[] MakeTriangleForSphere(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var result = new List<float>();
            result.AddRange(p1.ToArray());
            result.AddRange(p1.Normalized().ToArray());
            result.AddRange(p2.ToArray());
            result.AddRange(p2.Normalized().ToArray());
            result.AddRange(p3.ToArray());
            result.AddRange(p3.Normalized().ToArray());
            return result.ToArray();
        }

        private static float[] MakeRectangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            var result = new List<float>();
            result.AddRange(MakeTriangle(p1, p2, p3));
            result.AddRange(MakeTriangle(p1, p3, p4));
            return result.ToArray();
        }

        private static float[] MakeRectangle(Vector3[] points)
        {
            return MakeRectangle(points[0], points[1], points[2], points[3]);
        }

        private static Vector3 GetCircleVertexYZ(Vector3 center, float radius, int index, int sides)
        {
            return new Vector3(center.X,
                                center.Y + radius * MathF.Cos(2 * MathF.PI * index / sides),
                                center.Z + radius * MathF.Sin(2 * MathF.PI * index / sides));
        }
        private static Vector3 GetCircleVertexXZ(Vector3 center, float radius, int index, int sides)
        {
            return new Vector3(center.X + radius * MathF.Cos(2 * MathF.PI * index / sides),
                                center.Y,
                                center.Z + radius * MathF.Sin(2 * MathF.PI * index / sides));
        }

        private static bool GetWinding(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var normal = Vector3.Cross(p2 - p1, p3 - p1);
            var center = p1 + p2 + p3 / 3f;
            center.Normalize();
            var dot = Vector3.Dot(normal, center);
            if (dot > 0)
                return true;
            else
            {
                Console.WriteLine(normal);
                Console.WriteLine(center);
                Console.WriteLine(dot);
                return false;
            }
        }
    }
}
