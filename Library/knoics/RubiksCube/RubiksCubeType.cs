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
        public int Width { get; set;}   //x number of cubies
        public int Height { get; set; } //y number of cubies
        public int Depth { get; set; }  //z number of cubies
    }


    internal struct FaceConfig
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public Axis Normal { get; set; }
    }

    /*
    public struct Color
    {
        public byte A {get;set;}
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public Color(byte a, byte r, byte g, byte b):this()
        {
            A = a; R = r; G = g; B = b;
        }

        public readonly static Color Red = new Color(255, 0xD3, 0x20, 0x32);
        public readonly static Color Green = new Color(255, 0, 0xA0, 0x4C);
        public readonly static Color Blue = new Color(255, 0x0E, 0x33, 0xDE);
        public readonly static Color White = new Color(255, 0xF0, 0xEE, 0xED);
        public readonly static Color Yellow = new Color(255, 255, 255, 0);
        public readonly static Color Orange = new Color(255, 0xFB, 0x79, 0x08);
        public readonly static Color Black = new Color(255, 0, 0, 0);

    }
    */


}
