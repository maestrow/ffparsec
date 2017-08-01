module Parsec.Primitives

open System
open Parsec
open Parsec.Combinators.Core
open Parsec.Combinators.Logical

/// parse a char 
let pitem item = 
  fun (input: Input<'i,'u>) -> 
    let i = input.CurrentItem
    if i = item then
      input.SuccessResult (item, 1)
    else
      Fail (sprintf "Unexpected: %A" i)

/// Helper to create a string from a list of chars
let private charListToStr charList = 
  String(List.toArray charList)

// match a sequence of items
let pseq s = s |> Seq.map pitem |> List.ofSeq |> sequence