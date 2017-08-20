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
let p3 = returnP 3

let p = p1 ++ p2 ++ p3 ++ p1 => fun a b c -> a + b + c
let r = run p ""

