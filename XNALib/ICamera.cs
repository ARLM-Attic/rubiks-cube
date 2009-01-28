#region Description
//-----------------------------------------------------------------------------
// File:        ICamera.cs
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

namespace XNALib
{
    public interface ICamera
    {
        void Rotate(float pan, float tilt, float roll);
        void Translate(float right, float up, float forward);
        Matrix View { get; }
        Matrix Projection { get; }
    }
}
