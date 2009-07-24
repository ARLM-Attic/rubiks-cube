//-----------------------------------------------------------------------------
// File:        PollingTimer.fs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        07/23/2009
//-----------------------------------------------------------------------------
namespace Knoics
open System

type PollingTimer(inter:int) as this = 
    [<DefaultValue>] val mutable elapsedTime:int
    [<DefaultValue>] val mutable lastTime:DateTime 
    do this.lastTime <- DateTime.Now
    member x.Interval:int = inter
    member x.OnInterval() = 
        let now = DateTime.Now
        let elapsed = now - this.lastTime
        this.lastTime <- now
        this.elapsedTime <- this.elapsedTime + elapsed.Milliseconds;
        let onInterval = this.elapsedTime >= this.Interval
        if onInterval then 
            x.elapsedTime <- 0
        onInterval
    
    member x.Reset() = 
        x.elapsedTime <- 0
        ()