//-----------------------------------------------------------------------------
// File:        Interfaces.fs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        07/25/2009
//-----------------------------------------------------------------------------
namespace Knoics.RubiksCube

open System
open Kit3D.Windows.Media.Media3D
open System.Windows.Media
open Knoics.Math

type ITransform =
    abstract Reset : unit -> unit
    abstract Save : unit->unit
    abstract Restore : unit -> unit
    abstract DoTransform : Matrix3D * bool -> unit
    abstract Transform : Matrix3D
        with get

type IMesh = 
    inherit ITransform
type IModel = 
    inherit ITransform


type IFactory = 
    abstract CreateModel: unit->IModel
    abstract CreateMesh: string * Vector3D[] * Color -> IMesh
    

type RotationDirection =
    | CounterClockWise
    | Clockwise



[<Struct>]
type CubeSize(w:int, h:int, d:int)= 
    member x.Width = w
    member x.Height = h
    member x.Depth = d
    

[<Struct>]
type FaceConfig(color:Color, name:string, normal:Axis) =
    member x.Name = name
    member x.Color = color
    member x.Normal = normal
