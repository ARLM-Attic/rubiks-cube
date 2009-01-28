#region Description
//-----------------------------------------------------------------------------
// File:        Fps.cs
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
    public class Fps : DrawableGameComponent
    {
        ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        int frameRate = 0;

        public bool Show
        {
            get;
            set;
        }
        public int FrameRate
        {
            get { return frameRate; }
        }
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;


        public Fps(Game game, bool show)
            : base(game)
        {
            content = new ContentManager(game.Services);
            Show = show;
        }


        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                spriteBatch = new SpriteBatch(GraphicsDevice);
                spriteFont = content.Load<SpriteFont>("Content/Fonts/GameFont");
            }
        }


        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent)
                content.Unload();
        }


        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            frameCounter++;
            if (Show)
            {
                string fps = string.Format("fps: {0}", frameRate);
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

                spriteBatch.DrawString(spriteFont, fps, new Vector2(33, 33), Color.Black);
                spriteBatch.DrawString(spriteFont, fps, new Vector2(32, 32), Color.White);

                spriteBatch.End();
            }
        }
    }
}
    
