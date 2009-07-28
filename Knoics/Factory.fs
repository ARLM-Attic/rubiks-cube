//-----------------------------------------------------------------------------
// File:        Factory.fs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        07/27/2009
//-----------------------------------------------------------------------------

namespace Knoics.RubiksCube

open System
open System.Linq
open System.Windows
open System.Windows.Media
open Knoics.Math
open Kit3D.Windows.Media
open Kit3D.Windows.Media.Media3D

type CubeModel() = 
    let _visualModel:ModelVisual3D = new ModelVisual3D()
    let mutable _savedTransform = Matrix3D.Identity
    let group = new Model3DGroup()
    do _visualModel.Content <- group
    let rotation = new RotateTransform3D()
    do _visualModel.Transform <- rotation
    do rotation.Rotation <- new AxisAngleRotation3D()
    member x.ModelVisual = _visualModel
    interface IModel with
        member x.Transform = _visualModel.Transform.Value

        member x.DoTransform(matrix:Matrix3D, isFromSaved:bool) =
            if (isFromSaved)then
                _visualModel.Transform <- new MatrixTransform3D(Matrix3D.Multiply(matrix, _savedTransform))// m * _model.Transform;
            else
                _visualModel.Transform <- new MatrixTransform3D(Matrix3D.Multiply(matrix, _visualModel.Transform.Value))// m * _model.Transform;

        member x.Reset() =
            _visualModel.Transform <- new MatrixTransform3D(Matrix3D.Identity)

        member x.Save()=
            _savedTransform <- _visualModel.Transform.Value
        member x.Restore() =
            _visualModel.Transform <- new MatrixTransform3D(_savedTransform)


type CubieMesh (faceName:string, vertexes:Vector3D[], color:Color) =
    let _positiveFaces = [|"U"; "F"; "R" |]
    let _geometry = new GeometryModel3D()
    let _mesh = new MeshGeometry3D()
    do _geometry.Geometry <- _mesh
    let colorMaterial = new DiffuseMaterial(new Kit3DBrush(new SolidColorBrush(color)))
    let backcolorMaterial = new DiffuseMaterial(new Kit3DBrush(new SolidColorBrush(Colors.Black)))
    do _geometry.Material <- colorMaterial
    do _geometry.BackMaterial <- backcolorMaterial
         
    let mutable _savedTransform = Matrix3D.Identity
    let PositionsToPoint3DCollection(positions:Vector3D[])=
        let points = new Point3DCollection(positions.Length)
        for pos in positions do
            points.Add(Point3D(pos.X, pos.Y, pos.Z))
        points
         
    let isPositiveFace = 
        _positiveFaces |> Array.exists(fun fn -> fn = faceName)
        
    let getCollection =                 
        if (isPositiveFace) then 
            new Int32Collection([0;1;3;1;2;3])
        else 
            new Int32Collection([3;1;0;3;2;1])
    
    (*WPF Version
    MaterialGroup unlitMaterial = new MaterialGroup();
    unlitMaterial.Children.Add(new DiffuseMaterial(new SolidColorBrush(Colors.Black)));
    unlitMaterial.Children.Add(new EmissiveMaterial(new SolidColorBrush(color)));
    //unlitMaterial.Freeze();
    _geometry.Material = unlitMaterial;
    _geometry.BackMaterial = unlitMaterial;
     *)
    do _mesh.Positions <- PositionsToPoint3DCollection(vertexes)
//    if (string.IsNullOrEmpty(PositiveFaces.FirstOrDefault(fun f -> f = faceName))=false) then//X,Y,Z direction
    do _mesh.TriangleIndices <- getCollection
    
    do _geometry.SeamSmoothing <- 1.
        
    member x.Geometry = _geometry
    member x.Mesh = _mesh


    //#region IMesh Members
    interface IMesh with
        member x.Reset() =
            _geometry.Transform <- new MatrixTransform3D(Matrix3D.Identity)
        member x.Save()=
            _savedTransform <- _geometry.Transform.Value
        member x.Restore() =
            _geometry.Transform <- new MatrixTransform3D(_savedTransform)
            
        member x.DoTransform(matrix:Matrix3D,isFromSaved:bool) =
            if(isFromSaved) then
                _geometry.Transform <- new MatrixTransform3D(Matrix3D.Multiply(matrix, _savedTransform))// m * _model.Transform;
            else
                _geometry.Transform <- new MatrixTransform3D(Matrix3D.Multiply(matrix, _geometry.Transform.Value))// m * _model.Transform;

        member x.Transform with get() = _geometry.Transform.Value
    //#endregion


type Factory() =
    interface IFactory with
        member x.CreateMesh(faceName:string, vertexes:Vector3D[], color:Color) =
            new CubieMesh(faceName, vertexes, color) :> IMesh

        member x.CreateModel() =
            new CubeModel() :> IModel
