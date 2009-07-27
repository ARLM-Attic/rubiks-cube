#region Description
//-----------------------------------------------------------------------------
// File:        AnimContext.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Knoics.RubiksCube
{
    public class AnimContext
    {
        public AnimContext(double angle, double frames, string op, bool silent, List<Transform> transformParams)
        {
            RotatedAngle = angle;
            Frames = frames;
            Op = op;
            Silent = silent;
            TransformParams = transformParams;
        }
        public double RotatedAngle { get; set; } 
        public double Frames { get; set; }
        public string Op { get; set; }
        public bool Silent { get; set; }
        
        public List<Transform> TransformParams { get; set; } 
    }
}
