[<AutoOpen>]
module Parsec.Primitives

open System
open Parsec
open Parsec.Combinators.Core
open Parsec.Combinators.Logical

/// Parse single item
[<Description("Parse single item")>]
let pitem f = 
  (fun (input: Input<'i,'u>) -> 
    let i = input.CurrentItem
    if f i then
      input.SuccessResult (i, 1)
    else
      Fail (sprintf "Unexpected: %A" i))
  |> parser

// Match a sequence of items
[<Description("Match a sequence of items")>]
let pseq (f: 'a -> 'a -> bool) s =
  (s |> Seq.map (f >> pitem) |> List.ofSeq |> sequence)
  |> describe
  |> withParams [("sequence", box s)]

