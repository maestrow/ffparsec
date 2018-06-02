(*
  A template for playing with parsees library. Copy it and do your experiments.
  -load - using #load
*)
#load "references.fsx"

open Parsec
open Parsec.Combinators
open Parsec.Run
open Parsec.Logging
open Parsec.Visualizers
open Parsec.Primitives

let p = repeatAtLeast 2 (take 'a')
let r = run p "a"

printfn "%A" r
