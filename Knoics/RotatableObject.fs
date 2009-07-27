//-----------------------------------------------------------------------------
// File:        RotatableObject.fs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        07/25/2009
//-----------------------------------------------------------------------------
namespace Knoics.RubiksCube
open Knoics.Math
open Kit3D.Windows.Media.Media3D



  

type RotatableObject() =
    let mutable _transform:Matrix3D = Matrix3D.Identity
    let mutable _savedTransform = Matrix3D.Identity
    let mutable _axisTransform = Matrix3D.Identity;
    
    let mutable _unitX = Ext3D.UnitX
    let mutable _unitY = Ext3D.UnitY
    let mutable _unitZ = Ext3D.UnitZ


    member x.UnitX with get() = _unitX
    member x.UnitY with get() = _unitY
    member x.UnitZ with get() = _unitZ

    member x.Transform with get() = _transform

    abstract member Reset: unit -> unit
    default x.Reset() =
        _transform <- Matrix3D.Identity
        _savedTransform <- Matrix3D.Identity
        _axisTransform <- Matrix3D.Identity
        _unitX <- Ext3D.UnitX
        _unitY <- Ext3D.UnitY
        _unitZ <- Ext3D.UnitZ

    abstract member Save: unit->unit
    default x.Save() =
        _savedTransform <- _transform;


    abstract member Restore: unit->unit
    default x.Restore()=
        _transform <- _savedTransform;


        
    member x.RotateUnit(axis:Axis, rotation:double) = 
        let GetTransform axis rotation=
            match axis with
            |Axis.X -> Ext3D.CreateRotationX(rotation)
            |Axis.Y -> Ext3D.CreateRotationY(rotation)
            |Axis.Z -> Ext3D.CreateRotationZ(rotation)
    
        let transform = GetTransform axis rotation
        _axisTransform <- _axisTransform * transform
        let axisTransform = _axisTransform
        axisTransform.Invert()
        _unitX <- Ext3D.Transform(Ext3D.UnitX, axisTransform)
        _unitY <- Ext3D.Transform(Ext3D.UnitY, axisTransform)
        _unitZ <- Ext3D.Transform(Ext3D.UnitZ, axisTransform)

    abstract member Rotate: Axis*double*bool->Matrix3D
    default x.Rotate (axis:Axis, deltaAngle:double, isFromSaved:bool) =
        let GetQuaternion axis rotation=
            match axis with
            |Axis.X -> Quaternion3D.CreateFromAxisAngle(_unitX, rotation)
            |Axis.Y -> Quaternion3D.CreateFromAxisAngle(_unitY, rotation)
            |Axis.Z -> Quaternion3D.CreateFromAxisAngle(_unitZ, rotation)
            
        let rotationQ = GetQuaternion axis deltaAngle
        let matrix = Ext3D.CreateFromQuaternion(rotationQ);
        if (isFromSaved) then
            _transform <- matrix * _savedTransform;
        else
            _transform <- matrix * _transform;
        matrix

