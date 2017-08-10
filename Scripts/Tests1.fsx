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

let p1: Parser<char, string, unit> = pItemEq 'a' .>>. pItemEq '1' |>> (fun (a, b) -> [a;b] |> String.Concat)
let p2: Parser<char, string, unit> = (pItemEq 'b' .>>. pItemEq '2') |>> (fun (a, b) -> [a;b] |> String.Concat)

let p: Parser<char, string list, unit> = many (p1 <|> p2)
run p "a1b2b2a1a1b2XYZ" |> printRes
run p "a1b3a1" |> printRes