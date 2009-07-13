#region Description
//-----------------------------------------------------------------------------
// File:        CubeOperation.cs
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
    public class CubeOperation
    {
        public const double PiOver2 = System.Math.PI/2;
        public const double Pi = System.Math.PI;
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
        public static readonly Dictionary<Axis, string[]> BasicOpFromAxis;
        static CubeOperation()
        {
            /*
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
            */
            
            BasicOpFromAxis = new Dictionary<Axis, string[]>();
            BasicOpFromAxis.Add(Axis.X, new string[] { "L", "R" });
            BasicOpFromAxis.Add(Axis.Y, new string[] { "U", "D" });
            BasicOpFromAxis.Add(Axis.Z, new string[] { "F", "B" });
            



            _equivalentOp = new Dictionary<string, string>();
            _equivalentOp.Add("RM", "LM'");
            _equivalentOp.Add("RM'", "LM");
            _equivalentOp.Add("BM", "FM'");
            _equivalentOp.Add("BM'", "FM");
            _equivalentOp.Add("DM", "UM'");
            _equivalentOp.Add("DM'", "UM");

            _basicOps = new Dictionary<string, BasicOp>(); //corner cubie rotation, edge cubie rotation, center cubie

            _basicOps.Add("U", new BasicOp { Op = "U", CubicleOrientationCycles = new string[][] { new string[] { "ULB", "UBR", "URF", "UFL" }, new string[] { "UB", "UR", "UF", "UL" }, new string[] { "U" } }, Axis = Axis.Y, RotationAngle = -PiOver2 });
            _basicOps.Add("D", new BasicOp { Op = "D", CubicleOrientationCycles = new string[][] { new string[] { "DLF", "DFR", "DRB", "DBL" }, new string[] { "DF", "DR", "DB", "DL" }, new string[] { "D" } }, Axis = Axis.Y, RotationAngle = PiOver2 });
            _basicOps.Add("F", new BasicOp { Op = "F", CubicleOrientationCycles = new string[][] { new string[] { "UFL", "RFU", "DFR", "LFD" }, new string[] { "UF", "RF", "DF", "LF" }, new string[] { "F" } }, Axis = Axis.Z, RotationAngle = -PiOver2 });
            _basicOps.Add("B", new BasicOp { Op = "B", CubicleOrientationCycles = new string[][] { new string[] { "UBR", "LBU", "DBL", "RBD" }, new string[] { "RB", "UB", "LB", "DB" }, new string[] { "B" } }, Axis = Axis.Z, RotationAngle = PiOver2 });
            _basicOps.Add("L", new BasicOp { Op = "L", CubicleOrientationCycles = new string[][] { new string[] { "ULB", "FLU", "DLF", "BLD" }, new string[] { "UL", "FL", "DL", "BL" }, new string[] { "L" } }, Axis = Axis.X, RotationAngle = PiOver2 });
            _basicOps.Add("R", new BasicOp { Op = "R", CubicleOrientationCycles = new string[][] { new string[] { "URF", "BRU", "DRB", "FRD" }, new string[] { "UR", "BR", "DR", "FR" }, new string[] { "R" } }, Axis = Axis.X, RotationAngle = -PiOver2 });
            _basicOps.Add("UM", new BasicOp { Op = "UM", CubicleOrientationCycles = new string[][] { new string[] { }, new string[] { "LB", "BR", "RF", "FL" }, new string[] { "B", "R", "F", "L" } }, Axis = Axis.Y, RotationAngle = -PiOver2 });
            _basicOps.Add("FM", new BasicOp { Op = "FM", CubicleOrientationCycles = new string[][] { new string[] { }, new string[] { "UR", "RD", "DL", "LU" }, new string[] { "U", "R", "D", "L" } }, Axis = Axis.Z, RotationAngle = -PiOver2 });
            _basicOps.Add("LM", new BasicOp { Op = "LM", CubicleOrientationCycles = new string[][] { new string[] { }, new string[] { "UF", "FD", "DB", "BU" }, new string[] { "U", "F", "D", "B" } }, Axis = Axis.X, RotationAngle = PiOver2 });
            
            /*
            _basicOps.Add("U", new BasicOp { Op = "U", CubicleGroupCycles = new string[][] { new string[] { CubeConfiguration.GetCubicleName(0, 2, 0), CubeConfiguration.GetCubicleName(2, 2, 0), CubeConfiguration.GetCubicleName(2, 2, 2), CubeConfiguration.GetCubicleName(0, 2, 2) }, new string[] { CubeConfiguration.GetCubicleName(1, 2, 0), CubeConfiguration.GetCubicleName(2, 2, 1), CubeConfiguration.GetCubicleName(1, 2, 2), CubeConfiguration.GetCubicleName(0, 2, 1) }, new string[] { "U" } }, Axis = Axis.Y, RotationAngle = -PiOver2 });
            _basicOps.Add("D", new BasicOp { Op = "D", CubicleGroupCycles = new string[][] { new string[] { CubeConfiguration.GetCubicleName(0, 0, 2), CubeConfiguration.GetCubicleName(2, 0, 2), CubeConfiguration.GetCubicleName(2, 0, 0), CubeConfiguration.GetCubicleName(0, 0, 0) }, new string[] { CubeConfiguration.GetCubicleName(0, 0, 1), CubeConfiguration.GetCubicleName(1, 0, 2), CubeConfiguration.GetCubicleName(2, 0, 1), CubeConfiguration.GetCubicleName(1, 0, 0) }, new string[] { "D" } }, Axis = Axis.Y, RotationAngle = PiOver2 });
            _basicOps.Add("UM", new BasicOp { Op = "UM", CubicleGroupCycles = new string[][] { new string[] { }, new string[] { CubeConfiguration.GetCubicleName(0, 1, 0), CubeConfiguration.GetCubicleName(2, 1, 0), CubeConfiguration.GetCubicleName(2, 1, 2), CubeConfiguration.GetCubicleName(0, 1, 2) }, new string[] { "B", "R", "F", "L" } }, Axis = Axis.Y, RotationAngle = -PiOver2 });

            _basicOps.Add("B", new BasicOp { Op = "B", CubicleGroupCycles = new string[][] { new string[] { CubeConfiguration.GetCubicleName(0, 0, 0), CubeConfiguration.GetCubicleName(2, 0, 0), CubeConfiguration.GetCubicleName(2, 2, 0), CubeConfiguration.GetCubicleName(0, 2, 0) }, new string[] { CubeConfiguration.GetCubicleName(1, 0, 0), CubeConfiguration.GetCubicleName(2, 1, 0), CubeConfiguration.GetCubicleName(1, 2, 0), CubeConfiguration.GetCubicleName(0, 1, 0) }, new string[] { "B" } }, Axis = Axis.Z, RotationAngle = PiOver2 });
            _basicOps.Add("F", new BasicOp { Op = "F", CubicleGroupCycles = new string[][] { new string[] { CubeConfiguration.GetCubicleName(0, 2, 2), CubeConfiguration.GetCubicleName(2, 2, 2), CubeConfiguration.GetCubicleName(2, 0, 2), CubeConfiguration.GetCubicleName(0, 0, 2) }, new string[] { CubeConfiguration.GetCubicleName(0, 1, 2), CubeConfiguration.GetCubicleName(1, 2, 2), CubeConfiguration.GetCubicleName(2, 1, 2), CubeConfiguration.GetCubicleName(1, 0, 2) }, new string[] { "F" } }, Axis = Axis.Z, RotationAngle = -PiOver2 });
            _basicOps.Add("FM", new BasicOp { Op = "FM", CubicleGroupCycles = new string[][] { new string[] { }, new string[] { CubeConfiguration.GetCubicleName(0, 0, 1), CubeConfiguration.GetCubicleName(0, 2, 1), CubeConfiguration.GetCubicleName(2, 2, 1), CubeConfiguration.GetCubicleName(2, 0, 1) }, new string[] { "U", "R", "D", "L" } }, Axis = Axis.Z, RotationAngle = -PiOver2 });

            _basicOps.Add("L", new BasicOp { Op = "L", CubicleGroupCycles = new string[][] { new string[] { CubeConfiguration.GetCubicleName(0, 0, 0), CubeConfiguration.GetCubicleName(0, 2, 0), CubeConfiguration.GetCubicleName(0, 2, 2), CubeConfiguration.GetCubicleName(0, 0, 2) }, new string[] { CubeConfiguration.GetCubicleName(0, 0, 1), CubeConfiguration.GetCubicleName(0, 1, 0), CubeConfiguration.GetCubicleName(0, 2, 1), CubeConfiguration.GetCubicleName(0, 1, 2) }, new string[] { "L" } }, Axis = Axis.X, RotationAngle = PiOver2 });
            _basicOps.Add("R", new BasicOp { Op = "R", CubicleGroupCycles = new string[][] { new string[] { CubeConfiguration.GetCubicleName(2, 0, 2), CubeConfiguration.GetCubicleName(2, 2, 2), CubeConfiguration.GetCubicleName(2, 2, 0), CubeConfiguration.GetCubicleName(2, 0, 0) }, new string[] { CubeConfiguration.GetCubicleName(2, 1, 2), CubeConfiguration.GetCubicleName(2, 2, 1), CubeConfiguration.GetCubicleName(2, 1, 0), CubeConfiguration.GetCubicleName(2, 0, 1) }, new string[] { "R" } }, Axis = Axis.X, RotationAngle = -PiOver2 });
            _basicOps.Add("LM", new BasicOp { Op = "LM", CubicleGroupCycles = new string[][] { new string[] { }, new string[] { CubeConfiguration.GetCubicleName(1, 0, 0), CubeConfiguration.GetCubicleName(1, 2, 0), CubeConfiguration.GetCubicleName(1, 2, 2), CubeConfiguration.GetCubicleName(1, 0, 2) }, new string[] { "U", "F", "D", "B" } }, Axis = Axis.X, RotationAngle = PiOver2 });
             */
        }


        public static readonly string UndoOp = "X";
        public static readonly string FoldOp = "FOLD";
        public static readonly string UnFoldOp = "UNFOLD";
        private static OpFromBasic[] TotalOps = new OpFromBasic[] { 
            new OpFromBasic(){ Op = "U", BasicOp = "U", RotationDirectionWithAxis = RotationDirection.Clockwise},
            new OpFromBasic(){ Op = "U'", BasicOp = "U", RotationDirectionWithAxis = RotationDirection.CounterClockWise},
            new OpFromBasic(){ Op = "D", BasicOp = "D", RotationDirectionWithAxis = RotationDirection.CounterClockWise},
            new OpFromBasic(){ Op = "D'", BasicOp = "D", RotationDirectionWithAxis = RotationDirection.Clockwise},
            new OpFromBasic(){ Op = "F", BasicOp = "F", RotationDirectionWithAxis = RotationDirection.Clockwise},
            new OpFromBasic(){ Op = "F'", BasicOp = "F", RotationDirectionWithAxis = RotationDirection.CounterClockWise},
            new OpFromBasic(){ Op = "B", BasicOp = "B", RotationDirectionWithAxis = RotationDirection.CounterClockWise},
            new OpFromBasic(){ Op = "B'", BasicOp = "B", RotationDirectionWithAxis = RotationDirection.Clockwise},
            new OpFromBasic(){ Op = "L", BasicOp = "L", RotationDirectionWithAxis = RotationDirection.CounterClockWise},
            new OpFromBasic(){ Op = "L'", BasicOp = "L", RotationDirectionWithAxis = RotationDirection.Clockwise},
            new OpFromBasic(){ Op = "R", BasicOp = "R", RotationDirectionWithAxis = RotationDirection.Clockwise},
            new OpFromBasic(){ Op = "R'", BasicOp = "R", RotationDirectionWithAxis = RotationDirection.CounterClockWise},

            new OpFromBasic(){ Op = "UM", BasicOp = "UM", RotationDirectionWithAxis = RotationDirection.Clockwise},
            new OpFromBasic(){ Op = "UM'", BasicOp = "UM", RotationDirectionWithAxis = RotationDirection.CounterClockWise},
            new OpFromBasic(){ Op = "FM", BasicOp = "FM", RotationDirectionWithAxis = RotationDirection.Clockwise},
            new OpFromBasic(){ Op = "FM'", BasicOp = "FM", RotationDirectionWithAxis = RotationDirection.CounterClockWise},
            new OpFromBasic(){ Op = "LM", BasicOp = "LM", RotationDirectionWithAxis = RotationDirection.CounterClockWise},
            new OpFromBasic(){ Op = "LM'", BasicOp = "LM", RotationDirectionWithAxis = RotationDirection.Clockwise},
            new OpFromBasic(){ Op = "RM", BasicOp = "LM", RotationDirectionWithAxis = RotationDirection.Clockwise},
            new OpFromBasic(){ Op = "RM'", BasicOp = "LM", RotationDirectionWithAxis = RotationDirection.CounterClockWise},
            new OpFromBasic(){ Op = "BM", BasicOp = "FM", RotationDirectionWithAxis = RotationDirection.CounterClockWise},
            new OpFromBasic(){ Op = "BM'", BasicOp = "FM", RotationDirectionWithAxis = RotationDirection.Clockwise},
            new OpFromBasic(){ Op = "DM", BasicOp = "UM", RotationDirectionWithAxis = RotationDirection.CounterClockWise},
            new OpFromBasic(){ Op = "DM'", BasicOp = "UM", RotationDirectionWithAxis = RotationDirection.Clockwise}
        };

        public static string[] GetRandomOps(int numOp)
        {
            Random random = new Random();
            string[] ops = new string[numOp];
            
            for (int i = 0; i < numOp; i++)
            {
                int index = random.Next(0, TotalOps.Length - 1);
                ops[i] = TotalOps[index].Op;
                
            }
            return ops;
        }

        private static Dictionary<string, string> _equivalentOp;
        private static Dictionary<string, BasicOp> _basicOps;
        public static Dictionary<string, BasicOp> BasicOps
        {
            get { return _basicOps; }
        }

        public static BasicOp GetBasicOp(string cubicleName, Axis axis)
        {
            IEnumerable<BasicOp> ops = _basicOps.Values.Where(op => op.Axis == axis);
            BasicOp basicOp = ops.FirstOrDefault(op => cubicleName.Contains(op.Op));
            if (basicOp == null)
                basicOp = ops.FirstOrDefault(op => op.Op.Contains("M"));
            return basicOp;
        }

        public static void GetBasicOp(string op, out BasicOp basicOp, out bool isReverse)
        {
            basicOp = null;
            if (_equivalentOp.ContainsKey(op)) op = _equivalentOp[op];
            isReverse = IsReverse(op);// (op.Substring(op.Length - 1) == "'");
            string basicOpKey = isReverse ? op.Substring(0, op.Length - 1) : op;
            basicOp = _basicOps[basicOpKey];
        }

        public static bool IsValidOp(string op)
        {
            return (TotalOps.FirstOrDefault(o => o.Op == op) != null);
        }

        public static string GetOp(ref string commandSeq,  bool fromTail, bool checkOnly)
        {
            string action = string.Empty;
            string seq = commandSeq;
            if (string.IsNullOrEmpty(seq)) return action;
            int len = 1;
            int totalLen = seq.Length;
            while (len <= totalLen)
            {
                string check = seq.Substring(fromTail ? totalLen - len : 0, len);
                if (IsValidOp(check)||(fromTail&&(check=="'"||check=="M"||check=="M'"))||(!fromTail&&check=="X"))
                {
                    action = check;
                    len++;
                }
                else break;
            }
            if (len == 1) len++;
            len--;
            int startIndex = fromTail ? 0 : len;
            seq = seq.Substring(startIndex, totalLen - len);
            if (!checkOnly) {
                commandSeq = seq;
            }
            return action;
        }

        public static bool IsReverse(string op)
        {
            return (op.Substring(op.Length - 1) == "'");
        }

        public static bool IsReverse(string op1, string op2)
        {
            return op1.Length >= 1 && op2.Length >= 1 && (op1 + "'" == op2 || op2 + "'" == op1);
        }

        public static string GetReverseOp(string op)
        {
            return IsReverse(op) ? op.Substring(0, op.Length - 1) : op + "'";
        }


        public static string GetOp(string basicOp, RotationDirection dir)
        {
            string op = string.Empty;
            OpFromBasic opb = TotalOps.FirstOrDefault(o => o.BasicOp == basicOp && o.RotationDirectionWithAxis == dir);
            if (opb != null)
            {
                op = opb.Op;
            }
            return op;
        }
        /*
        public static string GetOp(string basicOp, Point3 from, Point3 to)
        {
            Axis axis = _basicOps[basicOp].Axis;
            Point2 fromV2; //= Vector2.Zero;
            Point2 toV2; //= Vector2.Zero;
            switch (axis)
            {
                case Axis.X: //R
                    fromV2 = new Point2(from.Y, from.Z);
                    toV2 = new Point2(to.Y, to.Z);
                    break;
                case Axis.Y: //U
                    fromV2 = new Point2(from.Z, from.X);
                    toV2 = new Point2(to.Z, to.X);
                    break;
                case Axis.Z: //F
                    fromV2 = new Point2(from.X, from.Y);
                    toV2 = new Point2(to.X, to.Y);
                    break;
                default:
                    fromV2 = new Point2();
                    toV2 = new Point2();
                    break;
            }
            float angle = (float)System.Math.Atan2(toV2.Y, toV2.X) - (float)System.Math.Atan2(fromV2.Y, fromV2.X);
            bool isReverse = (angle > 0);

            if (basicOp.IndexOf("U") >= 0 || basicOp.IndexOf("R") >= 0 || basicOp.IndexOf("F") >= 0)
            {
                isReverse = (angle > 0);
            }
            else
            {
                isReverse = (angle < 0);
            }

            if (isReverse) basicOp = basicOp + "'";
            return basicOp;
        }
         */
    }
}
