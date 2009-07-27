using System;
using System.Net;
using Kit3D.Windows.Media.Media3D;
using System.Windows;

namespace Knoics.Math
{

    public struct Ray3D
    {
        private Point3D origin;
        private Vector3D direction;

        public Ray3D(Point3D origin, Vector3D direction)
        {
            this.origin = origin;

            direction.Normalize();
            this.direction = direction;
        }

        public Point3D Origin
        {
            get { return this.origin; }
        }

        public Vector3D Direction
        {
            get { return this.direction; }
        }

        public override string ToString()
        {

            return string.Format("o: {0}, d: {1}", origin, direction);
        }
    }


    public struct Quaternion3D
    {
        public double X;
        public double Y;
        public double Z;
        public double W;
        private static Quaternion3D _identity;
        public static Quaternion3D Identity { get { return _identity; } }

        static Quaternion3D()
        {
            _identity = new Quaternion3D(0, 0, 0, 1);
        }
        public Quaternion3D(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /*
        public Quaternion3D(Vector3D axisOfRotation, double angleInDegrees)
        {
            angleInDegrees = angleInDegrees % 360.0F;
            double num2 = angleInDegrees * 0.017453292519943295F;
            double length = axisOfRotation.Length;
            if (length == 0.0)
            {
                throw new InvalidOperationException("error vector");
            }
            Vector3D vectord = (Vector3D)((axisOfRotation / length) * System.Math.Sin(0.5 * num2));
            this.X = vectord.X;
            this.Y = vectord.Y;
            this.Z = vectord.Z;
            this.W = System.Math.Cos(0.5 * num2);
            //this._isNotDistinguishedIdentity = true;
        }
        */

        public static Quaternion3D CreateFromAxisAngle(Vector3D axis, double angle)
        {
            Quaternion3D quaternion = new Quaternion3D();
            double num2 = angle * 0.5;
            double num = System.Math.Sin(num2);
            double num3 = System.Math.Cos(num2);
            quaternion.X = axis.X * num;
            quaternion.Y = axis.Y * num;
            quaternion.Z = axis.Z * num;
            quaternion.W = num3;
            return quaternion;
        }

        public static Quaternion3D operator *(Quaternion3D quaternion1, Quaternion3D quaternion2)
        {
            Quaternion3D quaternion = new Quaternion3D();
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

    public struct BoundingBox3D
    {
        public Vector3D Min { get; set; }
        public Vector3D Max { get; set; }
        public BoundingBox3D(Vector3D min, Vector3D max)
            : this()
        {
            Min = min;
            Max = max;
        }

        public double? Intersects(Ray3D ray)
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
                maxValue = (ty2 > maxValue) ? maxValue : ty2;
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
                d = (tz1 > d) ? tz1 : d;
                maxValue = (tz2 > maxValue) ? maxValue : tz2;
                if (d > maxValue)
                {
                    return null;
                }
            }
            return new double?(d);
        }


    }


    public struct Plane3D
    {
        private Vector3D _normal;
        private double _d;

        public Plane3D(Vector3D normal, double d)
        {
            normal.Normalize();
            this._normal = normal;
            this._d = d;
        }

        public Plane3D(Point3D p0, Point3D p1, Point3D p2)
        {
            Vector3D v1 = p1 - p0;
            Vector3D v2 = p2 - p0;
            this._normal = Vector3D.CrossProduct(v1, v2);
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

        public Vector3D Normal
        {
            get
            {
                return this._normal;
            }
        }

        public double DistanceToPoint(Point3D point)
        {
            return point.X * this._normal.X +
                   point.Y * this._normal.Y +
                   point.Z * this._normal.Z +
                   this._d;
        }

        public double? Intersects(Ray3D ray)
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

        public bool Intersect(Ray3D ray, out Point3D intersectionPoint)
        {
            double? d = Intersects(ray);
            intersectionPoint = new Point3D(double.NaN, double.NaN, double.NaN);
            if (d != null)
            {
                intersectionPoint = ray.Origin + (double)d * ray.Direction;
            }
            return (d != null);
        }


    }

    public class Ext3D
    {
        private static Vector3D _unitX = new Vector3D(1, 0, 0);
        private static Vector3D _unitY = new Vector3D(0, 1, 0);
        private static Vector3D _unitZ = new Vector3D(0, 0, 1);

        public static Vector3D UnitX { get { return _unitX; } }
        public static Vector3D UnitY { get { return _unitY; } }
        public static Vector3D UnitZ { get { return _unitZ; } }



        public static double AngleBetween(Vector3D vector1, Vector3D vector2)
        {
            double num;
            vector1.Normalize();
            vector2.Normalize();
            if (Vector3D.DotProduct(vector1, vector2) < 0.0)
            {
                Vector3D vectord2 = -vector1 - vector2;
                num = 3.1415926535897931 - (2.0 * System.Math.Asin(vectord2.Length / 2.0));
            }
            else
            {
                Vector3D vectord = vector1 - vector2;
                num = 2.0 * System.Math.Asin(vectord.Length / 2.0);
            }
            return Angle.RadiansToDegrees(num);
        }


        public static Vector3D Transform(Vector3D position, Matrix3D matrix)
        {
            Vector3D vector = new Vector3D();
            double x = (((position.X * matrix.M11) + (position.Y * matrix.M21)) + (position.Z * matrix.M31)) + matrix.OffsetX;
            double y = (((position.X * matrix.M12) + (position.Y * matrix.M22)) + (position.Z * matrix.M32)) + matrix.OffsetY;
            double z = (((position.X * matrix.M13) + (position.Y * matrix.M23)) + (position.Z * matrix.M33)) + matrix.OffsetZ;
            vector.X = x;
            vector.Y = y;
            vector.Z = z;
            return vector;
        }

        public static Matrix3D CreateRotationX(double radians)
        {
            Matrix3D matrix = new Matrix3D();
            double num2 = System.Math.Cos(radians);
            double num = System.Math.Sin(radians);
            matrix.M11 = 1f;
            matrix.M12 = 0f;
            matrix.M13 = 0f;
            matrix.M14 = 0f;
            matrix.M21 = 0f;
            matrix.M22 = num2;
            matrix.M23 = num;
            matrix.M24 = 0f;
            matrix.M31 = 0f;
            matrix.M32 = -num;
            matrix.M33 = num2;
            matrix.M34 = 0f;
            matrix.OffsetX = 0f;
            matrix.OffsetY = 0f;
            matrix.OffsetZ = 0f;
            matrix.M44 = 1f;
            return matrix;
        }

        public static Matrix3D CreateRotationY(double radians)
        {
            Matrix3D matrix = new Matrix3D();
            double num2 = System.Math.Cos(radians);
            double num = System.Math.Sin(radians);
            matrix.M11 = num2;
            matrix.M12 = 0f;
            matrix.M13 = -num;
            matrix.M14 = 0f;
            matrix.M21 = 0f;
            matrix.M22 = 1f;
            matrix.M23 = 0f;
            matrix.M24 = 0f;
            matrix.M31 = num;
            matrix.M32 = 0f;
            matrix.M33 = num2;
            matrix.M34 = 0f;
            matrix.OffsetX = 0f;
            matrix.OffsetY = 0f;
            matrix.OffsetZ = 0f;
            matrix.M44 = 1f;
            return matrix;
        }


        public static Matrix3D CreateRotationZ(double radians)
        {
            Matrix3D matrix = new Matrix3D();
            double num2 = System.Math.Cos(radians);
            double num = System.Math.Sin(radians);
            matrix.M11 = num2;
            matrix.M12 = num;
            matrix.M13 = 0f;
            matrix.M14 = 0f;
            matrix.M21 = -num;
            matrix.M22 = num2;
            matrix.M23 = 0f;
            matrix.M24 = 0f;
            matrix.M31 = 0f;
            matrix.M32 = 0f;
            matrix.M33 = 1f;
            matrix.M34 = 0f;
            matrix.OffsetX = 0f;
            matrix.OffsetY = 0f;
            matrix.OffsetZ = 0f;
            matrix.M44 = 1f;
            return matrix;
        }

        public static Matrix3D CreateFromQuaternion(Quaternion3D quaternion)
        {
            Matrix3D matrix = new Matrix3D();
            double num9 = quaternion.X * quaternion.X;
            double num8 = quaternion.Y * quaternion.Y;
            double num7 = quaternion.Z * quaternion.Z;
            double num6 = quaternion.X * quaternion.Y;
            double num5 = quaternion.Z * quaternion.W;
            double num4 = quaternion.Z * quaternion.X;
            double num3 = quaternion.Y * quaternion.W;
            double num2 = quaternion.Y * quaternion.Z;
            double num = quaternion.X * quaternion.W;
            matrix.M11 = 1f - (2f * (num8 + num7));
            matrix.M12 = 2f * (num6 + num5);
            matrix.M13 = 2f * (num4 - num3);
            matrix.M14 = 0f;
            matrix.M21 = 2f * (num6 - num5);
            matrix.M22 = 1f - (2f * (num7 + num9));
            matrix.M23 = 2f * (num2 + num);
            matrix.M24 = 0f;
            matrix.M31 = 2f * (num4 + num3);
            matrix.M32 = 2f * (num2 - num);
            matrix.M33 = 1f - (2f * (num8 + num9));
            matrix.M34 = 0f;
            matrix.OffsetX = 0f;
            matrix.OffsetY = 0f;
            matrix.OffsetZ = 0f;
            matrix.M44 = 1f;
            return matrix;
        }

        public static Matrix3D CreateTranslation(Vector3D position)
        {
            Matrix3D matrix = new Matrix3D();
            matrix.M11 = 1;
            matrix.M12 = 0;
            matrix.M13 = 0;
            matrix.M14 = 0;
            matrix.M21 = 0;
            matrix.M22 = 1;
            matrix.M23 = 0;
            matrix.M24 = 0;
            matrix.M31 = 0;
            matrix.M32 = 0;
            matrix.M33 = 1;
            matrix.M34 = 0;
            matrix.OffsetX = position.X;
            matrix.OffsetY = position.Y;
            matrix.OffsetZ = position.Z;
            matrix.M44 = 1;
            return matrix;
        }


        public static Ray3D Unproject(Point pt, Point3D viewpoint, Matrix3D worldToLocal, Matrix3D viewToWorld, Matrix3D screenToViewTransform)
        {
            //Matrix screenToLocal = worldToLocal * (viewToWorld * screenToViewTransform);
            Vector3D vs = new Vector3D(pt.X, pt.Y, 1);
            Vector3D view = Ext3D.Transform(vs, screenToViewTransform);
            Vector3D world = Ext3D.Transform(view, viewToWorld);
            Vector3D to = Ext3D.Transform(world, worldToLocal);

            //Matrix worldToLocal = Matrix.Invert(world);
            Vector3D from = Ext3D.Transform((Vector3D)viewpoint, worldToLocal);
            Vector3D d = to - from; d.Normalize();
            return new Ray3D((Point3D)from, d);
        }

    }



}
