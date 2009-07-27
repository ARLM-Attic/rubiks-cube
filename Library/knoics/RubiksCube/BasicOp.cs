#region Description
//-----------------------------------------------------------------------------
// File:        BasicOp.cs
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

namespace Knoics.RubiksCube
{

    public class CubicleOrientation
    {
        public string Name { get; set; }
        public string OrientationName { get; set; }

        public CubicleOrientation(string name, string orientationName)
        {
            Name = name;
            OrientationName = orientationName;
        }
    }

    public class OpFromBasic
    {
        public OpFromBasic(string op, string basicOp, RotationDirection dir)
        {
            Op = op;
            BasicOp = basicOp;
            RotationDirectionWithAxis = dir;
        }
        public string Op { get; set; }
        public string BasicOp { get; set; }
        public RotationDirection RotationDirectionWithAxis { get; set; }
    }
    

    public class BasicOp
    {
        public string Op { get; set; }
        public Axis Axis { get; set; }
        public double RotationAngle { get; set; }

        public BasicOp(string op, string[][] cycles, Axis axis, double angle)
        {
            Op = op;
            CubicleOrientationCycles = cycles;
            Axis = axis;
            RotationAngle = angle;
        }

        public string[][] CubicleOrientationCycles
        {
            set
            {
                _cubicleGroupCycles = new CubicleOrientation[value.Length][];
                _cubicleGroup = new List<string>();
                int i = 0;
                foreach (string[] orientationNames in value)
                {
                    _cubicleGroupCycles[i] = new CubicleOrientation[orientationNames.Count()];
                    int j = 0;
                    foreach (string orientationName in orientationNames)
                    {
                        CubicleOrientation co = new CubicleOrientation(GetNameFromOrientedName(orientationName), orientationName);
                        _cubicleGroupCycles[i][j++] = co;
                        _cubicleGroup.Add(co.Name);
                    }
                    i++;
                }
            }
        }


        private static string GetNameFromOrientedName(string orientationName)
        {
            string name = orientationName;
            int pos = orientationName.IndexOfAny(new char[] { 'U', 'D' });
            if (pos >= 0)
            {
                name = orientationName.Substring(pos, orientationName.Length - pos) + orientationName.Substring(0, pos);
            }
            else 
            {
                pos = orientationName.IndexOfAny(new char[] { 'L', 'R' });
                if (pos > 0)
                {
                    name = orientationName.Substring(pos, orientationName.Length - pos) + orientationName.Substring(0, pos);
                }
            }
            return name;
        }

        //types of cycle, cycle elements
        private CubicleOrientation[][] _cubicleGroupCycles;
        public CubicleOrientation[][] CubicleGroupCycles
        {
            get
            {
                return _cubicleGroupCycles;
            }
        }


        private List<string> _cubicleGroup;
        public List<string> CubicleGroup
        {
            get { return _cubicleGroup; }
        }
    }

}
