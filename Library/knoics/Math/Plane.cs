#region Description
//-----------------------------------------------------------------------------
// File:        Plane.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Description: Plane structure
// Reference:   Microsoft.Xna.Framework and System.Windows.Media.Media3D
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Net;

namespace Knoics.Math
{
    public struct Plane
    {
        private Vector3 _normal;
        private double _d;

        public Plane(double a, double b, double c, double d)
        {
            this._normal = new Vector3(a, b, c);
            this._normal.Normalize();
            this._d = d;
        }

        public Plane(Point3 p0, Point3 p1, Point3 p2)
        {
            Vector3 v1 = p1 - p0;
            Vector3 v2 = p2 - p0;
            this._normal = Vector3.Cross(v1, v2);
            this._normal.Normalize();
            this._d = -(this._normal.X * p0.X + this._normal.Y * p0.Y + this._normal.Z * p0.Z);
        }

        public double D
        {
            get
            {
                return this._d;
            }
        }

        public Vector3 Normal
        {
            get
            {
                return this._normal;
            }
        }

        public double DistanceToPoint(Point3 point)
        {
            return point.X * this._normal.X +
                   point.Y * this._normal.Y +
                   point.Z * this._normal.Z +
                   this._d;
        }

        public double? Intersects(Ray ray)
        {
            double num2 = ((this.Normal.X * ray.Direction.X) + (this.Normal.Y * ray.Direction.Y)) + (this.Normal.Z * ray.Direction.Z);
            if (System.Math.Abs(num2) < 1E-12f)
            {
                return null;
            }
            double num3 = ((this.Normal.X * ray.Origin.X) + (this.Normal.Y * ray.Origin.Y)) + (this.Normal.Z * ray.Origin.Z);
            double d = (-this.D - num3) / num2;
            if (d < 0f)
            {
                if (d < -1E-12f)
                {
                    return null;
                }
                d = 0;
            }
            return new double?(d);
        }

        public bool Intersect(Ray ray, out Point3 intersectionPoint)
        {   
            /*
            double d = Vector3.Dot(this.Normal, ray.Direction);
            if ((d > -1e-12) && (d < 1e-12))
            {
                intersectionPoint = new Point3(double.NaN, double.NaN, double.NaN);
                return false;
            }

            double t = -(Vector3.Dot((Vector3)ray.Origin, this.Normal) + this.D) / d;
            if (t < 0)
            {
                intersectionPoint = new Point3(double.NaN, double.NaN, double.NaN);
                return false;
            }
            intersectionPoint = ray.Origin + t * ray.Direction;
            return true;
             */
            double? d = Intersects(ray);
            intersectionPoint = new Point3(double.NaN, double.NaN, double.NaN);
            if (d != null)
            {
                intersectionPoint = ray.Origin + (double)d * ray.Direction;
            }
            return (d != null);
        }


    }
}
