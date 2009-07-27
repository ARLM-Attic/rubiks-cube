//-----------------------------------------------------------------------------
// File:        CubeTransform.fs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        07/25/2009
//-----------------------------------------------------------------------------
namespace Knoics.RubiksCube

open Knoics.Math
open Kit3D.Windows.Media.Media3D
open System.Collections.Generic
open System.Text
open System.Linq

[<AbstractClass>]
type Transform() = 
    [<DefaultValue>] val mutable _silent:bool
    [<DefaultValue>] val mutable _changeAngle:double
    [<DefaultValue>] val mutable _begin:double
    [<DefaultValue>] val mutable _delta:double
    
    member x.Silent with get() = x._silent and 
                         set v = x._silent <- v
                         
    //net change in angle, somtimes it already rotated manually by user                         
    member x.ChangeAngle with   get() = x._changeAngle and 
                                set v = x._changeAngle <- v
                         

    //needs to be refactored, Animation use only
    member x.Begin with   get() = x._begin and 
                          set v = x._begin <- v
    
    member x.Delta with   get() = x._delta and 
                          set v = x._delta <- v

    abstract member BeforeTransform : unit -> unit
    abstract member DoTransform: double -> unit
    abstract member AfterTransform : double -> unit


type GetCubicle =  delegate of string->Cubicle
type CubieTransform (op:string, isReversedBasicOp:bool, basicOp:BasicOp, getCubicle:GetCubicle) =
    inherit Transform()
    let _getCubicle = getCubicle
    let _op = op
    let _basicOp:BasicOp = basicOp
    let _isReversedBasicOp = isReversedBasicOp
    
    let mutable _affectedCubicles:IEnumerable<Cubicle> = null
    
    let Cycle(cycle:CubicleOrientation array) =
        if (cycle.Length >= 2) then
            let count = cycle.Length //.Count();
            let mutable fromOrientation = cycle.[count - 1]
            let fromCubicle = _getCubicle.Invoke(fromOrientation.Name)
            let mutable fromCubie = fromCubicle.Cubie.Value
            let mutable fromFaces = fromCubicle.CubieFaces
            //if (fromCubie == null) Debug.Assert(false);
            for i = 0 to  count-1 do
                let toOrientation = cycle.[i]
                let toCubicle = _getCubicle.Invoke(toOrientation.Name)
                fromCubie <- toCubicle.SetCubie(fromCubie).Value
                fromFaces <- toCubicle.SetCubieFaces(fromFaces, fromOrientation.OrientationName, toOrientation.OrientationName)
                fromOrientation <- toOrientation
    
    member x.IsReversedBasicOp = _isReversedBasicOp
    member x.Op = _op
    member x.BasicOp = _basicOp
    member x.AffectedCubicles = _affectedCubicles


    member x.RotateAngle with get() = if _isReversedBasicOp then -_basicOp.RotationAngle else _basicOp.RotationAngle //standard angle


    override x.BeforeTransform() =
        _affectedCubicles <- _basicOp.CubicleGroup.Select(fun c -> _getCubicle.Invoke(c))

    override x.DoTransform(deltaAngle:double) =
        //Debug.WriteLine(string.Format("transform bones: {0}", bones.Count()));
        for cubicle in _affectedCubicles do
            cubicle.Cubie.Value.Rotate(_basicOp.Axis, deltaAngle, false)
        ()

    
    override x.AfterTransform(rotationAngle:double)=
        for cubicle in _affectedCubicles do
            let cubie = cubicle.Cubie
            cubie.Value.RotateUnit(_basicOp.Axis, x.RotateAngle)

        for change in _basicOp.CubicleGroupCycles do
            if (_isReversedBasicOp) then
                let cycle = change |> Array.rev 
                Cycle (cycle)
            else
                //let cycle = change.ToArray()
                Cycle (change)

type GetCubicleFaces = delegate of string -> IEnumerable<CubicleFace>
type FaceTransform(face:string, axis:Axis, isAxisMoving:bool, axisTranslationFromOrigin:Vector3D, axis2TranslationFromOrigin:Vector3D, getCubicleFaces:GetCubicleFaces) =
    inherit Transform()
    
    let mutable _affectedFaces:IEnumerable<CubicleFace> = null
    let _face = face
    let _axisTranslationFromOrigin = axisTranslationFromOrigin
    let _axis2TranslationFromOrigin = axis2TranslationFromOrigin
    let _axis = axis
    let _isAxisMoving = isAxisMoving
    let _getCubicleFaces = getCubicleFaces
    let _axisTranslation = new Dictionary<CubieFace, Vector3D>()
    member x.Face = _face
    member x.AxisTranslationFromOrigin = _axisTranslationFromOrigin
    member x.Axis2TranslationFromOrigin = _axis2TranslationFromOrigin
    
    member x.Axis = _axis
    member x.IsAxisMoving = _isAxisMoving

    override x.BeforeTransform()=
        _affectedFaces <- _getCubicleFaces.Invoke(_face)// GetFaces(Face);

    override x.DoTransform(deltaAngle:double)=
      //  Debug.WriteLine("start DoTransform");
        for face in _affectedFaces do
            let cubie = face.CubieFace.Value.Cubie
            let cubieFace = face.CubieFace.Value
            let mutable axis = face.CubieFace.Value.Cubie.UnitX
            if (_axis = Axis.X)then
                axis <- face.CubieFace.Value.Cubie.UnitX
            else if (_axis = Axis.Y)then
                axis <- face.CubieFace.Value.Cubie.UnitY
            else if (_axis = Axis.Z)then
                axis <- face.CubieFace.Value.Cubie.UnitZ

            let rot =  Quaternion3D.CreateFromAxisAngle(axis, deltaAngle)
            let rotMatrix = Ext3D.CreateFromQuaternion(rot)
            let matrix = cubie.Transform
            matrix.Invert()
            
            
            let mutable rotation = Ext3D.CreateTranslation(Ext3D.Transform(-_axisTranslationFromOrigin, matrix)) * rotMatrix * Ext3D.CreateTranslation(Ext3D.Transform(_axisTranslationFromOrigin, matrix))
            if (_isAxisMoving) then
                let mutable v1 = new Vector3D()
                if (_axisTranslation.ContainsKey(cubieFace)=false) then
                    let m = (cubieFace :> ITransform).Transform
                    m.Invert()
                    v1 <- Ext3D.Transform(_axis2TranslationFromOrigin, m)
                    _axisTranslation.Add(cubieFace, v1)
                v1 <- _axisTranslation.[cubieFace]
                let r = rotation
                r.Invert()
                v1 <- Ext3D.Transform(v1, r)
                
                let rotationAxis = Ext3D.CreateTranslation(-v1) * rotMatrix * Ext3D.CreateTranslation(v1)
                rotation <- rotationAxis * rotation

                _axisTranslation.[cubieFace] <- v1
            (cubieFace :> ITransform).DoTransform(rotation, false);
    override x.AfterTransform(rotationAngle:double) = ()