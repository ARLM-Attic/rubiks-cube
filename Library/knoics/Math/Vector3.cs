#region Description
//-----------------------------------------------------------------------------
// File:        Vector3.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        01/31/2009
// Reference:   Microsoft.Xna.Framework and System.Windows.Media.Media3D
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Net;

namespace Knoics.Math
{
    public struct Vector3
    {
        private static Vector3 _unitX;
        private static Vector3 _unitY;
        private static Vector3 _unitZ;

        static Vector3()
        {
            _unitX = new Vector3(1f, 0f, 0f);
            _unitY = new Vector3(0f, 1f, 0f);
            _unitZ = new Vector3(0f, 0f, 1f);
        }

        public Vector3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }


        public double X;
        public double Y;
        public double Z;

        public static Vector3 UnitX { get { return _unitX; } }
        public static Vector3 UnitY { get { return _unitY; } }
        public static Vector3 UnitZ { get { return _unitZ; } }

        public static Vector3 CreateVector3(Point2 p, double size) 
        {
            Vector3 v = new Vector3();
            //[0,2]
            double x = p.X / (size / 2);
            double y = p.Y / (size / 2);
            
            x = x - 1; y = 1 - y;
            x = System.Math.Min(1, x); x = System.Math.Max(-1, x);
            y = System.Math.Min(1, y); y = System.Math.Max(-1, y);
            v.X = x;
            v.Y = y;
            double z2 = 1 - x * x - y * y;
            v.Z = z2 > 0 ? System.Math.Sqrt(z2) : 0;
            return v;
        }


        public static Vector3 Transform(Vector3 position, Matrix matrix)
        {
            Vector3 vector;
            double num3 = (((position.X * matrix.M11) + (position.Y * matrix.M21)) + (position.Z * matrix.M31)) + matrix.M41;
            double num2 = (((position.X * matrix.M12) + (position.Y * matrix.M22)) + (position.Z * matrix.M32)) + matrix.M42;
            double num = (((position.X * matrix.M13) + (position.Y * matrix.M23)) + (position.Z * matrix.M33)) + matrix.M43;
            vector.X = num3;
            vector.Y = num2;
            vector.Z = num;
            return vector;
        }

        public void Normalize()
        {
            double num2 = ((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z);
            double num = 1f / ((float)System.Math.Sqrt((double)num2));
            this.X *= num;
            this.Y *= num;
            this.Z *= num;
        }

        public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
        {
            Vector3 vector;
            vector.X = (vector1.Y * vector2.Z) - (vector1.Z * vector2.Y);
            vector.Y = (vector1.Z * vector2.X) - (vector1.X * vector2.Z);
            vector.Z = (vector1.X * vector2.Y) - (vector1.Y * vector2.X);
            return vector;
        }

        public static double Dot(Vector3 vector1, Vector3 vector2)
        {
            return (((vector1.X * vector2.X) + (vector1.Y * vector2.Y)) + (vector1.Z * vector2.Z));
        }


        public static double AngleBetween(Vector3 vector1, Vector3 vector2)
        {
            double num;
            vector1.Normalize();
            vector2.Normalize();
            if (Vector3.Dot(vector1, vector2) < 0.0)
            {
                Vector3 vectord2 = -vector1 - vector2;
                num = 3.1415926535897931 - (2.0 * System.Math.Asin(vectord2.Length / 2.0));
            }
            else
            {
                Vector3 vectord = vector1 - vector2;
                num = 2.0 * System.Math.Asin(vectord.Length / 2.0);
            }
            return Angle.RadiansToDegrees(num);
        }

        public static Vector3 operator-(Vector3 vector)
        {
            return new Vector3(-vector.X, -vector.Y, -vector.Z);
        }

        public static Vector3 operator -(Vector3 value1, Vector3 value2)
        {
            Vector3 vector = new Vector3();
            vector.X = value1.X - value2.X;
            vector.Y = value1.Y - value2.Y;
            vector.Z = value1.Z - value2.Z;
            return vector;
        }

        public static Vector3 operator /(Vector3 value, double divider)
        {
            Vector3 vector;
            double num = 1 / divider;
            vector.X = value.X * num;
            vector.Y = value.Y * num;
            vector.Z = value.Z * num;
            return vector;
        }

        public static Vector3 operator *(Vector3 value, double scaleFactor)
        {
            Vector3 vector;
            vector.X = value.X * scaleFactor;
            vector.Y = value.Y * scaleFactor;
            vector.Z = value.Z * scaleFactor;
            return vector;
        }

        public static Vector3 operator *(double scaleFactor, Vector3 value)
        {
            Vector3 vector;
            vector.X = value.X * scaleFactor;
            vector.Y = value.Y * scaleFactor;
            vector.Z = value.Z * scaleFactor;
            return vector;
        }
 

        public double Length
        {
            get
            {
                double num = ((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z);
                return System.Math.Sqrt(num);
            }
        }


        public static explicit operator Point3(Vector3 p)
        {
            return new Point3(p.X, p.Y, p.Z);
        }



        public override string ToString()
        {
            return string.Format("({0},{1},{2})", X, Y, Z);
        }

    }
}
