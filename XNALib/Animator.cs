#region Description
//-----------------------------------------------------------------------------
// File:        Animator.cs
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
    public class AnimContext
    {
        public float Duration { get; set; } // in ms
        public float Change { get; set; }
        public object Transform {get;set;}
        public string Op { get; set; }
        public bool Silent { get; set; }
    }

    public class Animator : DrawableGameComponent
    {
        private AnimContext _animContext;

        private float Begin { get; set; }
        private float Delta { get; set; }
        public Animator(Game game)
            : base(game)
        {
            InAnimation = false;
            _animQueue = new Queue<AnimContext>();
        }

        private Func<object, float, bool> _intervalUpdator;
        public Func<object, float, bool> IntervalUpdator
        {
            set { _intervalUpdator = value; }
        }

        private Func<AnimContext, bool> _beginUpdator;
        public Func<AnimContext, bool> BeginUpdator
        {
            set { _beginUpdator = value; }
        }


        private Func<object, float, bool> _endUpdator;
        public Func<object, float, bool> EndUpdator
        {
            set { _endUpdator = value; }
        }

        private Queue<AnimContext> _animQueue;
        public bool InAnimation { get; set; }
        public bool ReadyInQueue
        {
            get
            {
                return (!InAnimation && _animQueue.Count > 0);
            }
        }

        
        public void Clear()
        {
            _animContext = null;
            _animQueue.Clear();

        }
        public void Start(AnimContext animContext)
        {
            if(animContext!=null)
                _animQueue.Enqueue(animContext);
            if (InAnimation)
            {
                return;
            }

            if (_animQueue.Count > 0)
            {
                animContext = _animQueue.Dequeue();
                if (_beginUpdator != null)
                    _beginUpdator(animContext);

                _animContext = animContext;
                float frames = animContext.Duration / 1000f * (float)frameRate;
                //Debug.WriteLine(string.Format("frames: {0}", frames));
                Delta = animContext.Change / frames;
                InAnimation = true;
                Begin = 0f;
            }
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
            }

            if (InAnimation&&_intervalUpdator != null)
            {
                float delta = Delta;
                if (_animContext.Change > 0 && Begin + Delta > _animContext.Change)
                {
                    delta = _animContext.Change - Begin;
                    InAnimation = false;
                }
                else if (_animContext.Change < 0 && Begin + Delta < _animContext.Change)
                {
                    delta = _animContext.Change - Begin;
                    InAnimation = false;
                }

                _intervalUpdator(_animContext.Transform, delta);
                Begin += delta;
                if (!InAnimation)
                {
                    Begin = 0f;
                    if (_endUpdator != null)
                        _endUpdator(_animContext.Transform, _animContext.Change);
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            frameCounter++;
            base.Draw(gameTime);
        }
    }
}
