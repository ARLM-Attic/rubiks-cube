#region Description
//-----------------------------------------------------------------------------
// File:        Point2.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Knoics.Math
{
    public struct Point2
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point2(double x, double y)
            : this()
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", X, Y);
        }

    }
}
