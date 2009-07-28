#region Description
//-----------------------------------------------------------------------------
// File:        Cubicle.cs
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

    public class Cubicle
    {
        public string Name { get; private set; }
        public Cubie Cubie{get; private set;}
        public Vector3D Center { get; private set; }
        public double Size { get; private set; }
        
        public Dictionary<string, CubicleFace> Faces { get { return _cubicleFaces; } }
        
        public Dictionary<string, CubicleFace> _cubicleFaces;

        public Dictionary<string, CubieFace> CubieFaces
        {
            get
            {
                Dictionary<string, CubieFace> faces = new Dictionary<string, CubieFace>();
                foreach (string key in _cubicleFaces.Keys)
                {
                    faces.Add(key, _cubicleFaces[key].CubieFace);
                }
                return faces;
            }
        }
        public BoundingBox3D BoundingBox
        {
            get
            {
                double size = Size;
                Vector3D min = Center; min.X -= size / 2; min.Y -= size / 2; min.Z -= size / 2;
                Vector3D max = Center; max.X += size / 2; max.Y += size / 2; max.Z += size / 2;
                return new BoundingBox3D(min, max);
            }
        }

        
        private Cubicle(string name, Vector3D center, double size)
        {
            Name  = name;
            Center = center;
            Size = size;
            _cubicleFaces = new Dictionary<string, CubicleFace>();
            for (int i = 0; i < name.Length; i++)
            {
                string faceName = name.Substring(i, 1);
                int level;
                if(!Int32.TryParse(faceName,out level))
                    _cubicleFaces.Add(faceName, new CubicleFace(faceName, this));
            }
        }


        public void SetCubieFace(string faceName, CubieFace face)
        {
            _cubicleFaces[faceName].SetCubieFace(face);
        }

        //return origial Faces
        public Dictionary<string, CubieFace> SetCubieFaces(Dictionary<string, CubieFace> fromFaces, string from, string to)
        {
            Debug.Assert(from.Length == to.Length);
            Dictionary<string, CubieFace> originalFaces = CubieFaces;
            for (int i = 0; i < from.Length; i++)
            {
                string fromFace = from.Substring(i, 1);
                string toFace = to.Substring(i, 1);
                _cubicleFaces[toFace].SetCubieFace(fromFaces[fromFace]);
            }
            return originalFaces;
        }


        public static Cubicle CreateCubicle(string cubicleName, string cubieName, Vector3D center, double size, IFactory factory)
        {
            Cubicle cubicle = new Cubicle(cubicleName, center, size);
            Cubie cubie = Cubie.CreateCubie(cubicle, cubieName, center, size, factory);
            cubicle.Cubie = cubie;

            return cubicle;
        }



        public Cubie SetCubie(Cubie cubie)
        {
            Cubie original = Cubie;
            Cubie = cubie;
            cubie.Cubicle = this;
            return original;
        }

        public override string ToString()
        {
            return Name;
        }

    }

}
