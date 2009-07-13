//-----------------------------------------------------------------------------
// File:        Camera.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        02/21/2009
//-----------------------------------------------------------------------------
using System;
using System.Net;
using Knoics.Math;


namespace Knoics.Interactive
{
    public class Camera
    {
        public static Matrix GetProjectionMatrix(double fov, double aspectRatio, double zNear, double zFar)
        {
            double xScale = 1 / System.Math.Tan(System.Math.PI * fov / 360);
            double yScale = aspectRatio * xScale;

            double zScale = zFar == Double.PositiveInfinity ? -1 : zFar / (zNear - zFar);
            double zOffset = zNear * zScale;

            Matrix projectionMatrix =
            new Matrix(xScale, 0, 0, 0,
                 0, yScale, 0, 0,
                 0, 0, zScale, -1,
                 0, 0, zOffset, 0);
            return projectionMatrix;
        }

        public static Matrix GetViewMatrix(Point3 cameraPosition, Vector3 lookDirection, Vector3 upDirection)
        {
            /*
            // Normalize vectors:
            lookDirection.Normalize();
            upDirection.Normalize();
            // Define vectors, XScale, YScale, and ZScale:
            double denom = System.Math.Sqrt(1 - System.Math.Pow(Vector3.Dot(lookDirection, upDirection), 2));
            Vector3 XScale = Vector3.Cross(lookDirection, upDirection) / denom;
            Vector3 x = upDirection - Vector3.Dot(upDirection, lookDirection) * lookDirection;
            Vector3 YScale = x / denom;
            Vector3 ZScale = lookDirection;
            // Construct M matrix:
            Matrix M = new Matrix();
            M.M11 = XScale.X;
            M.M21 = XScale.Y;
            M.M31 = XScale.Z;
            M.M12 = YScale.X;
            M.M22 = YScale.Y;
            M.M32 = YScale.Z;
            M.M13 = ZScale.X;
            M.M23 = ZScale.Y;
            M.M33 = ZScale.Z;
            // Translate the camera position to the origin:
            Matrix translateMatrix = new Matrix();
            translateMatrix.Translate(new Vector3(-cameraPosition.X,
            -cameraPosition.Y, -cameraPosition.Z));
            // Define reflect matrix about the Z axis:
            Matrix reflectMatrix = new Matrix();
            reflectMatrix.M33 = -1;
            // Construct the View matrix:
            Matrix viewMatrix =
            translateMatrix * M * reflectMatrix;
            return viewMatrix;
             */

            Vector3 zAxis = -lookDirection;
            zAxis.Normalize();
            Vector3 xAxis = Vector3.Cross(upDirection, zAxis);
            xAxis.Normalize();
            Vector3 yAxis = Vector3.Cross(zAxis, xAxis);


            Vector3 pos = new Vector3(cameraPosition.X, cameraPosition.Y, cameraPosition.Z);
            double offsetX = -Vector3.Dot(xAxis, pos);
            double offsetY = -Vector3.Dot(yAxis, pos);
            double offsetZ = -Vector3.Dot(zAxis, pos);


            Matrix viewMatrix =
            new Matrix(xAxis.X, yAxis.X, zAxis.X, 0,
                 xAxis.Y, yAxis.Y, zAxis.Y, 0,
                 xAxis.Z, yAxis.Z, zAxis.Z, 0,
                 offsetX, offsetY, offsetZ, 1);


            return viewMatrix;
        }
    }
}
