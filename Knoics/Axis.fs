namespace Knoics.Math

open System

[<Flags>]
type Axis = 
    | X = 0x0001
    | Y = 0x0002
    | Z = 0x0004
    
    
    
type Angle = 
    static member RadiansToDegrees(radians:double) =
        radians * 57.295779513082323

    static member DegreesToRadians(degrees:double) = 
        degrees / 57.295779513082323
    