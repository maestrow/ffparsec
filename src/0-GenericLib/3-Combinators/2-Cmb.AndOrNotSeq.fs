/// And, Or, Not parsers and parser sequences
[<AutoOpen>]
module Parsec.Combinators.AndOrNotSeq
  
open Parsec

/// Combine two parsers as "A andThen B"
let andThen p1 p2 = 
  p1 >>= (fun r1 -> 
  p2 >>= (fun r2 -> returnP (r1, r2)))
  |> describe "andThen" "Combine two parsers as A andThen B"
  |> withParams [
      "p1", box p1
      "p2", box p2 ] 


/// Infix version of andThen
let (.>>.) = andThen

let (.>>) p1 p2 = p1 .>>. p2 |>> fst

let (>>.) p1 p2 = p1 .>>. p2 |>> snd

/// Combine two parsers as A orElse B
let orElse p1 p2 =
  (fun input ->
    let result1 = runParser p1 input
    match result1 with
    | Ok _ -> result1
    | Error err -> 
        let result2 = runParser p2 input
        result2)
  |> parser "orElse" "Combine two parsers as A orElse B"
  |> withParams [
      "p1", box p1
      "p2", box p2 ]

// infix version of orElse
let (<|>) = orElse

/// choose any of a list of parsers
let choice listOfParsers = 
  listOfParsers 
  |> List.reduce (<|>)

/// Process the list of parsers
let sequence parserList =
  let rec sequenceFn parserList = 
    let cons head tail = head::tail
    let consP = lift2 cons
    match parserList with
    | [] -> returnP []
    | head::tail -> consP head (sequenceFn tail)
  sequenceFn parserList
  |> describe "sequence" "Process the list of parsers"
  |> withParams (parserList |> List.map (fun p -> "p", box p))

let notP p = 
  (fun input -> 
    let result = runParser p input
    match result with
    | Ok _ -> Error "Match occured in NOT parser"
    | Error _ -> input.SuccessEmpty)
  |> parser "notP" "NOT parser"
  |> withParams [("p", box p)]
  
