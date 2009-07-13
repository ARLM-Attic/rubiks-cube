//-----------------------------------------------------------------------------
// File:        BoundingBox.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        02/18/2009
//-----------------------------------------------------------------------------
using System;
using System.Net;
using Knoics.Math;

namespace Knoics.Math
{
    public struct BoundingBox
    {
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }
        public BoundingBox(Vector3 min, Vector3 max):this()
        {
            Min = min;
            Max = max;
        }

        public double? Intersects(Ray ray)
        {
            double d = 0f;
            double maxValue = double.MaxValue;
            if (System.Math.Abs(ray.Direction.X) < 1E-06f)
            {
                if ((ray.Origin.X < this.Min.X) || (ray.Origin.X > this.Max.X))
                {
                    return null;
                }
            }
            else
            {
                double vx = 1f / ray.Direction.X;
                double t1 = (this.Min.X - ray.Origin.X) * vx;
                double t2 = (this.Max.X - ray.Origin.X) * vx;
                if (t1 > t2)
                {
                    double num14 = t1;
                    t1 = t2;
                    t2 = num14;
                }
                d = (t1 > d) ? t1 : d;
                maxValue = (t2 > maxValue) ? maxValue : t2;
                if (d > maxValue)
                {
                    return null;
                }
            }
            if (System.Math.Abs(ray.Direction.Y) < 1E-06f)
            {
                if ((ray.Origin.Y < this.Min.Y) || (ray.Origin.Y > this.Max.Y))
                {
                    return null;
                }
            }
            else
            {
                double vy = 1f / ray.Direction.Y;
                double ty1 = (this.Min.Y - ray.Origin.Y) * vy;
                double ty2 = (this.Max.Y - ray.Origin.Y) * vy;
                if (ty1 > ty2)
                {
                    double num13 = ty1;
                    ty1 = ty2;
                    ty2 = num13;
                }
                d = (ty1 > d) ? ty1 : d;
                maxValue = (ty2 > maxValue)? maxValue:ty2;
                if (d > maxValue)
                {
                    return null;
                }
            }
            if (System.Math.Abs(ray.Direction.Z) < 1E-06f)
            {
                if ((ray.Origin.Z < this.Min.Z) || (ray.Origin.Z > this.Max.Z))
                {
                    return null;
                }
            }
            else
            {
                double vz = 1f / ray.Direction.Z;
                double tz1 = (this.Min.Z - ray.Origin.Z) * vz;
                double tz2 = (this.Max.Z - ray.Origin.Z) * vz;
                if (tz1 > tz2)
                {
                    double num12 = tz1;
                    tz1 = tz2;
                    tz2 = num12;
                }
                d = (tz1>d)? tz1 : d;
                maxValue = (tz2>maxValue)? maxValue: tz2;
                if (d > maxValue)
                {
                    return null;
                }
            }
            return new double?(d);
        }


    }
}
