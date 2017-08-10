#load "references.fsx"

open System
open Parsec
open Parsec.Types.ParserInfo
open Parsec.Combinators
open Parsec.Run
open Parsec.Logging
open Parsec.Visualizers

let run p stream = 
  let logger = DebugLogger ()
  runWithLogger p stream () logger, logger

let printDebug result = 
  let r = result |> snd |> (fun (i: DebugLogger) -> i.GetTree ()) |> ConsoleDebugLogVisualizer.serialize
  printfn "\n%s" r

let printRes result = 
  result |> fst |> printResult


///////////////////////////////////////////////////////////////////////////////////////////////////

