//-----------------------------------------------------------------------------
// File:        BasicOp.fs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        07/25/2009
//-----------------------------------------------------------------------------
namespace Knoics.RubiksCube

open System
open Knoics.Math
open System.Collections.Generic

type CubicleOrientation(name:string, orientationName:string) =
    member x.Name = name
    member x.OrientationName = orientationName

type OpFromBasic(op:string, basicOp:string, rotationDirectionWithAxis:RotationDirection) = 
    member x.Op = op
    member x.BasicOp = basicOp
    member x.RotationDirectionWithAxis  = rotationDirectionWithAxis
    
//cubicleOrientationCycles, cycles for corner cubicles, edge cubicles and center cubicles
type BasicOp(op:string, cubicleOrientationCycles:string array array, axis:Axis, angle:double) =
    let GetNameFromOrientedName(orientationName:string) = 
        let name = orientationName;
        let pos = orientationName.IndexOfAny([|'U';'D'|])//(new char[] { 'U', 'D' })
        if (pos >= 0) then
            orientationName.Substring(pos, orientationName.Length - pos) + orientationName.Substring(0, pos)
        else 
            let pos1 = orientationName.IndexOfAny([|'L'; 'R'|])
            if (pos1 > 0) then
                orientationName.Substring(pos1, orientationName.Length - pos1) + orientationName.Substring(0, pos1)
            else
                orientationName

    let _cubicleGroupCycles = cubicleOrientationCycles |> Array.map (fun arr -> (arr |> Array.map( fun e -> new CubicleOrientation(GetNameFromOrientedName(e), e)))) //new CubicleOrientation[cubicleOrientationCycles.Length][]
    let _cubicleGroup = [for arr in cubicleOrientationCycles do
                            for s in arr -> GetNameFromOrientedName(s)
                         ] 
    member x.Op = op
    member x.Axis = axis
    member x.RotationAngle = angle
    member x.CubicleGroupCycles with get() = _cubicleGroupCycles
    member x.CubicleGroup with get() = _cubicleGroup

