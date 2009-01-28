#region Description
//-----------------------------------------------------------------------------
// File:        Transform.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace RubiksCube
{
    class Transform
    {
        public string Op { get; set; }
        public BasicOp BasicOp { get; set; }
        public bool IsReversedBasicOp { get; set; }
        public IEnumerable<Cubicle> AffectedCubicles { get; set; }
        public bool Silent { get; set; }
    }
}
