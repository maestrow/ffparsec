#load "references.fsx"

open Parsec
open Parsec.Types.ParserInfo
open Parsec.Combinators
open Parsec.Run
open Parsec.Logging
open Parsec.Visualizers

let run p stream = 
  let logger = DebugLogger ()
  runWithLogger p stream () logger, logger

let print result = result |> snd |> (fun (i: DebugLogger) -> i.GetTree ()) |> ConsoleDebugLogVisualizer.serialize


///////////////////////////////////////////////////////////////////////////////////////////////////

// ParserDescription

// Logging

// Logical

// let ``test andThen`` () = ()
// let ``test orElse`` () = ()
// let ``test choice`` () = ()
// let ``test sequence`` () = ()
// let ``test not`` () = ()

// Quantifiers

// UserState

// Primitives

run (pCurrent ()) "a" |> print
// run (pItemEq 'a') "b" |> print
// run (pItemEq 'a') "" |> print
// run (pItemEq 'a' |> many) "aaa" |> print
// run (pItemEq 'a' |> many) "aaab" |> print

//let p0 = sequence [pItemEq 'a'; pItemEq 'b'; pItemEq 'c']
//run p0 "abc" |> print