using System;
using System.Net;
using Knoics.Math;
using Kit3D.Windows.Media.Media3D;
namespace Knoics.RubiksCube
{

    public interface ITransform
    {
        void Reset();
        void Save();
        void Restore();
        void DoTransform(Matrix3D matrix, bool isFromSaved);
        Matrix3D Transform { get; }
    }

}
