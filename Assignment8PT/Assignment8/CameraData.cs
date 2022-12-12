using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment8
{
    internal struct CameraData
    {
        public Vector3 Position;
        public Vector2 ViewportSize;
        public float FOV;
        public Vector3 Direction;
        public Vector3 Up;
        public Matrix3 ViewToWorld;
        public void SetRotation(Vector2 rotation)
        {
            rotation.X = MathHelper.DegreesToRadians(rotation.X);
            rotation.Y = MathHelper.DegreesToRadians(rotation.Y);
            Direction.X = MathF.Cos(rotation.X) * MathF.Cos(rotation.Y);
            Direction.Y = MathF.Sin(rotation.X);
            Direction.Z = MathF.Cos(rotation.X) * MathF.Sin(rotation.Y);
            Direction.Normalize();
            var right = Vector3.Cross(Direction, Vector3.UnitY).Normalized();
            Up = Vector3.Cross(right, Direction).Normalized();
            ViewToWorld = new Matrix3(right, Up, Direction);
        }
        public Vector3 Right()
        {
            return Vector3.Cross(Direction, Vector3.UnitY).Normalized();
        }
    }
}
