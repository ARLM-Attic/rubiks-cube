//-----------------------------------------------------------------------------
// File:        Ext3D.fs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        07/21/2009
//-----------------------------------------------------------------------------
namespace Knoics.Math

open System.Windows
open Kit3D.Windows.Media.Media3D
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
        
        
[<Struct>]
type Ray3D(o:Point3D, d:Vector3D)= 
    member x.Origin = o
    member x.Direction = d
    
[<Struct>]
type Quaternion3D(x:double, y:double, z:double, w:double) = 
    static let identity:Quaternion3D = new Quaternion3D(0.0, 0.0, 0.0, 1.0)
    member this.X : double = x
    member this.Y : double = y
    member this.Z : double = z
    member this.W : double = w
    
    static member Identity = identity

    static member CreateFromAxisAngle(axis:Vector3D, angle:double) =
        let a = angle * 0.5
        let sina = System.Math.Sin(a)
        let cosa = System.Math.Cos(a)
        let quaternion = new Quaternion3D(axis.X * sina, axis.Y * sina, axis.Z * sina, cosa)
        quaternion

    static member (*) (quaternion1:Quaternion3D, quaternion2:Quaternion3D) =
        let x1 = quaternion1.X
        let y1 = quaternion1.Y
        let z1 = quaternion1.Z
        let w1 = quaternion1.W
        let x2 = quaternion2.X
        let y2 = quaternion2.Y
        let z2 = quaternion2.Z
        let w2 = quaternion2.W
        let x3 = (y1 * z2) - (z1 * y2)
        let y3 = (z1 * x2) - (x1 * z2)
        let z3 = (x1 * y2) - (y1 * x2)
        let w3 = ((x1 * x2) + (y1 * y2)) + (z1 * z2)
        
        let x = ((x1 * w2) + (x2 * w1)) + x3
        let y = ((y1 * w2) + (y2 * w1)) + y3
        let z = ((z1 * w2) + (z2 * w1)) + z3
        let w = (w1 * w2) - w3
        new Quaternion3D(x, y, z, w)

type BoundingBox3D(min:Vector3D, max:Vector3D) as this = 
    let fmin a b =
        if(a>b) then b
        else a
    let fmax a b =
        if(a<b) then b
        else a
    let intersect(rayDir:double, rayOrigin, boxMin, boxMax, maxV, d) =
        if (System.Math.Abs(rayDir) < 1E-06 && ((rayOrigin < boxMin) || (rayOrigin > boxMax))) then (None, double.MaxValue)
        else 
            let vx = 1.0 / rayDir;
            let t1 = (boxMin - rayOrigin) * vx
            let t2 = (boxMax - rayOrigin) * vx
            let tmin = fmin t1 t2
            let tmax = fmax t1 t2
            let dis = fmax tmin d
            let maxValue = fmin maxV tmax// if (t2 > maxValue) then maxValue else t2
            if(dis > maxValue) then (None, maxValue)
            else (Some(dis), maxValue)
            
    member x.Min = min
    member x.Max = max

    member x.Intersects(ray:Ray3D) = 
        let distancex = intersect(ray.Direction.X, ray.Origin.X, this.Min.X, this.Max.X, double.MaxValue, 0.)
        if(fst distancex = None) then None
        else 
            let dx = Option.get (fst distancex)
            let distancey = intersect(ray.Direction.Y, ray.Origin.Y, this.Min.Y, this.Max.Y, snd distancex, dx)
            if(fst distancey = None) then None
            else
                let dy = Option.get (fst distancey)
                let distancez = intersect(ray.Direction.Z, ray.Origin.Z, this.Min.Z, this.Max.Z, snd distancey, dy)
                if(fst distancez = None) then None
                else fst distancez

            
type Plane3D(normal:Vector3D, d:double) as this = 
    do normal.Normalize()
    member x.D  = d
    member x.Normal = normal

    new (p0:Point3D, p1:Point3D, p2:Point3D) =
        let v1 = p1 - p0
        let v2 = p2 - p0
        let normal = Vector3D.CrossProduct(v1, v2) 
        let d  = -(normal.X * p0.X + normal.Y * p0.Y + normal.Z * p0.Z)
        new Plane3D(normal, d)


    member x.Intersects(ray:Ray3D) =
        let num2 = ((this.Normal.X * ray.Direction.X) + (this.Normal.Y * ray.Direction.Y)) + (this.Normal.Z * ray.Direction.Z);
        if (System.Math.Abs(num2) < 1E-12) then None
        else
            let num3 = ((this.Normal.X * ray.Origin.X) + (this.Normal.Y * ray.Origin.Y)) + (this.Normal.Z * ray.Origin.Z)
            let d:double = (-this.D - num3) / num2;
            if (d < -1E-12) then None
            else if (d < 0.) then
                Some(0.)
            else 
                Some(d)

    member x.Intersect(ray:Ray3D) = 
        let d = this.Intersects(ray)
        let mutable intersectionPoint = new Point3D(double.NaN, double.NaN, double.NaN);
        if (d.IsSome) then
            intersectionPoint <- ray.Origin + d.Value * ray.Direction
        (d.IsSome, intersectionPoint)



type Ext3D() =
    static let _unitX:Vector3D = new Vector3D(1., 0., 0.)
    static let _unitY:Vector3D = new Vector3D(0., 1., 0.)
    static let _unitZ:Vector3D  = new Vector3D(0., 0., 1.)

    static member UnitX = _unitX
    static member UnitY = _unitY
    static member UnitZ = _unitZ


    ///to radian
    static member AngleBetween(vector1:Vector3D, vector2:Vector3D) =
        do vector1.Normalize()
        do vector2.Normalize()
        if (Vector3D.DotProduct(vector1, vector2) < 0.0) then
            let vectord2 = -vector1 - vector2
            let r = 3.1415926535897931 - (2.0 * System.Math.Asin(vectord2.Length / 2.0))
            r
            //57.295779513082323 * r//RadiansToDegrees(r)
        else
            let vectord = vector1 - vector2
            let r = 2.0 * System.Math.Asin(vectord.Length / 2.0)
            r
            //57.295779513082323 * r//RadiansToDegrees(r)



    static member Transform(position:Vector3D, matrix:Matrix3D) =
        let x = (((position.X * matrix.M11) + (position.Y * matrix.M21)) + (position.Z * matrix.M31)) + matrix.OffsetX
        let y = (((position.X * matrix.M12) + (position.Y * matrix.M22)) + (position.Z * matrix.M32)) + matrix.OffsetY
        let z = (((position.X * matrix.M13) + (position.Y * matrix.M23)) + (position.Z * matrix.M33)) + matrix.OffsetZ
        new Vector3D(x, y, z)

    static member CreateRotationX(radians:double) = 
        let cos = System.Math.Cos(radians)
        let sin = System.Math.Sin(radians)
        new Matrix3D(
            1., 
            0.,
            0.,
            0.,
            0.,
            cos,
            sin,
            0.,
            0.,
            -sin,
            cos,
            0.,
            0.,
            0.,
            0.,
            1.
        )
        

    static member CreateRotationY(radians:double) =
        let cos = System.Math.Cos(radians)
        let sin = System.Math.Sin(radians)
        new Matrix3D(
            cos,
            0.,
            -sin,
            0.,
            0.,
            1.,
            0.,
            0.,
            sin,
            0.,
            cos,
            0.,
            0.,
            0.,
            0.,
            1.
         )
        


    static member CreateRotationZ(radians:double) =
        let cos = System.Math.Cos(radians)
        let sin = System.Math.Sin(radians)
        new Matrix3D(
            cos,
            sin,
            0.,
            0.,
            -sin,
            cos,
            0.,
            0.,
            0.,
            0.,
            1., 
            0.,
            0.,
            0.,
            0.,
            1.
        )
        
    
    static member CreateFromQuaternion(quaternion:Quaternion3D) =
        let xx = quaternion.X * quaternion.X
        let yy = quaternion.Y * quaternion.Y
        let zz = quaternion.Z * quaternion.Z
        let xy = quaternion.X * quaternion.Y
        let zw = quaternion.Z * quaternion.W
        let zx = quaternion.Z * quaternion.X
        let yw = quaternion.Y * quaternion.W
        let yz = quaternion.Y * quaternion.Z
        let xw = quaternion.X * quaternion.W
        new Matrix3D(
            1. - (2. * (yy + zz)),
            2. * (xy + zw),
            2. * (zx - yw),
            0.,
            2. * (xy - zw),
            1. - (2. * (zz + xx)),
            2. * (yz + xw),
            0.,
            2. * (zx + yw),
            2. * (yz - xw),
            1. - (2. * (yy + xx)),
            0.,
            0.,
            0.,
            0.,
            1.
        )

    static member CreateTranslation(position:Vector3D) =
        new Matrix3D(
            1.,
            0.,
            0.,
            0.,
            0.,
            1.,
            0.,
            0.,
            0.,    
            0.,
            1.,
            0.,
            position.X,
            position.Y,
            position.Z,
            1.
        )
        


    static member Unproject(pt:Point , viewpoint:Point3D , worldToLocal:Matrix3D , viewToWorld:Matrix3D , screenToViewTransform:Matrix3D ) =
        //Matrix screenToLocal = worldToLocal * (viewToWorld * screenToViewTransform);
        let vs = new Vector3D(pt.X, pt.Y, 1.)
        let view = Ext3D.Transform(vs, screenToViewTransform)
        let world = Ext3D.Transform(view, viewToWorld)
        let toPt = Ext3D.Transform(world, worldToLocal)

        //Matrix worldToLocal = Matrix.Invert(world);
        let vp = new Vector3D(viewpoint.X, viewpoint.Y, viewpoint.Z)
        let fromPt = Ext3D.Transform(vp, worldToLocal)
        let d = toPt - fromPt 
        do d.Normalize()
        let fromPtPoint = new Point3D(fromPt.X, fromPt.Y, fromPt.Z)
        new Ray3D(fromPtPoint, d)

    