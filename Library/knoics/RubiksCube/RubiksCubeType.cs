#region Description
//-----------------------------------------------------------------------------
// File:        RubiksCubeType.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        01/31/2009
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Knoics.Math;

using System.Windows.Media;

namespace Knoics.RubiksCube
{

    public struct CubeSize
    {
        public int Width;  //x number of cubies
        public int Height; //y number of cubies
        public int Depth;  //z number of cubies
        public CubeSize(int w, int h, int d)
        {
            Width = w; Height = h; Depth = d;
        }
    }


    internal struct FaceConfig
    {
        public string Name;
        public Color Color;
        public Axis Normal;

        public FaceConfig(Color color, string name, Axis normal)
        {
            Color = color;
            Name = name;
            Normal = normal;
        }
    }

}
