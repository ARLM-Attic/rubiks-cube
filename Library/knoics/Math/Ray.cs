#region Description
//-----------------------------------------------------------------------------
// File:        Ray.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        02/21/2009
// Reference:   Microsoft.Xna.Framework and System.Windows.Media.Media3D
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Net;

namespace Knoics.Math
{
    public struct Ray
    {
        private Point3 origin;
        private Vector3 direction;

        public Ray(Point3 origin, Vector3 direction)
        {
            this.origin = origin;

            direction.Normalize();
            this.direction = direction;
        }

        public Point3 Origin
        {
            get { return this.origin; }
        }

        public Vector3 Direction
        {
            get { return this.direction; }
        }

        public override string ToString()
        {

            return string.Format("o: {0}, d: {1}", origin, direction);
        }
    }
}
