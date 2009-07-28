//-----------------------------------------------------------------------------
// File:        RubiksCube.fs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        07/26/2009
//-----------------------------------------------------------------------------
namespace Knoics.RubiksCube

open System;
open System.Collections.Generic;
open System.Linq;
open System.Text;
open Knoics.Math;
open System.Diagnostics;
open System.Windows;
open Kit3D.Windows.Media.Media3D;




type SelectMode =
    | Cubies
    | Cube

type CubeSelection = {
    SelectedCubies:IEnumerable<Cubie>;
    SelectMode:SelectMode 
    }

type OneOpDone = delegate of string * string * int  * bool -> unit
type HitResult = {
    HitPoint:Point3D;
    HitCubicle:Cubicle option;
    Distance:double
    }

type HitTestResult = {Distance:double option;HitResult:HitResult option}

type RubiksCube(cubieSize:double, cubeSize:CubeSize, boundingBox:BoundingBox3D, oneOpDone:OneOpDone, factory:IFactory) as this=
    inherit RotatableObject()
    let _oneOpDone = oneOpDone
    let mutable _steps:int = 0
    let mutable _sequence = string.Empty


    let _cubicles = new Dictionary<string, Cubicle>()
    let _cubieSize = cubieSize
    let _cubeSize = cubeSize
    let _model = factory.CreateModel()
    let _boundingBox = boundingBox
    let _factory = factory
        
    let mutable _selectedCubies:IEnumerable<Cubie>  = null
    let _selectedBasicOp:string = string.Empty
    let mutable _unfolded = false
    
    let mutable _prevHit:HitResult option = None
    let mutable _angle:double option = None
    let mutable _basicOp:string = string.Empty
    let mutable _axis = Axis.X
    let mutable _selected:CubeSelection option = None
    
    //ViewProjection Matrix
    let mutable _viewMatrix = Matrix3D.Identity
    let mutable _inverseViewMatrix = Matrix3D.Identity
    
    let mutable _projectionMatrix = Matrix3D.Identity
    let mutable _inverseProjectionMatrix = Matrix3D.Identity
    
    let mutable _viewpoint = new Point3D()
    
    
    let AfterOp(op:string) =
        let isSolved = this.IsSolved()
        //let text = _sequence;// tbCubeOps.Text;
        let fetchResult = CubeOperation.FetchOp(_sequence, true)
        let action = fst fetchResult
        let text = snd fetchResult
        if (CubeOperation.IsReverse(action, op))then
            //cancel
            _steps <- _steps-1
            _sequence <- text;
        else
            _steps<-_steps+1
            _sequence <- _sequence + op
        if (_oneOpDone <> null)then
            _oneOpDone.Invoke(op, _sequence, _steps, isSolved)
    
    
    let GetFaces(faceName:string)=
        seq {for cubicle in _cubicles.Values do
            if (cubicle.Name.IndexOf(faceName) >= 0) then
                yield cubicle.Faces.[faceName]}
    
    let BeforeOp(animContext:AnimContext) =
        let mutable op = animContext.Op
        if (op = CubeOperation.UndoOp&&string.IsNullOrEmpty(_sequence))then false
        else
            if(op = CubeOperation.UndoOp) then
                op <- fst (CubeOperation.FetchOp(_sequence, true))
                op <- CubeOperation.GetReverseOp(op)
                if (string.IsNullOrEmpty(op) = false) then
                    animContext.Op <- op
            let size = _cubeSize
            let edgeX = _cubieSize * double(size.Width) / 2.
            let edgeY = _cubieSize * double(size.Height) / 2.
            let edgeZ = _cubieSize * double(size.Depth) / 2.

            if (op = CubeOperation.FoldOp || op = CubeOperation.UnFoldOp) then
                let unfold = (op = CubeOperation.UnFoldOp)
                let f = new FaceTransform("F", Axis.X, false, new Vector3D(0., edgeY, edgeZ), new Vector3D(), fun face -> GetFaces(face))
                f.Silent <- true
                f.ChangeAngle <- if(unfold) then -CubeOperation.PiOver2 else CubeOperation.PiOver2
                f.Begin <- 0.0
                
                let b = new FaceTransform("B", Axis.X, false, new Vector3D(0., edgeY, -edgeZ), new Vector3D(), fun face -> GetFaces(face))
                b.Silent <- true
                b.ChangeAngle <- if(unfold) then CubeOperation.PiOver2 else -CubeOperation.PiOver2
                b.Begin <- 0.0
                
                let l = new FaceTransform("L", Axis.Z, false,new Vector3D(-edgeX, edgeY, 0.),new Vector3D(), fun face -> GetFaces(face))
                l.Silent <- true
                l.ChangeAngle <- if unfold then -CubeOperation.PiOver2 else CubeOperation.PiOver2
                l.Begin <- 0.

                let r = new FaceTransform("R", Axis.Z, false, new Vector3D(edgeX, edgeY, 0.), new Vector3D(), fun face -> GetFaces(face))
                r.Silent <- true
                r.ChangeAngle <- if unfold  then CubeOperation.PiOver2 else -CubeOperation.PiOver2
                r.Begin <- 0.


                let d = new FaceTransform("D", Axis.Z, true,new Vector3D(edgeX, -edgeY, 0.),new Vector3D(edgeX, edgeY, 0.), fun face -> GetFaces(face))
                d.Silent <- true
                d.ChangeAngle <- if unfold then CubeOperation.PiOver2 else -CubeOperation.PiOver2
                d.Begin <- 0.
                animContext.TransformParams.Add(f)
                animContext.TransformParams.Add(b)
                animContext.TransformParams.Add(l)
                animContext.TransformParams.Add(r)
                animContext.TransformParams.Add(d)
                true
            else
                if (CubeOperation.IsValidOp(op)) then
                    let getResult = CubeOperation.GetBasicOp(op);
                    let basicOp = fst getResult
                    let isReverse = snd getResult
                    let transform = new CubieTransform(op, isReverse, basicOp, (fun cn->this.Cubicles.[cn]))
                    transform.Silent <- animContext.Silent
                    transform.ChangeAngle <- transform.RotateAngle - animContext.RotatedAngle;
                    animContext.TransformParams.Add(transform)
                    true
                else false
                    
    let _animator = new Animator(BeforeOp, AfterOp )
                    
    member x.ViewMatrix 
        with get() = _viewMatrix
        and set v =
            _viewMatrix <- v
            _inverseViewMatrix <- _viewMatrix
            _inverseViewMatrix.Invert()

    member x.ProjectionMatrix 
        with get() =_projectionMatrix
        and set v=
            _projectionMatrix <- v
            _inverseProjectionMatrix <- _projectionMatrix;// Matrix.Invert(_projectionMatrix);
            _inverseProjectionMatrix.Invert()

    member x.InverseProjectionMatrix
        with set v=
            _inverseProjectionMatrix <- v
            _projectionMatrix <- _inverseProjectionMatrix// Matrix.Invert(_inverseProjectionMatrix);
            _projectionMatrix.Invert()

    member x.ViewPoint with get() = _viewpoint and set v = _viewpoint <- v
    
    
    member x.BoundingBox = _boundingBox
    member x.CubieSize = _cubieSize
    member x.CubeSize = _cubeSize
    member x.Cubicles:Dictionary<string, Cubicle> = _cubicles
    member x.Model = _model
    member x.Animator = _animator
    member x.Unfolded with get() = _unfolded

    static member CreateRubiksCube(origin:Vector3D,cubieNum:int,cubieSize:double, oneOpDone:OneOpDone, factory:IFactory) =
        let size = new CubeSize(cubieNum, cubieNum, cubieNum )
        let mutable start = origin
        start.X <- start.X - double(size.Width - 1) * cubieSize / 2.
        start.Y <- start.Y - double(size.Height - 1) * cubieSize / 2.
        start.Z <- start.Z - double(size.Depth - 1) * cubieSize / 2.
        let mutable min = origin
        min.X <- min.X- double(size.Width) * cubieSize / 2.
        min.Y <- min.Y - double(size.Height) * cubieSize / 2.
        min.Z <- min.Z - double(size.Depth) * cubieSize / 2.
        let mutable max = origin
        max.X <- max.X + double(size.Width) * cubieSize / 2.
        max.Y <- max.Y + double(size.Height) * cubieSize / 2.
        max.Z <- max.Z + double(size.Depth) * cubieSize / 2.
        let boundingBox = new BoundingBox3D(min, max)
        let rubiksCube = new RubiksCube(cubieSize, size, boundingBox, oneOpDone, factory)
        
        for i = 0 to size.Width - 1 do
            for j = 0 to size.Height - 1 do
                for k = 0 to size.Depth - 1 do
                    let mutable cubieOri = start
                    cubieOri.X <- cubieOri.X + double(i) * cubieSize
                    cubieOri.Y <- cubieOri.Y + double(j) * cubieSize
                    cubieOri.Z <- cubieOri.Z + double(k) * cubieSize
                    let cubicleName = CubeConfiguration.GetCubicleName(size, i, j, k)
                    //Debug.WriteLine(string.Format("({0},{1},{2}): {3}", i,j,k, cubicleName));
                    if (string.IsNullOrEmpty(cubicleName)=false)then
                        let cubieName = cubicleName //solved configuration
                        let cubicle = Cubicle.CreateCubicle(cubicleName, cubieName, cubieOri, cubieSize, factory)
                        rubiksCube.Cubicles.Add(cubicleName, cubicle)
        rubiksCube
    

    member x.Item with get(key:string) = _cubicles.[key]

    member x.Unfold(frames:double)=
        let unfold = not _unfolded
        _animator.Start(Some(new AnimContext(0.0, frames, (if unfold then CubeOperation.UnFoldOp else CubeOperation.FoldOp), true, new List<Transform>())))
        _unfolded <- unfold

    member x.Op(op:string, frames:int) =
        if (string.IsNullOrEmpty(op)=false)then
            let unfolded = _unfolded
            let unfoldFrames = if frames <= 3 then 1. else double(frames) / 3.
            if (unfolded) then
                x.Unfold(unfoldFrames)
            x.Rotate(op, (if unfolded then unfoldFrames else double(frames)), false, 0., false)
            if (unfolded)then
                x.Unfold(unfoldFrames)

    member x.Undo() =
        if(_sequence.Length>0)then
            x.Op(CubeOperation.UndoOp, 30)


    member x.DoCommand(commandString:string)=
        let text = commandString.Trim().ToUpper()
        if (string.IsNullOrEmpty(text)) then text
        else
            let fetchResult = CubeOperation.FetchOp(text, false)
            let action = fst fetchResult
            x.Op(action, 30)
            snd fetchResult


    member x.Meshes 
        with get() =
            seq {
                for cubicle in _cubicles.Values do
                    for face in cubicle.Cubie.Value.Faces.Values do
                        for mesh in face.Meshes do
                            yield mesh
                            }
    

    override x.Save()=
        base.Save()
        _model.Save()

    override x.Restore()=
        base.Restore()
        _model.Restore()

    override x.Reset()=
        base.Reset()
        _steps <- 0
        _sequence <- string.Empty
        _selectedCubies <- null
        _unfolded <- false
        let cubies = _cubicles.Values.Select(fun (c:Cubicle) -> c.Cubie.Value).ToArray()
        for cubie in cubies do
            cubie.Reset()
            _cubicles.[cubie.Name].SetCubie(cubie)
            let seq = string.Concat(cubie.Faces.Keys.ToArray())
            _cubicles.[cubie.Name].SetCubieFaces(cubie.Faces, seq, seq)
            //Debug.WriteLine(seq)
    

    override x.Rotate(axis:Axis, deltaAngle:double, isFromSaved:bool) = 
        let matrix = base.Rotate(axis, deltaAngle, isFromSaved)
        _model.DoTransform(matrix, isFromSaved)
        matrix

    member x.IsFaceSoved(face:string)=
        let cubicles = _cubicles.Where(fun (p:KeyValuePair<string, Cubicle>) -> p.Key.IndexOf(face)>= 0) |> Seq.map (fun kv -> kv.Value)
        let faceToMatch = _cubicles.[face].Cubie.Value.Name
        cubicles |> Seq.exists(fun c -> c.Cubie.Value.Name.IndexOf(faceToMatch) < 0) = false //no matched

    member x.IsSolved()=
        CubeConfiguration.Faces.Keys |> Seq.exists( fun face -> (x.IsFaceSoved(face)=false) ) = false


    
    /// <summary>
    /// Only if deep is true, then HitCubieResult could have some value
    /// </summary>
    /// <param name="pt"></param>
    /// <param name="deep"></param>
    /// <param name="hitResult"></param>
    /// <returns></returns>
    member x.HitTest(pt:Point, deep:bool)=
        let mutable hitResult:HitResult option = None
        let mutable ray = Ext3D.Unproject(pt, _viewpoint, _model.Transform, _inverseViewMatrix, _inverseProjectionMatrix)
        //Debug.WriteLine(string.Format("ray: {0}", ray));
        //double? d = this.BoundingBox.Intersects(ray);
        let d = this.BoundingBox.Intersects(ray)
        if (deep=false) then
            if (Option.isSome(d))then
                //Debug.WriteLine(string.Format("first ray: {0}, distance:{1}", ray, (double)d));
                hitResult <- Some { Distance = d.Value ; HitCubicle = None; HitPoint = ray.Origin + d.Value * ray.Direction }
        else
            let results = new List<HitResult>()
            if (Option.isSome(d))then
                //double? d1;
                for cubicle in _cubicles.Values do
                    let localToWorld = _model.Transform
                    localToWorld.Invert()// *cubicle.Cubie.Transform;
                    ray <- Ext3D.Unproject(pt, _viewpoint, localToWorld, _inverseViewMatrix, _inverseProjectionMatrix)
                    //d1 = cubicle.Cubie.BoundingBox.Intersects(ray);
                    let d1 = cubicle.BoundingBox.Intersects(ray)
                    if (Option.isSome(d1)) then
                        let result = { Distance = d1.Value; HitCubicle = Some cubicle; HitPoint = ray.Origin + d1.Value * ray.Direction }
                        results.Add(result)
                let comparison (a:HitResult) (b:HitResult) = a.Distance.CompareTo(b.Distance)
                results.Sort(new Comparison<HitResult>(comparison))
                if (results.Count > 0)then
                    hitResult <- Some results.[0]
        //d has some thing: true
        //(d, hitResult)
        {Distance=d;HitResult = hitResult}

    member x.HitTestOnly(pt:Point, deep:bool)=
        let hitTest = x.HitTest(pt, deep)
        Option.isSome(hitTest.Distance)
    
    member x.TestAngle(pt:Point, prevHit:HitResult)=
        let hitTest = x.HitTest(pt, true)
        let hit = Option.isSome(hitTest.Distance)
        
        let mutable angle:double option = None
        let mutable axis = Axis.X
        if (hit) then
            let hitResult = hitTest.HitResult.Value// snd hitTest
            angle <- Some 0.0
            axis <- Axis.Z
            let oxy = new Point3D(0., 0., prevHit.HitPoint.Z)
            let mutable fromV = prevHit.HitPoint - oxy
            let mutable toV = new Point3D(hitResult.HitPoint.X, hitResult.HitPoint.Y, prevHit.HitPoint.Z) - oxy
            let xyAngle = Ext3D.AngleBetween(fromV, toV)
             
            angle <- Some xyAngle
            if (Vector3D.DotProduct(Ext3D.UnitZ, Vector3D.CrossProduct(fromV, toV)) < 0.) then
                angle <- Some(-angle.Value)
            

            //plane yz, where x = prevHit.X, Axis.X
            let oyz = new Point3D(prevHit.HitPoint.X, 0., 0.)
            fromV <- prevHit.HitPoint - oyz
            toV <- new Point3D(prevHit.HitPoint.X, hitResult.HitPoint.Y, hitResult.HitPoint.Z) - oyz
            let yzAngle = Ext3D.AngleBetween(fromV, toV)
            if (System.Math.Abs(yzAngle) > System.Math.Abs(angle.Value))then
                angle <- Some yzAngle
                axis <- Axis.X
                if (Vector3D.DotProduct(Ext3D.UnitX, Vector3D.CrossProduct(fromV, toV)) < 0.) then
                    angle <- Some(-angle.Value)

            //plane zx, where y = prevHit.Y, Axis.Y
            let ozx = new Point3D(0., prevHit.HitPoint.Y, 0.)
            fromV <- prevHit.HitPoint - ozx
            toV <- new Point3D(hitResult.HitPoint.X, prevHit.HitPoint.Y, hitResult.HitPoint.Z) - ozx
            let zxAngle = Ext3D.AngleBetween(fromV, toV)
            if (System.Math.Abs(zxAngle) > System.Math.Abs(angle.Value)) then
                angle <- Some zxAngle
                axis <- Axis.Y
                if (Vector3D.DotProduct(Ext3D.UnitY, Vector3D.CrossProduct(fromV, toV)) < 0.)then
                    angle <- Some(-angle.Value)
            let threshold = Angle.DegreesToRadians(1.0)
            if (Option.isSome angle) then
                //Debug.WriteLine(string.Format("angle: {0}, threshold:{1}", angle, threshold));
                if (System.Math.Abs(angle.Value) < threshold) then
                    angle <- None
        (angle, axis)
    
    member x.NoOp with get() = _animator.NoOp && (_unfolded=false)
    member x.TestAngle(pt:Point, prevHit:HitResult, axis:Axis )=
        let ray = Ext3D.Unproject(pt, _viewpoint, _model.Transform, _inverseViewMatrix, _inverseProjectionMatrix)
        let mutable angle = 0.
        if (axis=Axis.X) then
            let v = new Vector3D(-1., 0., 0.)
            let yz = new Plane3D(v, prevHit.HitPoint.X)
            let oyz = new Point3D(prevHit.HitPoint.X, 0., 0.)
            let pyz = snd (yz.Intersect(ray))
            let fromV = prevHit.HitPoint - oyz
            let toV = pyz - oyz
            angle <-  Ext3D.AngleBetween(fromV, toV)
            if (Vector3D.DotProduct(Ext3D.UnitX, Vector3D.CrossProduct(fromV, toV)) < 0.) then
                angle <- -angle
        else if(axis = Axis.Z) then
            let v = new Vector3D(0., 0., -1.)
            let xy = new Plane3D(v, prevHit.HitPoint.Z)
            let oxy = new Point3D(0., 0., prevHit.HitPoint.Z)
            let pxy = snd (xy.Intersect(ray))
            let fromV = prevHit.HitPoint - oxy
            let toV = pxy - oxy
            angle <- Ext3D.AngleBetween(fromV, toV)
            if (Vector3D.DotProduct(Ext3D.UnitZ, Vector3D.CrossProduct(fromV, toV)) < 0.) then
                angle <- -angle
        else if(axis = Axis.Y) then
            let v = new Vector3D(0., -1., 0.)
            let zx = new Plane3D(v, prevHit.HitPoint.Y)
            let ozx = new Point3D(0., prevHit.HitPoint.Y, 0.)
            let pzx = snd(zx.Intersect(ray)) 
            let fromV = prevHit.HitPoint - ozx
            let toV = pzx - ozx
            angle <- Ext3D.AngleBetween(fromV, toV)
            if (Vector3D.DotProduct(Ext3D.UnitY, Vector3D.CrossProduct(fromV, toV)) < 0.) then
                angle <- -angle

        angle
         

    member x.Select(cubicle:Cubicle , axis:Axis)=
        let basicOp = CubeOperation.GetBasicOp(cubicle.Name, axis).Op
        let cs = { SelectMode = SelectMode.Cubies; SelectedCubies = CubeOperation.BasicOps.[basicOp].CubicleGroup.Select(fun c -> _cubicles.[c].Cubie.Value)}
        (cs,basicOp)



    
    //private Animator _animator;

    //public Animator Animator { get { return _animator; } }
//    private int _steps;
//    private string _sequence = string.Empty;
    member x.Random() =
        if (x.Unfolded=false)then
            _steps <- 0
            _sequence <- string.Empty;
            let ops = CubeOperation.GetRandomOps(10)
            for op in ops do
                x.Rotate(op)


    member x.Rotate(op:string)=
        x.Rotate(op, 0., true, 0., false)



    

    /// <summary>
    /// startPos: 
    /// </summary>
    /// <param name="op"></param>
    /// <param name="duration"></param>
    /// <param name="silent"></param>
    /// <param name="startPos"></param>
    member x.Rotate(op:string , frames:double , silent:bool, rotatedAngle:double, fromInteraction:bool) =
        if (string.IsNullOrEmpty(op)=false)then
            if (fromInteraction || x.InTrack=false) then
                _animator.Start(Some(new AnimContext(rotatedAngle, frames, op.ToUpper(), silent, new List<Transform>())))
            //_animator.Start(new AnimContext(rotatedAngle, frames, op.ToUpper(), silent, new List<Transform>()));

    member x.InTrack with get() = Option.isSome(_prevHit)
    member x.StartTrack(pt:Point)=
        if (Option.isSome(_prevHit))then
            x.EndTrack()

        if (x.NoOp && Option.isNone(_prevHit)) then
            let hitTest = x.HitTest(pt, true)
            let hit = Option.isSome(hitTest.Distance)
            let result = hitTest.HitResult// snd hitTest
            if (hit && Option.isSome(result)) then
                _prevHit <- result
                _angle <- None
            
    member x.Track(p:Point)=
        if (Option.isSome(_prevHit) && x.NoOp) then
            if (Option.isNone(_angle)) then
                let testAngle = x.TestAngle(p, _prevHit.Value)
                _angle <- fst testAngle
                _axis <- snd testAngle
                //_angle <- x.TestAngle(p, _prevHit, out _axis);
                let select = x.Select(_prevHit.Value.HitCubicle.Value, _axis)
                _selected <- Some (fst select)
                _basicOp <- snd select
                //_selected = x.Select(_prevHit.HitCubicle, _axis, out _basicOp);
                
                if (_selected.Value.SelectMode = SelectMode.Cubies) then
                    for cubie in _selected.Value.SelectedCubies do cubie.Save()
                else
                    x.Save()

            if (Option.isSome(_angle) && (_selected.Value.SelectedCubies <> null || _selected.Value.SelectMode = SelectMode.Cube)) then
                let angle = x.TestAngle(p, _prevHit.Value, _axis)
                if (System.Math.Abs(angle) < Angle.DegreesToRadians(90.))then
                    if (_selected.Value.SelectMode = SelectMode.Cubies) then
                        for cubie in _selected.Value.SelectedCubies do
                            cubie.Rotate(_axis, angle, true)
                    else
                        do x.Rotate(_axis, angle, true)
                    _angle <- Some angle


    member x.EndTrack()=
        if (x.NoOp && Option.isSome( _prevHit )) then
            if (Option.isSome(_angle )) then
                let angle = _angle.Value
                if (System.Math.Abs(angle) > Angle.DegreesToRadians(10.) && System.Math.Abs(angle) < Angle.DegreesToRadians(90.)) then
                    if (_selected.Value.SelectMode = SelectMode.Cubies) then
                        //finish OP
                        let dir = if (angle > 0.) then RotationDirection.CounterClockWise else RotationDirection.Clockwise
                        let op = CubeOperation.GetOp(_basicOp, dir)
                        if (string.IsNullOrEmpty(op) = false) then
                            x.Rotate(op, 20., false, angle, true)
                else
                    if (_selected.Value.SelectMode = SelectMode.Cubies) then
                        for cubie in _selected.Value.SelectedCubies do cubie.Restore()
                    else
                        x.Restore()
            _prevHit <- None
