//-----------------------------------------------------------------------------
// File:        Cubicle.fs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        07/25/2009
//-----------------------------------------------------------------------------
namespace Knoics.RubiksCube

open System
open System.Collections.Generic
open System.Windows
open System.Windows.Media
open System.Diagnostics
open Knoics.Math
open Kit3D.Windows.Media.Media3D

type CubieFace (cubie:Cubie , name:string , center:Vector3D , size:double, faceMesh:IMesh, cubicleFace:CubicleFace option) = 
    let _meshes = new List<IMesh>()
    let _center = center
    let _name = name
    let _size = size
    let _cubie = cubie
    let mutable _faceMesh:IMesh = faceMesh
    let mutable _cubicleFace:CubicleFace option = cubicleFace

    let mutable _transform = Matrix3D.Identity
    let mutable _savedTransform = Matrix3D.Identity
    let mutable _unfoldTransform = Matrix3D.Identity
    
    
    interface ITransform with
        member x.Transform = _transform
        member x.Save() =
            _savedTransform <- _transform
            for mesh in _meshes do mesh.Save()

        member x.Restore() =
            _transform <- _savedTransform
            for mesh in _meshes do mesh.Restore()

        member x.DoTransform(matrix:Matrix3D, isFromSaved:bool)=
            if (isFromSaved) then
                _transform <- matrix * _savedTransform
            else
                _transform <- matrix * _transform

            for mesh in _meshes do mesh.DoTransform(matrix, isFromSaved)

        member x.Reset() =
            _transform <- Matrix3D.Identity
            _savedTransform <- Matrix3D.Identity
            _unfoldTransform <- Matrix3D.Identity
            for mesh in _meshes do mesh.Reset()
            
    member x.Meshes = _meshes
    member x.FaceMesh = _faceMesh
    member x.CubicleFace with get() = _cubicleFace and
                            set v = _cubicleFace <- v

    member x.Cubie = _cubie
    member x.Name = _name

    static member ConstructVertexes(axis:Axis, center:Vector3D, size:Size, edgeWidth:double)=
        let positions = [|for i in 0 .. 3 -> new Vector3D()|]
        let mutable vertex = center;
        let sz = new Size(size.Width - edgeWidth * 2.0, size.Height - edgeWidth * 2.0)
        match axis with
        |Axis.X -> 
            //right-bottom
            vertex.Y <- vertex.Y - sz.Width / 2.
            vertex.Z <- vertex.Z - sz.Height / 2.
            positions.[0] <- vertex// Add(vertex);
            //right-top
            vertex.Y <- vertex.Y + sz.Width
            positions.[1] <- vertex// .Add(vertex);
            //left-top
            vertex.Z <- vertex.Z+ sz.Height
            positions.[2] <- vertex//.Add(vertex);
            //left-bottom
            vertex.Y <- vertex.Y - sz.Width
            positions.[3] <- vertex//.Add(vertex);
            positions
        |Axis.Y ->
            vertex.Z <- vertex.Z - sz.Width / 2.
            vertex.X <- vertex.X - sz.Height / 2.
            positions.[0] <- vertex//.Add(vertex);
            
            vertex.Z <- vertex.Z+sz.Width 
            positions.[1] <- vertex// .Add(vertex);
            vertex.X <- vertex.X + sz.Height 
            positions.[2] <- vertex//.Add(vertex);
            vertex.Z <- vertex.Z - sz.Width
            positions.[3] <- vertex;//.Add(vertex);
            positions
        |Axis.Z ->
            vertex.X <- vertex.X- sz.Width / 2.
            vertex.Y <- vertex.Y - sz.Height / 2.
            positions.[0] <- vertex//.Add(vertex);
            vertex.X <- vertex.X + sz.Width
            positions.[1] <- vertex;// .Add(vertex);
            vertex.Y <- vertex.Y + sz.Height
            positions.[2] <- vertex;// .Add(vertex);
            vertex.X <- vertex.X-sz.Width
            positions.[3] <- vertex;// .Add(vertex);
            positions
        |_  -> positions    
    static member ConstructCubieFace(cubie:Cubie , name:string , center:Vector3D , size:double )=
        let faceSize = new Size(size, size)
        let edgeWidth = size * 0.05
        let u = CubeConfiguration.Factory.Value.CreateMesh(name, CubieFace.ConstructVertexes(CubeConfiguration.Faces.[name].Normal, center, faceSize, edgeWidth), CubeConfiguration.Faces.[name].Color)
        let face = new CubieFace(cubie, name, center, size, u, None)
        face.Meshes.Add(u)

        let decorCenters = [|for i in 0..3 -> Vector3D()|]
        let sizes = [|for i in 0..3 -> Size()|]
        if (name = "U" || name = "D") then
            //decorator:
            let mutable decorCenter = center
            decorCenter <- center
            decorCenter.Z <- decorCenter.Z + (size / 2. - edgeWidth / 2.)
            decorCenters.[0] <- decorCenter
            sizes.[0] <- new Size(edgeWidth, size)
            decorCenter <- center
            decorCenter.Z <- decorCenter.Z - (size / 2. - edgeWidth / 2.)
            decorCenters.[1] <- decorCenter
            sizes.[1] <- new Size(edgeWidth, size)
            decorCenter <- center
            decorCenter.X <- decorCenter.X + (size / 2. - edgeWidth / 2.)
            decorCenters.[2] <- decorCenter
            sizes.[2] <- new Size(size,edgeWidth)
            decorCenter <- center
            decorCenter.X <- decorCenter.X - (size / 2. - edgeWidth / 2.)
            decorCenters.[3] <- decorCenter
            sizes.[3] <- new Size(size, edgeWidth)
        else if (name = "F" || name = "B") then
            let mutable decorCenter = center
            decorCenter <- center 
            decorCenter.X <- decorCenter.X+(size / 2. - edgeWidth / 2.)
            decorCenters.[0] <- decorCenter 
            sizes.[0] <- new Size(edgeWidth, size)
            decorCenter <- center 
            decorCenter.X <- decorCenter.X-(size / 2. - edgeWidth / 2.)
            decorCenters.[1] <- decorCenter
            sizes.[1] <- new Size(edgeWidth, size)
            decorCenter <- center 
            decorCenter.Y <- decorCenter.Y+(size / 2. - edgeWidth / 2.)
            decorCenters.[2] <- decorCenter
            sizes.[2] <- new Size(size, edgeWidth)
            decorCenter <- center 
            decorCenter.Y <- decorCenter.Y-(size / 2. - edgeWidth / 2.)
            decorCenters.[3] <- decorCenter
            sizes.[3] <- new Size(size, edgeWidth)
        else if (name = "L" || name = "R") then
            //decorator:
            let mutable decorCenter = center
            decorCenter <- center
            decorCenter.Y <- decorCenter.Y+(size / 2. - edgeWidth / 2.)
            decorCenters.[0] <- decorCenter
            sizes.[0] <- new Size(edgeWidth, size)
            decorCenter <- center
            decorCenter.Y <- decorCenter.Y - (size / 2. - edgeWidth / 2.)
            decorCenters.[1] <- decorCenter
            sizes.[1] <- new Size(edgeWidth, size)
            decorCenter <- center
            decorCenter.Z <- decorCenter.Z + (size / 2. - edgeWidth / 2.)
            decorCenters.[2] <- decorCenter
            sizes.[2] <- new Size(size, edgeWidth)
            decorCenter <- center
            decorCenter.Z <- decorCenter.Z - (size / 2. - edgeWidth / 2.)
            decorCenters.[3] <- decorCenter
            sizes.[3] <- new Size(size, edgeWidth)
        else
            System.Diagnostics.Debug.Assert(false)

        
        for i = 0 to decorCenters.Length-1 do
            face.Meshes.Add(CubeConfiguration.Factory.Value.CreateMesh(face.Name, CubieFace.ConstructVertexes(CubeConfiguration.Faces.[name].Normal, decorCenters.[i], sizes.[i], 0.), Colors.Black))
        face
    


    override x.ToString() =
        string.Format("{0}-{1}", _cubie, _name)


            
and Cubie(cubicle:Cubicle, name:string, center:Vector3D, size:double) =
    inherit RotatableObject()
    let _name = name
    let mutable _cubicle = cubicle
    let mutable _center = center
    let _size = size
    
    
    let _faces = new Dictionary<string,CubieFace>()

    member x.Name = _name
    member x.Center = _center
    member x.Size = _size
    member x.Cubicle with get() = _cubicle and set v = _cubicle <- v
    member x.Faces = _faces

    member x.BoundingBox 
        with get() =
            let size = _size
            let mutable min = _center 
            min.X <- min.X - size / 2.
            min.Y <- min.Y - size / 2.
            min.Z <- min.Z-size / 2.
            let mutable max = _center
            max.X <- max.X+ size / 2.
            max.Y <- max.Y+size / 2.
            max.Z <- max.Z + size / 2.
            new BoundingBox3D(min, max)


    static member CreateCubie(cubicle:Cubicle, name:string , center:Vector3D , size:double )=
        let cubie = new Cubie(cubicle, name, center, size)
        let mutable c = center
        let offset = size /2.
        if (name.IndexOf("U") >= 0)then
            c <- center
            c.Y <- c.Y + offset
            let face = CubieFace.ConstructCubieFace(cubie, "U", c, size);
            cubie.Faces.Add("U", face)
            cubicle.SetCubieFace("U", face)

        if (name.IndexOf("D") >= 0)then
            c <- center
            c.Y <- c.Y-offset
            let face = CubieFace.ConstructCubieFace(cubie, "D", c, size)
            cubie.Faces.Add("D", face)
            cubicle.SetCubieFace("D", face)

        if (name.IndexOf("F") >= 0) then
            c <- center
            c.Z <- c.Z + offset
            let face = CubieFace.ConstructCubieFace(cubie, "F", c, size)
            cubie.Faces.Add("F", face)
            cubicle.SetCubieFace("F", face);

        if (name.IndexOf("B") >= 0) then
            c <- center
            c.Z <- c.Z-offset
            let face = CubieFace.ConstructCubieFace(cubie, "B", c, size)
            cubie.Faces.Add("B", face)
            cubicle.SetCubieFace("B", face)
        if (name.IndexOf("L") >= 0)then
            c <- center
            c.X <- c.X - offset
            let face = CubieFace.ConstructCubieFace(cubie, "L", c, size)
            cubie.Faces.Add("L", face)
            cubicle.SetCubieFace("L", face)
        if (name.IndexOf("R") >= 0) then
            c <- center
            c.X <- c.X + offset
            let face = CubieFace.ConstructCubieFace(cubie, "R", c, size)
            cubie.Faces.Add("R", face)
            cubicle.SetCubieFace("R", face)
        cubie

    override x.ToString() =
        string.Format("{0}-{1}", _cubicle, _name)

    override x.Reset() =
        base.Reset()
        for face in _faces.Values do 
            (face:>ITransform).Reset()

    override x.Restore() =
        base.Restore()
        for face in _faces.Values do (face:>ITransform).Restore()
        _center <- Ext3D.Transform(_center, base.Transform)

    override x.Save() =
        base.Save()
        for face in _faces.Values do (face:>ITransform).Save()

    override x.Rotate(axis:Axis, deltaAngle:double, isFromSaved:bool) =
        let matrix = base.Rotate(axis, deltaAngle, isFromSaved)

        _center <- Ext3D.Transform(_center,  base.Transform)
        for face in _faces.Values do
            (face:>ITransform).DoTransform(matrix, isFromSaved)
        matrix

and CubicleFace(name:string, cubicle:Cubicle, cubieFace:CubieFace option) =
    let _name = name
    let _cubicle = cubicle
    let mutable _cubieFace:CubieFace option = cubieFace
    member x.Name = _name
    member x.Cubicle = _cubicle
    
    member x.CubieFace with get() = _cubieFace
                       and set v = _cubieFace <- v
    
    member x.SetCubieFace(cubieFace:CubieFace) =
        _cubieFace <- Some(cubieFace)
        cubieFace.CubicleFace <- Some(x)

    override x.ToString() =
        string.Format("{0}==face:{1}-{2}", _cubicle.Cubie.ToString(), _name,  _cubieFace.Value.Name)
        
and Cubicle(name:string, center:Vector3D, size:double, cubie:Cubie option) as this =
    let _name  = name
    let _center = center
    let _size = size
    let _cubicleFaces = new Dictionary<string, CubicleFace>()
    let mutable _cubie:Cubie option = cubie
    do this.InitFaces(name)

    member x.InitFaces(name:string)=
        for i = 0 to name.Length-1 do
            let faceName = name.Substring(i, 1)
            let mutable result = 0
            if(int.TryParse(faceName, &result)=false)then
                _cubicleFaces.Add(faceName, new CubicleFace(faceName, this, None))
        
    member x.Name = _name
    member x.Cubie with get() = _cubie and set v = _cubie <- v
    member x.Center = _center
    member x.Size = _size
    member x.Faces = _cubicleFaces
    
//    public Dictionary<string, CubicleFace> _cubicleFaces;

    member x.CubieFaces 
        with get() =
            let faces = new Dictionary<string, CubieFace>()
            for key in _cubicleFaces.Keys do
                faces.Add(key, _cubicleFaces.[key].CubieFace.Value)
            faces
        
    member x.BoundingBox 
        with get() =
            let size = _size
            let mutable min = _center
            min.X <- min.X - size / 2.
            min.Y <- min.Y - size / 2. 
            min.Z <- min.Z - size / 2.
            let mutable max = _center
            max.X <- max.X  + size / 2.
            max.Y <- max.Y +  size / 2.
            max.Z <- max.Z +  size / 2.
            new BoundingBox3D(min, max)

    


    member x.SetCubieFace(faceName:string, face:CubieFace) =
        _cubicleFaces.[faceName].SetCubieFace(face)

    //return origial Faces
    member x.SetCubieFaces(fromFaces:Dictionary<string, CubieFace>, fromString:string, toString:string)=
        Debug.Assert(fromString.Length = toString.Length);
        let originalFaces = x.CubieFaces
        for i = 0 to fromString.Length-1 do
            let fromFace = fromString.Substring(i, 1)
            let toFace = toString.Substring(i, 1)
            _cubicleFaces.[toFace].SetCubieFace(fromFaces.[fromFace])
        originalFaces
        



    static member CreateCubicle(cubicleName:string, cubieName:string, center:Vector3D , size:double ) =
        let cubicle = new Cubicle(cubicleName, center, size, None)
        let cubie = Cubie.CreateCubie(cubicle, cubieName, center, size)
        cubicle.Cubie <- Some(cubie)
        cubicle



    member x.SetCubie(cubie:Cubie) =
        let original = _cubie
        _cubie <- Some(cubie)
        cubie.Cubicle <- x
        original

    override x.ToString() = _name

