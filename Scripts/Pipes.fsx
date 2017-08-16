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


let p1 = returnP (1, true)
let p2 = returnP 1
let p = p1 + p2

let r = run p "dfg"

printResult r