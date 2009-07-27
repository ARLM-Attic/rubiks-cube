//-----------------------------------------------------------------------------
// File:        CubeOperation.fs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        07/25/2009
//-----------------------------------------------------------------------------

namespace Knoics.RubiksCube
open System;
open System.Collections.Generic;
open System.Linq;
open Knoics.Math;


type CubeOperation() =
    static let _PiOver2 = System.Math.PI/2.0
    static let _Pi = System.Math.PI
    //Cubicle Selection --> Basic Op Map
    //UB, UF, DF, DB -> LM
    //UL, UR, DR, DL -> FM
    //LB, RB, RF, LF -> UM

    //UFL-U
    //URF-F
    //UBR-R
    //ULB-B

    //DLF-D
    //DFR-R
    //DRB-B
    //DBL-L
    static let BasicOpFromAxis = new Dictionary<Axis, string[]>()
    static do BasicOpFromAxis.Add(Axis.X, [| "L"; "R" |])
    static do BasicOpFromAxis.Add(Axis.Y, [| "U"; "D" |])
    static do BasicOpFromAxis.Add(Axis.Z, [| "F"; "B" |])
        

    static let _equivalentOp = Dictionary<string, string>()
    static do _equivalentOp.Add("RM", "LM'")
    static do _equivalentOp.Add("RM'", "LM")
    static do _equivalentOp.Add("BM", "FM'")
    static do _equivalentOp.Add("BM'", "FM")
    static do _equivalentOp.Add("DM", "UM'")
    static do _equivalentOp.Add("DM'", "UM")
    
    static let _basicOps = new Dictionary<string, BasicOp>()


    static do _basicOps.Add("U", new BasicOp ("U", [| [| "ULB"; "UBR"; "URF"; "UFL" |]; [| "UB"; "UR"; "UF"; "UL" |]; [| "U" |] |], Axis.Y, -_PiOver2 ))
    static do _basicOps.Add("D", new BasicOp ("D", [| [| "DLF"; "DFR"; "DRB"; "DBL" |]; [| "DF"; "DR"; "DB"; "DL" |]; [| "D" |] |], Axis.Y, _PiOver2 ))
    static do _basicOps.Add("F", new BasicOp ("F", [| [| "UFL"; "RFU";"DFR"; "LFD" |]; [| "UF"; "RF"; "DF"; "LF" |]; [| "F" |] |], Axis.Z, -_PiOver2 ))
    static do _basicOps.Add("B", new BasicOp ("B", [| [| "UBR"; "LBU"; "DBL"; "RBD" |]; [| "RB"; "UB"; "LB"; "DB" |]; [| "B" |] |], Axis.Z, _PiOver2 ))
    static do _basicOps.Add("L", new BasicOp ("L", [| [| "ULB"; "FLU"; "DLF"; "BLD" |]; [| "UL"; "FL"; "DL"; "BL" |]; [| "L" |] |], Axis.X, _PiOver2 ))
    static do _basicOps.Add("R", new BasicOp ("R", [| [| "URF"; "BRU"; "DRB"; "FRD" |]; [| "UR"; "BR"; "DR"; "FR" |]; [| "R" |] |], Axis.X, -_PiOver2 ))
    static do _basicOps.Add("UM", new BasicOp ("UM", [| [| |]; [| "LB"; "BR"; "RF"; "FL" |]; [| "B"; "R"; "F"; "L" |] |], Axis.Y, -_PiOver2 ))
    static do _basicOps.Add("FM", new BasicOp ("FM", [| [| |]; [| "UR"; "RD"; "DL"; "LU" |]; [| "U"; "R"; "D"; "L" |] |], Axis.Z, -_PiOver2 ))
    static do _basicOps.Add("LM", new BasicOp ("LM", [| [| |]; [| "UF"; "FD"; "DB"; "BU" |]; [| "U"; "F"; "D"; "B" |] |], Axis.X, _PiOver2 ))
        
    

    static let _totalOps = [| 
        new OpFromBasic("U", "U", RotationDirection.Clockwise);
        new OpFromBasic("U'", "U", RotationDirection.CounterClockWise);
        new OpFromBasic( "D", "D", RotationDirection.CounterClockWise);
        new OpFromBasic( "D'", "D", RotationDirection.Clockwise);
        new OpFromBasic( "F", "F", RotationDirection.Clockwise);
        new OpFromBasic( "F'", "F", RotationDirection.CounterClockWise);
        new OpFromBasic( "B", "B", RotationDirection.CounterClockWise);
        new OpFromBasic( "B'", "B", RotationDirection.Clockwise);
        new OpFromBasic( "L", "L", RotationDirection.CounterClockWise);
        new OpFromBasic( "L'", "L", RotationDirection.Clockwise);
        new OpFromBasic( "R", "R", RotationDirection.Clockwise);
        new OpFromBasic( "R'", "R", RotationDirection.CounterClockWise);

        new OpFromBasic( "UM", "UM", RotationDirection.Clockwise);
        new OpFromBasic( "UM'", "UM", RotationDirection.CounterClockWise);
        new OpFromBasic( "FM", "FM", RotationDirection.Clockwise);
        new OpFromBasic( "FM'", "FM", RotationDirection.CounterClockWise);
        new OpFromBasic( "LM", "LM", RotationDirection.CounterClockWise);
        new OpFromBasic( "LM'", "LM", RotationDirection.Clockwise);
        new OpFromBasic( "RM", "LM", RotationDirection.Clockwise);
        new OpFromBasic( "RM'", "LM", RotationDirection.CounterClockWise);
        new OpFromBasic( "BM", "FM", RotationDirection.CounterClockWise);
        new OpFromBasic( "BM'", "FM", RotationDirection.Clockwise);
        new OpFromBasic( "DM", "UM", RotationDirection.CounterClockWise);
        new OpFromBasic( "DM'", "UM", RotationDirection.Clockwise)|]
        
    static member UndoOp = "X";
    static member FoldOp = "FOLD"
    static member UnFoldOp = "UNFOLD"
    
    static member PiOver2 = _PiOver2
    static member Pi = _Pi

    static member IsReverse(op:string) =
        op.Substring(op.Length - 1) = "'"

    static member IsReverse(op1:string, op2:string) =
        op1.Length >= 1 && op2.Length >= 1 && (op1 + "'" = op2 || op2 + "'" = op1)
    

    static member GetRandomOps(numOp:int) =
        let random = new Random()
        [|for i in 0..numOp-1 -> _totalOps.[random.Next(0, _totalOps.Length - 1)].Op|]
    
    
    static member BasicOps = _basicOps
    static member GetBasicOp(cubicleName:string, axis:Axis) = 
        let ops = _basicOps.Values |> Seq.filter(fun op -> op.Axis = axis)
        try
            ops |> Seq.find(fun (op) -> cubicleName.Contains(op.Op)) 
        with
            | :? KeyNotFoundException as ex -> ops |> Seq.find (fun (op:BasicOp) -> op.Op.Contains("M"))

    static member GetBasicOp(op:string) =
        let newOp = if (_equivalentOp.ContainsKey(op)) then _equivalentOp.[op] else op
        let isReverse = CubeOperation.IsReverse(newOp)
        let basicOpKey = if isReverse then newOp.Substring(0, newOp.Length - 1) else newOp
        (_basicOps.[basicOpKey], isReverse)
        

    static member IsValidOp(op:string) =
        _totalOps |> Array.exists (fun o -> o.Op = op)
    

    static member FetchOp(commandSeq:string, fromTail:bool) =
        
        if (string.IsNullOrEmpty(commandSeq)) then (string.Empty, commandSeq)
        else
            let rec findop len =
                if len > commandSeq.Length then len
                else
                    let searchOp = commandSeq.Substring((if fromTail then commandSeq.Length - len else 0), len)
                    if (CubeOperation.IsValidOp(searchOp)||(fromTail&&(searchOp="'"||searchOp="M"||searchOp="M'"))||(fromTail=false&&searchOp="X")) && len <= commandSeq.Length then 
                        findop (len + 1)
                    else len
                (*    
            let mutable len = 1
            let totalLen = commandSeq.Length
            while (len <= totalLen) do
                let searchOp = commandSeq.Substring((if fromTail then totalLen - len else 0), len)
                if (CubeOperation.IsValidOp(searchOp)||(fromTail&&(searchOp="'"||searchOp="M"||searchOp="M'"))||(fromTail=false&&searchOp="X")) then
                    action <- searchOp
                    len <- len+1
                else do break
            *)
            let len = findop 1
            let xlen = if (len = 1) then len else len - 1
            let action = commandSeq.Substring(0, xlen)
            let startIndex =    if fromTail then 0 else xlen
            let remainingSeq = commandSeq.Substring(startIndex, commandSeq.Length - xlen)
            (action, remainingSeq)


    static member GetReverseOp(op:string) =
        if(CubeOperation.IsReverse(op)) then op.Substring(0, op.Length - 1) else op + "'";


    static member GetOp(basicOp:string, dir:RotationDirection) =
        try
             _totalOps.FirstOrDefault(fun o -> o.BasicOp = basicOp && o.RotationDirectionWithAxis = dir).Op
        with | :? KeyNotFoundException as ex -> string.Empty 
        

