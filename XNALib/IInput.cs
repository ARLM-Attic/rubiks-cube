#region Description
//-----------------------------------------------------------------------------
// File:        IInput.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNALib
{
    public enum InputMode
    {
        NoInput,
        Select,
        Drag
    }

    public interface IInput
    {
        InputMode Update(Game game, Matrix projection, Matrix view);
        void Reset();
        float Pan { get; set; }
        float Tilt { get; set; }
        float Roll { get; set; }

        float Right { get; set; }
        float Up { get; set; }
        float Forward { get; set; }

        Vector2 Selected { get; }
        Vector2 DragFrom { get; }
        Vector2 DragTo { get; }

    }
}
