#region Description
//-----------------------------------------------------------------------------
// File:        Cubie.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        01/31/2009
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Knoics.Math;
using Kit3D.Windows.Media.Media3D;
//using Knoics.Interactive;
using Knoics.Math;

namespace Knoics.RubiksCube
{
    public class Cubie : RotatableObject
    {
        private string _name;
        public readonly Dictionary<string, CubieFace> Faces = new Dictionary<string,CubieFace>();

        public string Name
        {
            get { return _name; }
        }


        public BoundingBox3D BoundingBox
        {
            get
            {
                double size = _size; 
                Vector3D min = _center; min.X -= size / 2; min.Y -= size / 2; min.Z -= size / 2;
                Vector3D max = _center; max.X += size / 2; max.Y += size / 2; max.Z += size / 2;
                return new BoundingBox3D(min, max);
            }
        }

        private Vector3D _center;
        public Vector3D Center { get { return _center; } }
        private Double _size;
        public double Size { get { return _size; } }

        private Cubie(Cubicle cubicle, string name, Vector3D center, double size) { 
            _name = name; 
            _cubicle = cubicle;
            _center = center;
            _size = size;
        }

        private Cubicle _cubicle;
        public Cubicle Cubicle { get { return _cubicle; } internal set { _cubicle = value; } }
        public static Cubie CreateCubie(Cubicle cubicle, string name, Vector3D center, double size)
        {
            Cubie cubie = new Cubie(cubicle, name, center, size);
            //Debug.WriteLine(name);
            Vector3D c = center;
            double offset = size /2;
            CubieFace face = null;
            int i = 0;
            if (name.IndexOf("U") >= 0)
            {
                c = center; c.Y += offset;
                face = CubieFace.ConstructCubieFace(cubie, "U", c, size);
                cubie.Faces.Add("U", face);
                cubicle.SetCubieFace("U", face);
            }

            if (name.IndexOf("D") >= 0)
            {
                c = center; c.Y -= offset;
                face = CubieFace.ConstructCubieFace(cubie, "D", c, size);
                cubie.Faces.Add("D", face);
                cubicle.SetCubieFace("D", face);
            }

            if (name.IndexOf("F") >= 0)
            {
                c = center; c.Z += offset;
                face = CubieFace.ConstructCubieFace(cubie, "F", c, size);
                cubie.Faces.Add("F", face);
                cubicle.SetCubieFace("F", face);
            }

            if (name.IndexOf("B") >= 0)
            {
                c = center; c.Z -= offset;
                face = CubieFace.ConstructCubieFace(cubie, "B", c, size);
                cubie.Faces.Add("B", face);
                cubicle.SetCubieFace("B", face);
            }

            if (name.IndexOf("L") >= 0)
            {
                c = center; c.X -= offset;
                face = CubieFace.ConstructCubieFace(cubie, "L", c, size);
                cubie.Faces.Add("L", face);
                cubicle.SetCubieFace("L", face);
            }

            if (name.IndexOf("R") >= 0)
            {
                c = center; c.X += offset;
                face = CubieFace.ConstructCubieFace(cubie, "R", c, size);
                cubie.Faces.Add("R", face);
                cubicle.SetCubieFace("R", face);
            }

            return cubie;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", _cubicle, _name);
        }

        /*
        public Matrix Transform { get { return _transform; } }
        private Matrix _transform = Matrix.Identity;
        private Matrix _savedTransform = Matrix.Identity;

        private Matrix _axisTransform = Matrix.Identity;
        private Vector3 _unitX = Vector3.UnitX;
        private Vector3 _unitY = Vector3.UnitY;
        private Vector3 _unitZ = Vector3.UnitZ;


        public Vector3 UnitX { get { return _unitX; } }
        public Vector3 UnitY { get { return _unitY; } }
        public Vector3 UnitZ { get { return _unitZ; } }
        */

        public override void Reset()
        {
            base.Reset();
            foreach (CubieFace face in Faces.Values) face.Reset();
        }

        public override void Restore()
        {
            base.Restore();
            foreach (CubieFace face in Faces.Values) face.Restore();
            _center = Ext3D.Transform(_center, _transform);

            
        }

        public override void Save()
        {
            base.Save();
            foreach (CubieFace face in Faces.Values) face.Save();
        }

        public override Matrix3D Rotate(Axis axis, double deltaAngle, bool isFromSaved)
        {
            Matrix3D matrix = base.Rotate(axis, deltaAngle, isFromSaved);

            _center = Ext3D.Transform(_center,  _transform);
            foreach (CubieFace face in Faces.Values)
            {
                face.DoTransform(matrix, isFromSaved);
            }
            return matrix;
        }
 
        
    }
}
