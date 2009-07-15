#region Description
//-----------------------------------------------------------------------------
// File:        Matrix.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Reference:   Microsoft.Xna.Framework and System.Windows.Media.Media3D
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Net;

namespace Knoics.Math
{
    public struct Matrix 
    {
        public double M11;
        public double M12;
        public double M13;
        public double M14;
        public double M21;
        public double M22;
        public double M23;
        public double M24;
        public double M31;
        public double M32;
        public double M33;
        public double M34;
        public double M41;
        public double M42;
        public double M43;
        public double M44;

        private static Matrix _identity;
        public static Matrix Identity
        {
            get
            {
                return _identity;
            }
        }
 

 
        static Matrix()
        {
            _identity = new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
        }

        public Matrix(double m11, double m12, double m13, double m14, double m21, double m22, double m23, double m24, double m31, double m32, double m33, double m34, double m41, double m42, double m43, double m44)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = m14;
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = m24;
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = m34;
            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
            this.M44 = m44;
            this._isNotKnownToBeIdentity = true;
        }

        internal static Matrix CreateFromQuaternion(Quaternion quaternion)
        {
            Matrix matrix = new Matrix();
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
            matrix.M41 = 0f;
            matrix.M42 = 0f;
            matrix.M43 = 0f;
            matrix.M44 = 1f;
            return matrix;
        }


        public static Matrix CreateRotationX(double radians)
        {
            Matrix matrix = new Matrix();
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
            matrix.M41 = 0f;
            matrix.M42 = 0f;
            matrix.M43 = 0f;
            matrix.M44 = 1f;
            return matrix;
        }

        public static Matrix CreateRotationY(double radians)
        {
            Matrix matrix = new Matrix();
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
            matrix.M41 = 0f;
            matrix.M42 = 0f;
            matrix.M43 = 0f;
            matrix.M44 = 1f;
            return matrix;
        }


        public static Matrix CreateRotationZ(double radians)
        {
            Matrix matrix = new Matrix();
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
            matrix.M41 = 0f;
            matrix.M42 = 0f;
            matrix.M43 = 0f;
            matrix.M44 = 1f;
            return matrix;
        }

        public static Matrix CreateTranslation(Vector3 position)
        {
            Matrix matrix = new Matrix();
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
            matrix.M41 = position.X;
            matrix.M42 = position.Y;
            matrix.M43 = position.Z;
            matrix.M44 = 1;
            return matrix;
        }


        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            Matrix matrix =new Matrix();
            matrix.M11 = (((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31)) + (matrix1.M14 * matrix2.M41);
            matrix.M12 = (((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32)) + (matrix1.M14 * matrix2.M42);
            matrix.M13 = (((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33)) + (matrix1.M14 * matrix2.M43);
            matrix.M14 = (((matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24)) + (matrix1.M13 * matrix2.M34)) + (matrix1.M14 * matrix2.M44);
            matrix.M21 = (((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31)) + (matrix1.M24 * matrix2.M41);
            matrix.M22 = (((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32)) + (matrix1.M24 * matrix2.M42);
            matrix.M23 = (((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33)) + (matrix1.M24 * matrix2.M43);
            matrix.M24 = (((matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24)) + (matrix1.M23 * matrix2.M34)) + (matrix1.M24 * matrix2.M44);
            matrix.M31 = (((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31)) + (matrix1.M34 * matrix2.M41);
            matrix.M32 = (((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32)) + (matrix1.M34 * matrix2.M42);
            matrix.M33 = (((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33)) + (matrix1.M34 * matrix2.M43);
            matrix.M34 = (((matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24)) + (matrix1.M33 * matrix2.M34)) + (matrix1.M34 * matrix2.M44);
            matrix.M41 = (((matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21)) + (matrix1.M43 * matrix2.M31)) + (matrix1.M44 * matrix2.M41);
            matrix.M42 = (((matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22)) + (matrix1.M43 * matrix2.M32)) + (matrix1.M44 * matrix2.M42);
            matrix.M43 = (((matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23)) + (matrix1.M43 * matrix2.M33)) + (matrix1.M44 * matrix2.M43);
            matrix.M44 = (((matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24)) + (matrix1.M43 * matrix2.M34)) + (matrix1.M44 * matrix2.M44);
            return matrix;
        }



        public static Matrix Invert(Matrix matrix)
        {
            Matrix matrix2 = new Matrix();

            double num5 = matrix.M11;
            double num4 = matrix.M12;
            double num3 = matrix.M13;
            double num2 = matrix.M14;
            double num9 = matrix.M21;
            double num8 = matrix.M22;
            double num7 = matrix.M23;
            double num6 = matrix.M24;
            double num17 = matrix.M31;
            double num16 = matrix.M32;
            double num15 = matrix.M33;
            double num14 = matrix.M34;
            double num13 = matrix.M41;
            double num12 = matrix.M42;
            double num11 = matrix.M43;
            double num10 = matrix.M44;
            double num23 = (num15 * num10) - (num14 * num11);
            double num22 = (num16 * num10) - (num14 * num12);
            double num21 = (num16 * num11) - (num15 * num12);
            double num20 = (num17 * num10) - (num14 * num13);
            double num19 = (num17 * num11) - (num15 * num13);
            double num18 = (num17 * num12) - (num16 * num13);
            double num39 = ((num8 * num23) - (num7 * num22)) + (num6 * num21);
            double num38 = -(((num9 * num23) - (num7 * num20)) + (num6 * num19));
            double num37 = ((num9 * num22) - (num8 * num20)) + (num6 * num18);
            double num36 = -(((num9 * num21) - (num8 * num19)) + (num7 * num18));
            double num = 1f / ((((num5 * num39) + (num4 * num38)) + (num3 * num37)) + (num2 * num36));
            matrix2.M11 = num39 * num;
            matrix2.M21 = num38 * num;
            matrix2.M31 = num37 * num;
            matrix2.M41 = num36 * num;
            matrix2.M12 = -(((num4 * num23) - (num3 * num22)) + (num2 * num21)) * num;
            matrix2.M22 = (((num5 * num23) - (num3 * num20)) + (num2 * num19)) * num;
            matrix2.M32 = -(((num5 * num22) - (num4 * num20)) + (num2 * num18)) * num;
            matrix2.M42 = (((num5 * num21) - (num4 * num19)) + (num3 * num18)) * num;
            double num35 = (num7 * num10) - (num6 * num11);
            double num34 = (num8 * num10) - (num6 * num12);
            double num33 = (num8 * num11) - (num7 * num12);
            double num32 = (num9 * num10) - (num6 * num13);
            double num31 = (num9 * num11) - (num7 * num13);
            double num30 = (num9 * num12) - (num8 * num13);
            matrix2.M13 = (((num4 * num35) - (num3 * num34)) + (num2 * num33)) * num;
            matrix2.M23 = -(((num5 * num35) - (num3 * num32)) + (num2 * num31)) * num;
            matrix2.M33 = (((num5 * num34) - (num4 * num32)) + (num2 * num30)) * num;
            matrix2.M43 = -(((num5 * num33) - (num4 * num31)) + (num3 * num30)) * num;
            double num29 = (num7 * num14) - (num6 * num15);
            double num28 = (num8 * num14) - (num6 * num16);
            double num27 = (num8 * num15) - (num7 * num16);
            double num26 = (num9 * num14) - (num6 * num17);
            double num25 = (num9 * num15) - (num7 * num17);
            double num24 = (num9 * num16) - (num8 * num17);
            matrix2.M14 = -(((num4 * num29) - (num3 * num28)) + (num2 * num27)) * num;
            matrix2.M24 = (((num5 * num29) - (num3 * num26)) + (num2 * num25)) * num;
            matrix2.M34 = -(((num5 * num28) - (num4 * num26)) + (num2 * num24)) * num;
            matrix2.M44 = (((num5 * num27) - (num4 * num25)) + (num3 * num24)) * num;

            return matrix2;
        }

        private void SetTranslationMatrix(ref Vector3 offset)
        {
            this.M11 = this.M22 = this.M33 = this.M44 = 1.0;
            this.M41 = offset.X;
            this.M42 = offset.Y;
            this.M43 = offset.Z;
            this.IsDistinguishedIdentity = false;
        }

        private bool _isNotKnownToBeIdentity;
        private bool IsDistinguishedIdentity
        {
            get
            {
                return !this._isNotKnownToBeIdentity;
            }
            set
            {
                this._isNotKnownToBeIdentity = !value;
            }
        }


        public void Translate(Vector3 offset)
        {
            if (this.IsDistinguishedIdentity)
            {
                this.SetTranslationMatrix(ref offset);
            }
            else
            {
                this.M11 += this.M14 * offset.X;
                this.M12 += this.M14 * offset.Y;
                this.M13 += this.M14 * offset.Z;
                this.M21 += this.M24 * offset.X;
                this.M22 += this.M24 * offset.Y;
                this.M23 += this.M24 * offset.Z;
                this.M31 += this.M34 * offset.X;
                this.M32 += this.M34 * offset.Y;
                this.M33 += this.M34 * offset.Z;
                this.M41 += this.M44 * offset.X;
                this.M42 += this.M44 * offset.Y;
                this.M43 += this.M44 * offset.Z;
            }
        }
    }
}
