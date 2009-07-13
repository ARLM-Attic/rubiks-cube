//-----------------------------------------------------------------------------
// File:        Angle.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        02/21/2009
//-----------------------------------------------------------------------------
using System;
using System.Net;

namespace Knoics.Math
{
    public class Angle
    {
        public static double RadiansToDegrees(double radians)
        {
            return (radians * 57.295779513082323);
        }

        public static double DegreesToRadians(double degrees)
        {
            return (degrees / 57.295779513082323);
        }

    }
}
