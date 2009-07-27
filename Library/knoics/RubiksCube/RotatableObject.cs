using System;
using System.Net;
using Knoics.Math;
using Kit3D.Windows.Media.Media3D;

namespace Knoics.RubiksCube
{

    public class RotatableObject 
    {
        protected Matrix3D _transform = Matrix3D.Identity;
        private Matrix3D _savedTransform = Matrix3D.Identity;

        private Matrix3D _axisTransform = Matrix3D.Identity;
        private Vector3D _unitX = Ext3D.UnitX;
        private Vector3D _unitY = Ext3D.UnitY;
        private Vector3D _unitZ = Ext3D.UnitZ;


        public Vector3D UnitX { get { return _unitX; } }
        public Vector3D UnitY { get { return _unitY; } }
        public Vector3D UnitZ { get { return _unitZ; } }

        #region ITransform Members
        public virtual void Reset()
        {
            _transform = Matrix3D.Identity;
            _savedTransform = Matrix3D.Identity;
            _axisTransform = Matrix3D.Identity;
            _unitX = Ext3D.UnitX;
            _unitY = Ext3D.UnitY;
            _unitZ = Ext3D.UnitZ;
        }

        public virtual void Save()
        {
            _savedTransform = _transform;
        }

        public virtual void Restore()
        {
            _transform = _savedTransform;
        }

        public void RotateUnit(Axis axis, double rotation)
        {
            Matrix3D transform = Matrix3D.Identity;
            switch (axis)
            {
                case Axis.X:
                    transform = Ext3D.CreateRotationX(rotation);
                    break;
                case Axis.Y:
                    transform = Ext3D.CreateRotationY(rotation);
                    break;
                case Axis.Z:
                    transform = Ext3D.CreateRotationZ(rotation);
                    break;

            }
            _axisTransform = _axisTransform * transform;
            transform = _axisTransform;
            transform.Invert();
            _unitX = Ext3D.Transform(Ext3D.UnitX, transform);
            _unitY = Ext3D.Transform(Ext3D.UnitY, transform);
            _unitZ = Ext3D.Transform(Ext3D.UnitZ, transform);
        }

        public virtual Matrix3D Rotate(Axis axis, double deltaAngle, bool isFromSaved)
        {
            Quaternion3D rotationQ = Quaternion3D.Identity;

            switch (axis)
            {
                case Axis.X:
                    rotationQ *= Quaternion3D.CreateFromAxisAngle(_unitX, deltaAngle);
                    break;
                case Axis.Y:
                    rotationQ *= Quaternion3D.CreateFromAxisAngle(_unitY, deltaAngle);
                    break;
                case Axis.Z:
                    rotationQ *= Quaternion3D.CreateFromAxisAngle(_unitZ, deltaAngle);
                    break;

            }
            Matrix3D matrix = Ext3D.CreateFromQuaternion(rotationQ);
            if (isFromSaved)
                _transform = matrix * _savedTransform;
            else
                _transform = matrix * _transform;

            return matrix;
        }

        public Matrix3D Transform { get { return _transform; } }

        #endregion
    }

}
