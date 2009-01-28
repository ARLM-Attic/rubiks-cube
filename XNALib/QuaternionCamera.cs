#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion


namespace XNALib
{
    public class QuaternionCamera : ICamera
    {
        private Vector3 _pos;
        private Vector3 _target;
        private Vector3 _up;
        private Vector3 _right;

        private Matrix _view;
        private Matrix _projection;
        public Matrix Projection
        {
            get
            {
                return _projection;
            }
        }
        public Matrix View
        {
            get
            {
                return _view;
            }
        }

        public void Update() { }

        public QuaternionCamera(Vector3 pos, Vector3 target, Vector3 up, float ar)
        {
            _pos = pos; _target = target;
            _view = Matrix.CreateLookAt(pos, target, up);
            _projection = Matrix.CreatePerspectiveFieldOfView(
                            MathHelper.PiOver4,
                            ar,
                            1.0f,
                            1000.0f);

            // Even if the up vector is off, we can get
            // a reliable right vector
            //_right = Vector3.Cross(_target - _pos, _up);
            //_right.Normalize();
            // Given a correct right vector, can recalculate up
            //this._up = Vector3.Cross(_right, _target - _pos);
            //this._up.Normalize();   

            // Shortcut to get the _up and _right vectors
            _right.X = _view.M11; _right.Y = _view.M21;
            _right.Z = _view.M31; _up.X = _view.M12;
            _up.Y = _view.M22; _up.Z = _view.M32;
        }
        public QuaternionCamera(Vector3 pos, Vector3 target, float ar)
            : this(pos, target, Vector3.Up, ar)
        {
        }
        public void Translate(float forward, float right, float up)
        {
            // Move the camera position, and calculate a 
            // new target

            //Vector3 direction = _target - _pos;
            //direction.Normalize();
            // Shortcut to pull the above vector from the view matrix
            Vector3 direction = new Vector3(-_view.M13, -_view.M23, -_view.M33);

            _pos += direction * forward;
            _pos += _right * right;
            _pos += _up * up;
            _target = _pos + direction;

            // Calculate the new view matrix
            _view = Matrix.CreateLookAt(_pos, _target, _up);
        }


        public void Rotate(float pan, float tilt, float roll)
        {
            _view = GetViewMatrix(ref _pos, ref _target, ref _up, pan, tilt, roll);

            _right = Vector3.Cross(_target - _pos, _up);
            _right.Normalize();
            _up = Vector3.Cross(_right, _target - _pos);
            _up.Normalize();
        }


        public static Matrix GetViewMatrix(ref Vector3 position, ref Vector3 target,
            ref Vector3 up, float yaw, float pitch, float roll)
        {
            // The right vector can be inferred
            Vector3 forward = target - position;
            Vector3 right = Vector3.Cross(forward, up);

            // This quaternion is the total of all the 
            // specified rotations
            Quaternion yawpitch = CreateFromYawPitchRoll(up, yaw,
                right, pitch, forward, roll);

            // Calculate the new target position, and the 
            // new up vector by transforming the quaternion
            //target = position + Vector3.Transform(forward, yawpitch);
            up = Vector3.Transform(up, yawpitch);
            position = Vector3.Transform(position, yawpitch);

            return Matrix.CreateLookAt(position, target, up);
        }
        public static Quaternion CreateFromYawPitchRoll(Vector3 up, float yaw, Vector3 right, float pitch, Vector3 forward, float roll)
        {
            // Create a quaternion for each rotation, and multiply them 
            // together.  We normalize them to avoid using the conjugate
            Quaternion qyaw = Quaternion.CreateFromAxisAngle(up, (float)yaw);
            qyaw.Normalize();
            Quaternion qtilt = Quaternion.CreateFromAxisAngle(right, (float)pitch);
            qtilt.Normalize();
            Quaternion qroll = Quaternion.CreateFromAxisAngle(forward, (float)roll);
            qroll.Normalize();
            Quaternion yawpitch = qyaw * qtilt * qroll;
            yawpitch.Normalize();

            return yawpitch;
        }
    }
}
