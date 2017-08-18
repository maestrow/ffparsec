#load "references.fsx"
#load "../src/Pipes.fs"

open System
open Parsec
open Parsec.Types.ParserInfo
open Parsec.Combinators
open Parsec.Run
open Parsec.Logging
open Parsec.Visualizers
open Parsec.Pipes


let p1 = returnP 1
let p2 = returnP 2
let p3 = returnP 2

let p12 = p1 + p2 
let p = p12 + p3 => "ok"
let r = run p ""

match r with
| Ok (res, _, _) -> printfn "%A" (res ())
| _ -> ()
