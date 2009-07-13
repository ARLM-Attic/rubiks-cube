#region Description
//-----------------------------------------------------------------------------
// File:        CubeTransform.cs
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

namespace Knoics.RubiksCube
{
    public abstract class Transform
    {
        public bool Silent { get; set; }
        public double ChangeAngle { get; set; } //net change in angle, somtimes it already rotated manually by user

        //needs to be refactored, Animation use only
        public double Begin { get; set; }
        public double Delta { get; set; }
        public RubiksCube Cube { get; set; }

        public abstract void BeforeTransform();
        public abstract void DoTransform(double deltaAngle);
        public abstract void AfterTransform(double rotationAngle);
    }

    public class CubieTransform : Transform
    {
        public string Op { get; set; }
        public BasicOp BasicOp { get; set; }
        public bool IsReversedBasicOp { get; set; }
        public IEnumerable<Cubicle> AffectedCubicles { get; set; }
        

        public double RotateAngle { get { return IsReversedBasicOp ? -BasicOp.RotationAngle : BasicOp.RotationAngle; } } //standard angle


        public override void BeforeTransform()
        {
            AffectedCubicles = BasicOp.CubicleGroup.Select(c => Cube. Cubicles[c]);
        }

        public override void DoTransform(double deltaAngle)
        {
            //Debug.WriteLine(string.Format("transform bones: {0}", bones.Count()));
            foreach (Cubicle cubicle in AffectedCubicles)
            {
                cubicle.Cubie.Rotate(BasicOp.Axis, deltaAngle, false);
                //Debug.WriteLine(string.Format("{0}", bone.Name));
            }
        }


        public override void AfterTransform(double rotationAngle)
        {
            foreach (Cubicle cubicle in AffectedCubicles)
            {
                Cubie cubie = cubicle.Cubie;
                cubie.RotateUnit(BasicOp.Axis, RotateAngle);// rotationAngle);
                //Debug.WriteLine(string.Format("{0}", bone.Name));
            }

            foreach (CubicleOrientation[] change in BasicOp.CubicleGroupCycles)
            {
                CubicleOrientation[] cycle;
                if (IsReversedBasicOp)
                {
                    cycle = change.Reverse().ToArray();
                }
                else
                    cycle = change.ToArray();

                Cycle(cycle);
            }
            
            
        }


        private void Cycle(CubicleOrientation[] cycle)
        {
            try
            {
                if (cycle.Count() < 2) return;
                int count = cycle.Count();
                CubicleOrientation fromOrientation = cycle[count - 1];
                Cubicle fromCubicle = Cube.Cubicles[fromOrientation.Name];
                Cubie fromCubie = fromCubicle.Cubie;
                Dictionary<string, CubieFace> fromFaces = fromCubicle.CubieFaces;
                if (fromCubie == null) Debug.Assert(false);
                for (int i = 0; i < count; i++)
                {
                    CubicleOrientation toOrientation = cycle[i];
                    Cubicle toCubicle = Cube.Cubicles[toOrientation.Name];
                    fromCubie = toCubicle.SetCubie(fromCubie);
                    fromFaces = toCubicle.SetCubieFaces(fromFaces, fromOrientation.OrientationName, toOrientation.OrientationName);
                    fromOrientation = toOrientation;
                }
            }
            catch (Exception)
            {
                Debug.Assert(false);
            }
        }
    }


    public class FaceTransform : Transform
    {
        public string Face { get; set; }
        public IEnumerable<CubicleFace> AffectedFaces { get; set; }
        public Vector3D AxisTranslationFromOrigin { get; set; }
        public Vector3D Axis2TranslationFromOrigin { get; set; }
        public Axis Axis { get; set; }
        public bool IsAxisMoving { get; set; }
        public override void BeforeTransform()
        {
            AffectedFaces = GetFaces(Face);
        }

        private IEnumerable<CubicleFace> GetFaces(string faceName)
        {
            foreach (Cubicle cubicle in  Cube.Cubicles.Values) {
                if (cubicle.Name.IndexOf(faceName) >= 0)
                {
                    yield return cubicle.Faces[faceName];
                }
            }
        }

        
        Dictionary<CubieFace, Vector3D> _axisTranslation = new Dictionary<CubieFace, Vector3D>();
        
        public override void DoTransform(double deltaAngle)
        {
          //  Debug.WriteLine("start DoTransform");
            foreach (CubicleFace face in AffectedFaces)
            {
                Cubie cubie = face.CubieFace.Cubie;
                CubieFace cubieFace = face.CubieFace;
                //cubicle.Cubie.Rotate(BasicOp.Axis, deltaAngle, false);
                Vector3D axis = face.CubieFace.Cubie.UnitX;
                
                if (Axis == Axis.X)
                {
                    axis = face.CubieFace.Cubie.UnitX;
                
                }
                else if (Axis == Axis.Y)
                {
                    axis = face.CubieFace.Cubie.UnitY;
                
                }
                else if (Axis == Axis.Z)
                {
                    axis = face.CubieFace.Cubie.UnitZ;
                
                }

                Quaternion3D rot =  Quaternion3D.CreateFromAxisAngle(axis, deltaAngle);
                Matrix3D rotMatrix = Ext3D.CreateFromQuaternion(rot);
                Matrix3D matrix = cubie.Transform;// Matrix.Invert(cubie.Transform);
                matrix.Invert();
                
                
                Matrix3D rotation = Ext3D.CreateTranslation(Ext3D.Transform(-AxisTranslationFromOrigin, matrix)) * rotMatrix * Ext3D.CreateTranslation(Ext3D.Transform(AxisTranslationFromOrigin, matrix));
                if (IsAxisMoving)
                {
                    Vector3D v1;
                    if (!_axisTranslation.ContainsKey(cubieFace))
                    {
                        Matrix3D m = cubieFace.Transform; m.Invert();
                        v1 = Ext3D.Transform(Axis2TranslationFromOrigin, m);
                        _axisTranslation.Add(cubieFace, v1);
                    }
                    v1 = _axisTranslation[cubieFace];
                    Matrix3D r = rotation; r.Invert();
                    v1 = Ext3D.Transform(v1, r);// Matrix.Invert(rotation));
                    
                    Matrix3D rotationAxis = Ext3D.CreateTranslation(-v1) * rotMatrix * Ext3D.CreateTranslation(v1);
                    rotation = rotationAxis * rotation;

                    _axisTranslation[cubieFace] = v1;
                    
                }
                cubieFace.DoTransform(rotation, false);
            }
        }

        public override void AfterTransform(double rotationAngle)
        {
            
        }
    }

}
