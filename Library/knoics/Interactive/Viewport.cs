//-----------------------------------------------------------------------------
// File:        Viewport.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        02/21/2009
//-----------------------------------------------------------------------------
using System;
using System.Net;
using Knoics.Math;

namespace Knoics.Interactive
{
    public class Viewport
    {

        public static Ray Unproject(Point2 pt, Point3 viewpoint, Matrix worldToLocal, Matrix viewToWorld, Matrix screenToViewTransform)
        {
            //Matrix screenToLocal = worldToLocal * (viewToWorld * screenToViewTransform);
            Vector3 vs = new Vector3(pt.X, pt.Y, 1);
            Vector3 view = Vector3.Transform(vs, screenToViewTransform);
            Vector3 world = Vector3.Transform(view, viewToWorld);
            Vector3 to = Vector3.Transform(world, worldToLocal);

            //Matrix worldToLocal = Matrix.Invert(world);
            Vector3 from = Vector3.Transform((Vector3)viewpoint, worldToLocal);
            
            return new Ray((Point3)from, to - from);
        }

    }
}
