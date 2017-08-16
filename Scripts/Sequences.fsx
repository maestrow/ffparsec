#load "references.fsx"
#load "../src/Sequences.fs"

open System
open Parsec
open Parsec.Types.ParserInfo
open Parsec.Combinators
open Parsec.Run
open Parsec.Logging
open Parsec.Visualizers
//open Parsec.Sequences


type CapturedResult<'r> = 'r * bool

let (+) (p1: Parser<'i, CapturedResult<int>, 'u>) (p2: Parser<'i, int, 'u>) =
  fun i -> 
    let r1 = runParser p1 i
    let r2 = runParser p2 i
    match r1, r2 with
    | Error err, _ 
    | _, Error err -> Error err
    | Ok ((res1, capture), _,  _), Ok (res2, pos, state) -> Ok (res1 + res2, pos, state)




let p1 : Parser<char, CapturedResult<int>, unit> = returnP (1, true)
let p2 : Parser<char, int, unit> = returnP 1
let p = p1 + p2

run p "dfg"