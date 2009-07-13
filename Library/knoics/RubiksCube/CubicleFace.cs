using System;
using System.Net;
using Knoics.Math;

namespace Knoics.RubiksCube
{
    public class CubicleFace
    {
        public string Name { get; private set; }
        public CubieFace CubieFace { get; private set; }
        private Cubicle _cubicle;
        private Matrix _transform = Matrix.Identity;
        //public Matrix Transform { get { return _transform; } set { _transform = value; } }
        public Cubicle Cubicle { get { return _cubicle; } }
        public CubicleFace(string name, Cubicle cubicle)
        {
            Name = name;
            _cubicle = cubicle;
        }

        public void SetCubieFace(CubieFace face)
        {
            CubieFace = face;
            face.CubicleFace = this;
        }

        public override string ToString()
        {
            return string.Format("{0}==face:{1}-{2}", Cubicle.Cubie.ToString(), Name,  CubieFace.Name); 
            
        }
    }
}
