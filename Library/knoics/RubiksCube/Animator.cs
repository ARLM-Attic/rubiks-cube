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
using System.Diagnostics;

namespace Knoics.RubiksCube
{

    public delegate void PostOp(string op);
    public delegate bool PreOp(AnimContext context);
    public class Animator
    {
        private AnimContext _animContext;
        private PostOp _afterTransform;
        private PreOp _beforeTransform;
        public Animator(PreOp beforeTransform, PostOp afterTransform)
        {
            //Interval = interval;
            _afterTransform = afterTransform;
            _beforeTransform = beforeTransform;
            _inAnimation = false;
            _animQueue = new Queue<AnimContext>();
        }


        private Func<Transform, string,  bool> _endUpdator;
        public Func<Transform, string, bool> EndUpdator
        {
            set { _endUpdator = value; }
        }

        private Queue<AnimContext> _animQueue;
        private bool _inAnimation;
        public bool InAnimation { get { return _inAnimation; } }

        public bool ReadyInQueue
        {
            get
            {
                return (!InAnimation && _animQueue.Count > 0);
            }
        }

        public bool NoOp
        {
            get
            {
                return !InAnimation && _animQueue.Count == 0 && _animContext == null;
            }
        }
        
        public void Start(AnimContext animContext)
        {

            if(animContext!=null)
                _animQueue.Enqueue(animContext);
            if (_inAnimation||!ReadyInQueue)
            {
                return;
            }

            if (_animQueue.Count > 0)
            {
                animContext = _animQueue.Dequeue();
                if (_beforeTransform != null)
                {
                    if (!_beforeTransform(animContext)) return;
                }
                _inAnimation = true;
                foreach (Transform transformParam in animContext.TransformParams)
                {
                    transformParam.BeforeTransform();

                    _animContext = animContext;
                    double intervalCounts = animContext.Frames;// animContext.Duration / _interval;
                    transformParam.Delta = transformParam.ChangeAngle / intervalCounts;

                    transformParam.Begin = 0;
                }
            }
        }


        public void Update()
        {
            if (_inAnimation)
            {
                bool inAnimation = true;
                foreach (Transform transformParam in _animContext.TransformParams)
                {
                    double delta = transformParam.Delta;
                    inAnimation = true;
                    if (transformParam.ChangeAngle > 0 && transformParam.Begin + transformParam.Delta > transformParam.ChangeAngle)
                    {
                        delta = transformParam.ChangeAngle - transformParam.Begin;
                        inAnimation = false;
                    }
                    else if (transformParam.ChangeAngle < 0 && transformParam.Begin + transformParam.Delta < transformParam.ChangeAngle)
                    {
                        delta = transformParam.ChangeAngle - transformParam.Begin;
                        inAnimation = false;
                    }

                    transformParam.DoTransform(delta);
                    transformParam.Begin += delta;
                    if (!inAnimation)
                    {
                        transformParam.Begin = 0;
                        transformParam.AfterTransform(transformParam.ChangeAngle);
                    }
                }

                if (!inAnimation)
                {
                    if (_afterTransform != null && !_animContext.Silent)
                    _afterTransform(_animContext.Op);
                    _inAnimation = inAnimation;
                    _animContext = null;
                }
            }
        }
    }
}
