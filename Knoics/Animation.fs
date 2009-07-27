//-----------------------------------------------------------------------------
// File:        Animation.fs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        07/23/2009
//-----------------------------------------------------------------------------

namespace Knoics.RubiksCube
open System.Collections.Generic
open System


type AnimContext(rotatedAngle:double, frames:double, op:string, silent:bool, transformParams:List<Transform>) =
    let mutable _op:string = op
    member x.RotatedAngle = rotatedAngle
    member x.Frames = frames
    member x.Op with get() = _op and set v = _op <- v
    member x.Silent = silent
    member x.TransformParams  = transformParams
    

type Animator (beforeTransform:AnimContext -> bool , afterTransform:string -> unit ) =
    let _afterTransform = afterTransform
    let _beforeTransform = beforeTransform
    let _animQueue:Queue<AnimContext> = new Queue<AnimContext>()
    let mutable _inAnimation:bool = false
    let mutable _animContext:AnimContext option = None
    
    member x.InAnimation with get() = _inAnimation

    member x.ReadyInQueue with get() = _inAnimation=false && _animQueue.Count > 0

    member x.NoOp with get() = _inAnimation=false && _animQueue.Count = 0 && _animContext.IsNone
    
    member x.Start(anim:AnimContext option) =
        if(Option.isSome(anim)) then
            _animQueue.Enqueue(Option.get(anim))
        if (_inAnimation=false && x.ReadyInQueue&&_animQueue.Count > 0)then
            let animContext = _animQueue.Dequeue()
            if _beforeTransform(animContext) then
                _inAnimation <- true;
                for transformParam in animContext.TransformParams do
                    transformParam.BeforeTransform()
                    _animContext <- Some(animContext)
                    transformParam.Delta <- transformParam.ChangeAngle / animContext.Frames
                    transformParam.Begin <- 0.0

    member x.Update() =
        if (_inAnimation&&_animContext.IsSome) then
            let mutable inAnimation = true;
            let animContext:AnimContext = _animContext.Value
            for transformParam in animContext.TransformParams do
                let mutable delta = transformParam.Delta
                inAnimation <- true
                if ((transformParam.ChangeAngle > 0. && transformParam.Begin + transformParam.Delta > transformParam.ChangeAngle)||
                    (transformParam.ChangeAngle < 0. && transformParam.Begin + transformParam.Delta < transformParam.ChangeAngle))then
                    delta <- transformParam.ChangeAngle - transformParam.Begin
                    inAnimation <- false

                transformParam.DoTransform(delta)
                transformParam.Begin <- transformParam.Begin + delta
                if (inAnimation=false) then
                    transformParam.Begin <- 0.
                    transformParam.AfterTransform(transformParam.ChangeAngle)
            
            if (inAnimation = false) then
                if (animContext.Silent=false) then
                    _afterTransform(animContext.Op)
                _inAnimation <- inAnimation
                _animContext <- None
    