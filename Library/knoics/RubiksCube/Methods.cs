using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Kit3D.Windows.Media.Media3D;
using Knoics.Math;

namespace Knoics.RubiksCube
{


    public static class Methods
    {
        public class RotateCameraResult
        {
            public Matrix3D RotateMatrix { get; set; }
            public Matrix3D ViewMatrix { get; set; }
        }
        
        private static Matrix3D CalculateViewMatrix(Point3D position, Vector3D lookDirection, Vector3D upDirection)
        {
            Vector3D cameraZAxis = -lookDirection;
            cameraZAxis.Normalize();
            Vector3D cameraXAxis = Vector3D.CrossProduct(upDirection, cameraZAxis);
            cameraXAxis.Normalize();
            Vector3D cameraYAxis = Vector3D.CrossProduct(cameraZAxis, cameraXAxis);
            Vector3D cameraPosition = new Vector3D(position.X, position.Y, position.Z);
            double offsetX = -Vector3D.DotProduct(cameraXAxis, cameraPosition);
            double offsetY = -Vector3D.DotProduct(cameraYAxis, cameraPosition);
            double offsetZ = -Vector3D.DotProduct(cameraZAxis, cameraPosition);
            return new Matrix3D(cameraXAxis.X, cameraYAxis.X, cameraZAxis.X, 0.0,
                                        cameraXAxis.Y, cameraYAxis.Y, cameraZAxis.Y, 0.0,
                                        cameraXAxis.Z, cameraYAxis.Z, cameraZAxis.Z, 0.0,
                                        offsetX, offsetY, offsetZ, 1.0);
        }

        public static RotateCameraResult RotateCamera(this PerspectiveCamera camera, Vector3D axis, double angle) 
        {
            axis.Normalize();
            Quaternion3D q = Quaternion3D.CreateFromAxisAngle(axis, angle);
            Matrix3D m  = Ext3D.CreateFromQuaternion(q);
            
            Vector3D p = new Vector3D(camera.Position.X, camera.Position.Y, camera.Position.Z);
            Vector3D pos = Ext3D.Transform(p, m);
            camera.Position = new Point3D(pos.X, pos.Y, pos.Z);
            
            Vector3D d = new Vector3D(camera.LookDirection.X, camera.LookDirection.Y, camera.LookDirection.Z);
            camera.LookDirection = Ext3D.Transform(d, m) ;
            Vector3D up = new Vector3D(camera.UpDirection.X, camera.UpDirection.Y, camera.UpDirection.Z);
            camera.UpDirection = Ext3D.Transform(up, m);
            return new RotateCameraResult
            {
                RotateMatrix = m,
                ViewMatrix = CalculateViewMatrix(camera.Position, camera.LookDirection, camera.UpDirection)
            };
        }        

        
        public static Vector3D ProjectToTrackball (this Point point,  double width, double height)
        {
            double  x = point.X / (width / 2.0) - 1.0;    
            double y = 1.0 - point.Y / (height / 2.0);
            double z2 = 1.0 - x * x - y * y;       // z^2 = 1 - x^2 - y^2
            double  z =  z2 > 0.0 ? System.Math.Sqrt(z2) : 0.0;
            return new Vector3D(x, y, z);
        }
    }
}
