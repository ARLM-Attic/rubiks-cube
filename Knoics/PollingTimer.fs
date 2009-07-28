//-----------------------------------------------------------------------------
// File:        PollingTimer.fs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        07/23/2009
//-----------------------------------------------------------------------------
namespace Knoics
open System

type PollingTimer(inter:int) = 
    let mutable elapsedTime = 0
    let mutable lastTime = DateTime.Now
    member x.Interval:int = inter
    member x.OnInterval() = 
        let now = DateTime.Now
        let elapsed = now - lastTime
        lastTime <- now
        elapsedTime <- elapsedTime + elapsed.Milliseconds;
        let onInterval = elapsedTime >= x.Interval
        if onInterval then 
            elapsedTime <- 0
        onInterval
    member x.Reset() = 
        elapsedTime <- 0
        ()