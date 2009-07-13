#region Description
//-----------------------------------------------------------------------------
// File:        IMesh.cs
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
    public interface IMesh : ITransform
    {
        CubieFace Face { get; }
    }

}
