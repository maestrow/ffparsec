[<AutoOpen>]
module Parsec.Primitives

open System
open Parsec
open Parsec.Combinators.Core
open Parsec.Combinators.Logical

/// Parse single item
[<Description("Parse single item")>]
let pitem item = 
  (fun (input: Input<'i,'u>) -> 
    let i = input.CurrentItem
    if i = item then
      input.SuccessResult (item, 1)
    else
      Fail (sprintf "Unexpected: %A" i))
  |> parser
  |> withParams [("item", box item)]

// Match a sequence of items
[<Description("Match a sequence of items")>]
let pseq s =
  (s |> Seq.map pitem |> List.ofSeq |> sequence)
  |> describe
  |> withParams [("sequence", box s)]

