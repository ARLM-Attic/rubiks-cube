#region Description
//-----------------------------------------------------------------------------
// File:        Point3.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Knoics.Math
{
    public struct Point3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point3(double x, double y, double z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", X, Y, Z);
        }

        public static Vector3 operator -(Point3 value1, Point3 value2)
        {
            Vector3 vector = new Vector3();
            vector.X = value1.X - value2.X;
            vector.Y = value1.Y - value2.Y;
            vector.Z = value1.Z - value2.Z;
            return vector;
        }

        public static Point3 operator +(Point3 point, Vector3 v)
        {
            return new Point3(point.X + v.X, point.Y + v.Y, point.Z + v.Z);
        }

        public static explicit operator Vector3(Point3 p)
        {
            return new Vector3(p.X, p.Y, p.Z);
        }

    }
}
