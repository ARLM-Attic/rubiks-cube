
#region Description
//-----------------------------------------------------------------------------
// File:        Quaternion.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Reference:   Microsoft.Xna.Framework and System.Windows.Media.Media3D
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Net;

namespace Knoics.Math
{
    internal struct Quaternion
    {
        public double X;
        public double Y;
        public double Z;
        public double W;
        private static Quaternion _identity;
        public static Quaternion Identity { get { return _identity; } }

        static Quaternion()
        {
            _identity = new Quaternion(0f, 0f, 0f, 1f);
        }
        public Quaternion(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public Quaternion(Vector3 axisOfRotation, float angleInDegrees)
        {
            angleInDegrees = angleInDegrees % 360.0F;
            double num2 = angleInDegrees * 0.017453292519943295F;
            double length = axisOfRotation.Length;
            if (length == 0.0)
            {
                throw new InvalidOperationException("error vector");
            }
            Vector3 vectord = (Vector3)((axisOfRotation / length) * System.Math.Sin(0.5 * num2));
            this.X = vectord.X;
            this.Y = vectord.Y;
            this.Z = vectord.Z;
            this.W = System.Math.Cos(0.5 * num2);
            //this._isNotDistinguishedIdentity = true;
        }


        public static Quaternion CreateFromAxisAngle(Vector3 axis, double angle)
        {
            Quaternion quaternion;
            double num2 = angle * 0.5;
            double num = System.Math.Sin(num2);
            double num3 = System.Math.Cos(num2);
            quaternion.X = axis.X * num;
            quaternion.Y = axis.Y * num;
            quaternion.Z = axis.Z * num;
            quaternion.W = num3;
            return quaternion;
        }

        public static Quaternion operator *(Quaternion quaternion1, Quaternion quaternion2)
        {
            Quaternion quaternion;
            double x = quaternion1.X;
            double y = quaternion1.Y;
            double z = quaternion1.Z;
            double w = quaternion1.W;
            double num4 = quaternion2.X;
            double num3 = quaternion2.Y;
            double num2 = quaternion2.Z;
            double num = quaternion2.W;
            double num12 = (y * num2) - (z * num3);
            double num11 = (z * num4) - (x * num2);
            double num10 = (x * num3) - (y * num4);
            double num9 = ((x * num4) + (y * num3)) + (z * num2);
            quaternion.X = ((x * num) + (num4 * w)) + num12;
            quaternion.Y = ((y * num) + (num3 * w)) + num11;
            quaternion.Z = ((z * num) + (num2 * w)) + num10;
            quaternion.W = (w * num) - num9;
            return quaternion;
        }

    }
     
}
