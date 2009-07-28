//-----------------------------------------------------------------------------
// File:        Methods.fs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        07/21/2009
//-----------------------------------------------------------------------------

namespace Knoics

open Kit3D.Windows.Media.Media3D
open System.Windows
open Knoics.Math
open System

[<System.Runtime.CompilerServices.Extension>]
module Methods =
    type RotateCameraResult = {RotateMatrix:Matrix3D; ViewMatrix:Matrix3D}

    let CalculateViewMatrix(position:Point3D, lookDirection:Vector3D, upDirection:Vector3D) =
        let cameraZAxis = -lookDirection
        cameraZAxis.Normalize()
        let cameraXAxis = Vector3D.CrossProduct(upDirection, cameraZAxis)
        cameraXAxis.Normalize()
        let cameraYAxis = Vector3D.CrossProduct(cameraZAxis, cameraXAxis)
        let cameraPosition = new Vector3D(position.X, position.Y, position.Z)
        let offsetX = -Vector3D.DotProduct(cameraXAxis, cameraPosition)
        let offsetY = -Vector3D.DotProduct(cameraYAxis, cameraPosition)
        let offsetZ = -Vector3D.DotProduct(cameraZAxis, cameraPosition);
        new Matrix3D(cameraXAxis.X, cameraYAxis.X, cameraZAxis.X, 0.,
                                    cameraXAxis.Y, cameraYAxis.Y, cameraZAxis.Y, 0.,
                                    cameraXAxis.Z, cameraYAxis.Z, cameraZAxis.Z, 0.,
                                    offsetX, offsetY, offsetZ, 1.);


    [<System.Runtime.CompilerServices.Extension>]
    let  RotateCamera (this:PerspectiveCamera, axis:Vector3D, angle:double) =
        do axis.Normalize()
        let q = Quaternion3D.CreateFromAxisAngle(axis, angle)
        let m  = Ext3D.CreateFromQuaternion(q)
        
        let p = new Vector3D(this.Position.X, this.Position.Y, this.Position.Z)
        let pos = Ext3D.Transform(p, m)
        this.Position <- new Point3D(pos.X, pos.Y, pos.Z);
        
        let d = new Vector3D(this.LookDirection.X, this.LookDirection.Y, this.LookDirection.Z)
        this.LookDirection <- Ext3D.Transform(d, m) 
        let up = new Vector3D(this.UpDirection.X, this.UpDirection.Y, this.UpDirection.Z)
        this.UpDirection <- Ext3D.Transform(up, m)
        {RotateMatrix = m;  ViewMatrix = CalculateViewMatrix(this.Position, this.LookDirection, this.UpDirection)}
            

    [<System.Runtime.CompilerServices.Extension>]
    let  ProjectToTrackball (point:Point,  width:double , height:double) =
        let x = point.X / (width / 2.0) - 1.0;    
        let y = 1.0 - point.Y / (height / 2.0);
        let z2 = 1.0 - x * x - y * y;       // z^2 = 1 - x^2 - y^2
        let z = if z2 > 0.0 then Math.Sqrt(z2) else 0.0;
        new Vector3D(x, y, z);
        
    
    