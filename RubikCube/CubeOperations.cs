#region Description
//-----------------------------------------------------------------------------
// File:        CubeOperation.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace RubiksCube
{
    class BasicOp
    {
        public string Op { get; set; }
        public Axis Axis { get; set; }
        public float Rotation { get; set; }
        public string[][] CubicleGroupCycles { get; set; }


        public IEnumerable<string> GetCubicles()
        {
            string[][] cubiclesArray = CubicleGroupCycles;
            foreach (string[] cubicles in cubiclesArray)
            {
                foreach (string cubicle in cubicles)
                {
                    yield return cubicle;
                }
            }
        }

    }

    class CubeOperations
    {
        private static string[] TotalOps = new string[] { 
            "U",
            "U'",
            "D",
            "D'",
            "F",
            "F'",
            "B",
            "B'",
            "L",
            "L'",
            "R",
            "R'",

            "UM",
            "UM'",
            "FM",
            "FM'",
            "LM",
            "LM'",
            "RM",
            "RM'",
            "BM",
            "BM'",
            "DM",
            "DM'"
        };

        public static string[] GetRandomOps(int numOp)
        {
            Random random = new Random();
            string[] ops = new string[numOp];
            
            for (int i = 0; i < numOp; i++)
            {
                int index = random.Next(0, TotalOps.Length - 1);
                //Debug.WriteLine(index.ToString());
                ops[i] = TotalOps[index];
                
            }
            return ops;
        }

        private static Dictionary<string, string> _equivalentOp;
        private static Dictionary<string, BasicOp> _basicOps;
        public static Dictionary<string, BasicOp> BasicOps
        {
            get { return _basicOps; }
        }


        static CubeOperations()
        {
            _equivalentOp = new Dictionary<string, string>();
            _equivalentOp.Add("RM", "LM'");
            _equivalentOp.Add("RM'", "LM");
            _equivalentOp.Add("BM", "FM'");
            _equivalentOp.Add("BM'", "FM");
            _equivalentOp.Add("DM", "UM'");
            _equivalentOp.Add("DM'", "UM");

            _basicOps = new Dictionary<string, BasicOp>(); //corner cubie rotation, edge cubie rotation, center cubie
            _basicOps.Add("U", new BasicOp { Op = "U", CubicleGroupCycles = new string[][] { new string[] { "ULB", "UBR", "URF", "UFL" }, new string[] { "UB", "UR", "UF", "UL" }, new string[] { "U" } }, Axis = Axis.Y, Rotation = -MathHelper.PiOver2 });
            _basicOps.Add("D", new BasicOp { Op = "D", CubicleGroupCycles = new string[][] { new string[] { "DLF", "DFR", "DRB", "DBL" }, new string[] { "DF", "DR", "DB", "DL" }, new string[] { "D" } }, Axis = Axis.Y, Rotation = MathHelper.PiOver2 });
            _basicOps.Add("F", new BasicOp { Op = "F", CubicleGroupCycles = new string[][] { new string[] { "UFL", "URF", "DFR", "DLF" }, new string[] { "UF", "RF", "DF", "LF" }, new string[] { "F" } }, Axis = Axis.Z, Rotation = -MathHelper.PiOver2 });
            _basicOps.Add("B", new BasicOp { Op = "B", CubicleGroupCycles = new string[][] { new string[] { "UBR", "ULB", "DBL", "DRB" }, new string[] { "RB", "UB", "LB", "DB" }, new string[] { "B" } }, Axis = Axis.Z, Rotation = MathHelper.PiOver2 });
            _basicOps.Add("L", new BasicOp { Op = "L", CubicleGroupCycles = new string[][] { new string[] { "ULB", "UFL", "DLF", "DBL" }, new string[] { "UL", "LF", "DL", "LB" }, new string[] { "L" } }, Axis = Axis.X, Rotation = MathHelper.PiOver2 });
            _basicOps.Add("R", new BasicOp { Op = "R", CubicleGroupCycles = new string[][] { new string[] { "URF", "UBR", "DRB", "DFR" }, new string[] { "UR", "RB", "DR", "RF" }, new string[] { "R" } }, Axis = Axis.X, Rotation = -MathHelper.PiOver2 });
            _basicOps.Add("UM", new BasicOp { Op = "UM", CubicleGroupCycles = new string[][] { new string[] { }, new string[] { "LB", "RB", "RF", "LF" }, new string[] { "B", "R", "F", "L" } }, Axis = Axis.Y, Rotation = -MathHelper.PiOver2 });
            _basicOps.Add("FM", new BasicOp { Op = "FM", CubicleGroupCycles = new string[][] { new string[] { }, new string[] { "UL", "UR", "DR", "DL" }, new string[] { "U", "R", "D", "L" } }, Axis = Axis.Z, Rotation = -MathHelper.PiOver2 });
            _basicOps.Add("LM", new BasicOp { Op = "LM", CubicleGroupCycles = new string[][] { new string[] { }, new string[] { "UB", "UF", "DF", "DB" }, new string[] { "U", "F", "D", "B" } }, Axis = Axis.X, Rotation = MathHelper.PiOver2 });

        }


        public static void GetBasicOp(string op, out BasicOp basicOp, out bool isReverse)
        {
            if (_equivalentOp.ContainsKey(op)) op = _equivalentOp[op];
            isReverse = (op.Substring(op.Length - 1) == "'");
            string basicOpKey = isReverse ? op.Substring(0, op.Length - 1) : op;
            basicOp = _basicOps[basicOpKey];
        }

        public static bool IsValidOp(string action)
        {
            return !string.IsNullOrEmpty(TotalOps.FirstOrDefault(op => op == action));
        }

        public static string GetOp(ref string commandLines)
        {
            string action = string.Empty;
            int len = 1;
            while (len <= commandLines.Length)
            {
                string check = commandLines.Substring(0, len);
                if (!IsValidOp(check))
                {
                    break;
                }
                action = check;
                len++;
            }
            if (len == 1) len++;
            commandLines = commandLines.Substring(len - 1);
            return action;
        }

        public static string GetOp(string basicOp, Vector3 from, Vector3 to)
        {
            Axis axis = _basicOps[basicOp].Axis;
            Vector2 fromV2 = Vector2.Zero;
            Vector2 toV2 = Vector2.Zero;
            switch (axis)
            {
                case Axis.X: //R
                    fromV2 = new Vector2(from.Y, from.Z);
                    toV2 = new Vector2(to.Y, to.Z);
                    break;
                case Axis.Y: //U
                    fromV2 = new Vector2(from.Z, from.X);
                    toV2 = new Vector2(to.Z, to.X);
                    break;
                case Axis.Z: //F
                    fromV2 = new Vector2(from.X, from.Y);
                    toV2 = new Vector2(to.X, to.Y);
                    break;
            }
            float angle = (float)Math.Atan2(toV2.Y, toV2.X) - (float)Math.Atan2(fromV2.Y, fromV2.X);
            bool isReverse = (angle > 0);

            Vector3 normal = Vector3.Zero;
            /*
            if (basicOp.IndexOf("L") >= 0)
            {
                normal = Vector3.UnitX;
            }
            else if (basicOp.IndexOf("D") >= 0)
            {
                normal = Vector3.UnitY;
            }
            else if (basicOp.IndexOf("B") >= 0)
                normal = Vector3.UnitZ;

            if (normal != Vector3.Zero && Vector3.Dot(normal, from) >=0 )
            {
                isReverse = !isReverse;
            }*/
            
            if (basicOp.IndexOf("U") >= 0 || basicOp.IndexOf("R") >= 0 || basicOp.IndexOf("F") >= 0)
            {
                isReverse = (angle > 0);
            }
            else
            {
                isReverse = (angle < 0);
            }

            //Debug.WriteLine(string.Format("from: {0} to: {1} angle: {2}, reverse: {3} basicOP: {4}", fromV2, toV2, angle, isReverse, basicOp));
            if (isReverse) basicOp = basicOp + "'";
            return basicOp;
        }
    }
}
