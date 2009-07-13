#region Description
//-----------------------------------------------------------------------------
// File:        CubeConfiguration.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        02/21/2009
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections.Generic;
using Knoics.Math;

using System.Windows.Media;

namespace Knoics.RubiksCube
{
    public class CubeConfiguration
    {
        public static IFactory Factory { get; set; }
        internal static Dictionary<string, FaceConfig> Faces = new Dictionary<string, FaceConfig>();

        static CubeConfiguration()
        {
            Faces.Add("U", new FaceConfig() { Color = Colors.Red, Name = "U", Normal = Axis.Y });
            Faces.Add("D", new FaceConfig() { Color = Colors.Orange, Name = "D", Normal = Axis.Y });
            Faces.Add("L", new FaceConfig() { Color = Colors.Green, Name = "L", Normal = Axis.X });
            Faces.Add("R", new FaceConfig() { Color = Colors.Blue, Name = "R", Normal = Axis.X });
            Faces.Add("B", new FaceConfig() { Color = Colors.White, Name = "B", Normal = Axis.Z });
            Faces.Add("F", new FaceConfig() { Color = Colors.Yellow, Name = "F", Normal = Axis.Z });
        }

        //x--0:Left     Width-1 :Right
        //y--0:Down     Height-1:Up
        //z--0:Back     Depth-1 :Front
        //To make notation closewise, use pattern YXZ
        public static string GetCubicleName(CubeSize size, int x, int y, int z)
        {
            //StringBuilder sb = new StringBuilder();
            bool xface = true;
            bool yface = true;
            bool zface = true;
            bool postive = true;
            string yFace = string.Empty;
            string xFace = string.Empty;
            string zFace = string.Empty;
            if (y == 0)
            {
                yFace = "D";
                //sb.Append("D");
                postive = !postive;
            }
            else if (y == size.Height - 1)
                //sb.Append("U");
                yFace = "U";
            else if (size.Height > 3)
            {
                yface = false;
                //sb.Append(y);
                yFace = "D" + y.ToString();
            }


            if (x == 0)
            {
                //sb.Append("L");
                xFace = "L";
                postive = !postive;
            }
            else if (x == size.Width - 1)
                //sb.Append("R");
                xFace = "R";
            else if (size.Width > 3)
            {
                xface = false;
                //sb.Append(x);
                xFace =  "L" + x.ToString();
            }


            if (z == 0)
            //sb.Append("B");
            {
                zFace = "B";
                postive = !postive;
            }
            else if (z == size.Depth - 1)
                //sb.Append("F");
                zFace = "F";
            else if (size.Depth > 3)
            {
                zface = false;
                //sb.Append("B");
                //sb.Append(z);
                zFace = "B" + z.ToString();
            }
            string name = string.Empty;

            if (!xface && !yface && !zface)
                name = string.Empty;
            else
            {
                if (!string.IsNullOrEmpty(xFace) && !string.IsNullOrEmpty(yFace) && !string.IsNullOrEmpty(zFace))
                    name = postive ? yFace + xFace + zFace : yFace + zFace + xFace;
                else
                    name = yFace + xFace + zFace;
            }
            return name;
        }

    }
}
