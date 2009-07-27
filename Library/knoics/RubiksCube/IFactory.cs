#region Description
//-----------------------------------------------------------------------------
// File:        IFactory.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        01/31/2009
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Net;
using Knoics.Math;
using System.Windows.Media;
using Kit3D.Windows.Media.Media3D;

namespace Knoics.RubiksCube
{

    public interface IFactory
    {
        IModel CreateModel();
        IMesh CreateMesh(string faceName, Vector3D[] vertexes, Color color);
    }

}
