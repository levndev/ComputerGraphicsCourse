using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6
{
    internal class Transform
    {
        public Vector3 Position
        {
            get => _Position;
            set
            {
                _Position = value;
                Dirty = true;
            }
        }
        public Vector3 Rotation
        {
            get => _Rotation;
            set
            {
                _Rotation = value;
                Dirty = true;
            }
        }
        public Vector3 Scale
        {
            get => _Scale;
            set
            {
                _Scale = value;
                Dirty = true;
            }
        }
        public bool Dirty;
        private Vector3 _Position;
        private Vector3 _Rotation;
        private Vector3 _Scale;
        public Transform(Vector3? position = null, Vector3? rotation = null, Vector3? scale = null)
        {
            _Position = position ?? Vector3.Zero;
            _Rotation = rotation ?? Vector3.Zero;
            _Scale = scale ?? Vector3.One;
        }

        public Matrix4 GenerateModel()
        {
            var euler = new Vector3(MathHelper.DegreesToRadians(Rotation.X),
                                    MathHelper.DegreesToRadians(Rotation.Y),
                                    MathHelper.DegreesToRadians(Rotation.Z));
            var Model = Matrix4.Identity;
            Model = Model * Matrix4.CreateScale(Scale);
            Model = Model * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(euler));
            Model = Model * Matrix4.CreateTranslation(Position);
            return Model;
        }
    }
}
