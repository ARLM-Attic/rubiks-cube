//-----------------------------------------------------------------------------
// File:        Rotation.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        02/21/2009
//-----------------------------------------------------------------------------
using System;
using System.Net;
using Knoics.Math;
using System.Diagnostics;
namespace Knoics.Interactive
{

    public class Rotation
    {
        public static void GetRotation(Vector3 p1, Vector3 p2, out Vector3 axis, out double theta)
        {
            Vector3 v1 = new Vector3(p1.X, p1.Y, p1.Z);
            Vector3 v2 = new Vector3(p2.X, p2.Y, p2.Z);

            axis = Vector3.Cross(v1, v2);
            theta = Vector3.AngleBetween(v1, v2);
        }

        public static Matrix GetRotationTransform(Vector3 v1, Vector3 v2, bool isForCamera)
        {
            Vector3 axis; double theta;
            GetRotation(v1, v2, out axis, out theta);
            if (isForCamera) theta = -theta;
            Quaternion delta = new Quaternion(axis, (float)theta);

            return Matrix.CreateFromQuaternion(delta);
        }

        public static double GetAngle(Vector3 v1, Vector3 v2, Axis axises, out Axis axis)
        {

            
            double yzAngle = 0; 
            double zxAngle = 0;
            double xyAngle = 0; 
            double angle = 0;
            axis = Axis.X;

            if ((axises & Axis.X) == Axis.X)
            {
                yzAngle = Vector2.AngleBetween(new Vector2(v1.Y, v1.Z), new Vector2(v2.Y, v2.Z));
                angle = yzAngle;
                axis = Axis.X;
            }
            if ((axises & Axis.Y) == Axis.Y)
            {
                zxAngle = Vector2.AngleBetween(new Vector2(v1.Z, v1.X), new Vector2(v2.Z, v2.X));
                if (System.Math.Abs(zxAngle) > System.Math.Abs(angle))
                {
                    axis = Axis.Y;
                    angle = zxAngle;
                }
            }
            if ((axises & Axis.Z) == Axis.Z)
            {
                xyAngle = Vector2.AngleBetween(new Vector2(v1.X, v1.Y), new Vector2(v2.X, v2.Y));
                if (System.Math.Abs(xyAngle) > System.Math.Abs(angle))
                {
                    axis = Axis.Z;
                    angle = xyAngle;
                }
            }
            //Debug.WriteLine(string.Format("YZ:{0}, ZX:{1}, XY:{2}", yzAngle, zxAngle, xyAngle));
            
            return angle;
        }
    }
}
