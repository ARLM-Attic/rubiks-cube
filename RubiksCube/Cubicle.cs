#region Description
//-----------------------------------------------------------------------------
// File:        Cubicle.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNALib;

namespace RubiksCube
{
    class Cubicle
    {
        //Cubicle Selection --> Basic Op Map
        //UB, UF, DF, DB -> LM
        //UL, UR, DR, DL -> FM
        //LB, RB, RF, LF -> UM

        //UFL-U
        //URF-F
        //UBR-R
        //ULB-B

        //DLF-D
        //DFR-R
        //DRB-B
        //DBL-L
        public static readonly Dictionary<string, string> BasicOpFromSelectedCubicle;
        public static readonly Dictionary<Axis, string[]> BasicOpFromAxis;
        static Cubicle()
        {
            //UB, UF, DF, DB -> LM
            //UL, UR, DR, DL -> FM
            //LB, RB, RF, LF -> UM
            BasicOpFromSelectedCubicle = new Dictionary<string, string>();
            BasicOpFromSelectedCubicle.Add("UB", "LM"); BasicOpFromSelectedCubicle.Add("UF", "LM"); BasicOpFromSelectedCubicle.Add("DF", "LM"); BasicOpFromSelectedCubicle.Add("DB", "LM");
            BasicOpFromSelectedCubicle.Add("UL", "FM"); BasicOpFromSelectedCubicle.Add("UR", "FM"); BasicOpFromSelectedCubicle.Add("DR", "FM"); BasicOpFromSelectedCubicle.Add("DL", "FM");
            BasicOpFromSelectedCubicle.Add("LB", "UM"); BasicOpFromSelectedCubicle.Add("RB", "UM"); BasicOpFromSelectedCubicle.Add("RF", "UM"); BasicOpFromSelectedCubicle.Add("LF", "UM");

            //UFL-U
            //URF-F
            //UBR-R
            //ULB-B
            //UFLX, selected cubicles
            BasicOpFromSelectedCubicle.Add("UFL", "U"); 
            BasicOpFromSelectedCubicle.Add("URF", "F"); BasicOpFromSelectedCubicle.Add("UBR", "R"); BasicOpFromSelectedCubicle.Add("ULB", "B");

            //DLF-D
            //DFR-R
            //DRB-B
            //DBL-L
            BasicOpFromSelectedCubicle.Add("DLF", "D"); BasicOpFromSelectedCubicle.Add("DFR", "R"); BasicOpFromSelectedCubicle.Add("DRB", "B"); BasicOpFromSelectedCubicle.Add("DBL", "L");


            BasicOpFromAxis = new Dictionary<Axis, string[]>();
            BasicOpFromAxis.Add(Axis.X, new string[] { "L", "R" });
            BasicOpFromAxis.Add(Axis.Y, new string[] { "U", "D" });
            BasicOpFromAxis.Add(Axis.Z, new string[] { "F", "B" });
        }

        public string Id { get; set; }
        public Cubie Cubie{get;set;}
        public BoundingBox BoundingBox { get; set; }


        public Cubicle(Cubie cubie)
        {
            Id = cubie.Id;
            Cubie = cubie;
            BoundingBox = XNAUtils.CreateBoxFromSphere(cubie.Mesh.BoundingSphere);
        }

    }
}
