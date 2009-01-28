#region Description
//-----------------------------------------------------------------------------
// File:        Timer.cs
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
using System.Diagnostics;

namespace XNALib
{
    public class Timer : DrawableGameComponent
    {
        public Timer(Game game)
            : base(game)
        {
        }

        private Func<GameTime, bool> _onTimeUpdator;
        public Func<GameTime, bool> OnTime
        {
            set { _onTimeUpdator = value; }
        }


        private int frameCounter = 0;
        private float timer = 0f;
        private float interval = 1000.0f;
        private int frameRate = 0;
        public override void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                timer = 0f;
                frameRate = frameCounter;
                frameCounter = 0;
                if (_onTimeUpdator!=null)
                {
                    _onTimeUpdator(gameTime);
                }
            }


            base.Update(gameTime);
        }

        public void Reset()
        {
            timer = 0f;
        }

        public override void Draw(GameTime gameTime)
        {
            frameCounter++;
            base.Draw(gameTime);
        }
    }
}
