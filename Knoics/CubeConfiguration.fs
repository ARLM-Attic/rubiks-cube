//-----------------------------------------------------------------------------
// File:        CubeConfiguration.fs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        07/25/2009
//-----------------------------------------------------------------------------
namespace Knoics.RubiksCube
open System.Collections.Generic
open System
open System.Windows.Media
open Knoics.Math

type CubeConfiguration() =
    static let faces = new Dictionary<string, FaceConfig>()
    static do faces.Add("U", new FaceConfig(Colors.Red, "U", Axis.Y ))
    static do faces.Add("D", new FaceConfig(Colors.Orange, "D", Axis.Y ))
    static do faces.Add("L", new FaceConfig(Colors.Green, "L", Axis.X ))
    static do faces.Add("R", new FaceConfig(Colors.Blue, "R", Axis.X ))
    static do faces.Add("B", new FaceConfig(Colors.White, "B", Axis.Z ))
    static do faces.Add("F", new FaceConfig(Colors.Yellow, "F", Axis.Z ))
    
    
    
    static member Faces = faces
    

    //x--0:Left     Width-1 :Right
    //y--0:Down     Height-1:Up
    //z--0:Back     Depth-1 :Front
    //To make notation closewise, use pattern YXZ
    static member GetCubicleName(size:CubeSize, x:int , y:int , z:int ) =
        //StringBuilder sb = new StringBuilder();
        let mutable xface = true
        let mutable yface = true
        let mutable zface = true
        let mutable postive = true
        let mutable yFace = string.Empty
        let mutable xFace = string.Empty
        let mutable zFace = string.Empty
        if (y = 0) then
            yFace <- "D"
            postive <- not postive
        else if (y = size.Height - 1)then
            yFace <- "U"
        else if (size.Height > 3) then
            yface <- false
            yFace <- "D" + y.ToString()


        if (x = 0) then
            xFace <- "L"
            postive <- not postive
        else if (x = size.Width - 1) then
            xFace <- "R"
        else if (size.Width > 3) then
            xface <- false
            xFace <-  "L" + x.ToString()

        if (z = 0) then
            zFace <- "B"
            postive <- not postive
        else if (z = size.Depth - 1) then
            zFace <- "F"
        else if (size.Depth > 3) then
            zface <- false;
            zFace <- "B" + z.ToString()

        if (xface = false && yface = false && zface = false) then
            string.Empty
        else
            if (string.IsNullOrEmpty(xFace) = false && string.IsNullOrEmpty(yFace) = false && string.IsNullOrEmpty(zFace)=false) then
                if postive then yFace + xFace + zFace else yFace + zFace + xFace
            else
                yFace + xFace + zFace
        
