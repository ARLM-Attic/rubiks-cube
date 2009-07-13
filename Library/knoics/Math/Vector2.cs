#region Description
//-----------------------------------------------------------------------------
// File:        Vector2.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        01/31/2009
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Net;

namespace Knoics.Math
{
    public class Vector2
    {
        public double X;
        public double Y;

        public Vector2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }


        public static double AngleBetween(Vector2 vector1, Vector2 vector2)
        {
            double y = (vector1.X * vector2.Y) - (vector2.X * vector1.Y);
            double x = (vector1.X * vector2.X) + (vector1.Y * vector2.Y);
            return (System.Math.Atan2(y, x) * 57.295779513082323);
        }
    }
}
