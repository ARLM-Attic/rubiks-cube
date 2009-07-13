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
    }


    public class BasicOp
    {
        public string Op { get; set; }
        public Axis Axis { get; set; }
        public double RotationAngle { get; set; }

        //types of cycle, cycle elements
        public CubicleOrientation[][] CubicleGroupCycles
        {
            get
            {
                return _cubicleGroupCycles;
            }
            /*
            set
            {
                _cubicleGroupCycles = value;
                _cubicleGroup = new List<string>();
                foreach (string[] cubicles in value)
                {
                    foreach (string cubicle in cubicles)
                    {
                        _cubicleGroup.Add(cubicle);
                    }
                }

            }
             */
        }

        public string[][] CubicleOrientationCycles
        {
            set
            {
                _cubicleGroupCycles = new CubicleOrientation[value.Count()][];
                _cubicleGroup = new List<string>();
                int i = 0;
                foreach (string[] orientationNames in value)
                {
                    _cubicleGroupCycles[i] = new CubicleOrientation[orientationNames.Count()];
                    int j = 0;
                    foreach (string orientationName in orientationNames)
                    {
                        //_cubicleGroup.Add(cubicle);
                        CubicleOrientation co = new CubicleOrientation()
                        {
                            Name = GetNameFromOrientedName(orientationName),
                            OrientationName = orientationName
                        };
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

        private CubicleOrientation[][] _cubicleGroupCycles;
        private List<string> _cubicleGroup;
        public List<string> CubicleGroup
        {
            get { return _cubicleGroup; }
        }
    }

    public class OpFromBasic
    {
        public string Op { get; set; }
        public string BasicOp { get; set; }
        public RotationDirection RotationDirectionWithAxis{get;set;}
    }
}
