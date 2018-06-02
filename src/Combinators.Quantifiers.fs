[<AutoOpen>]
module Parsec.Combinators.Quantifiers
  
open Parsec

/// (helper) match zero or more occurences of the specified parser
let rec private parseZeroOrMore parser result maxCount input =
  let parseResult = runParser parser input
  match parseResult with
  | Error err -> 
    (result |> List.rev, input.Position, input.UserState)  
  | Ok (value, _, _) -> 
      let newInput = parseResult |> input.UpdateState
      match maxCount-1 with
      | 0 -> value::result |> List.rev, newInput.Position, newInput.UserState
      | _ -> parseZeroOrMore parser (value::result) (maxCount-1) newInput

let repeatExactly n p = List.init n (fun _ -> p) |> sequence

let repeatNoMore n p = 
  (fun input ->
    Ok (parseZeroOrMore p [] n input))
  |> parser "repeatNoMore" "Match specified parser from zero to n occurences"
  |> withParams [("n", box n); ("p", box p) ]

/// Match zero or more occurences of the specified parser
let many p = 
  repeatNoMore -1 p
  |> withName "many" 
  |> withDescr "Match zero or more occurences of the specified parser"
  |> withParams [("p", box p)]

let repeatAtLeast n p = repeatExactly n p .>>. many p |>> fun (a, b) -> a @ b

let repeatFromTo from till p = (repeatExactly from p) .>>. (repeatNoMore (till-from) p) |>> fun (a, b) -> a @ b

/// Match one or more occurences of the specified parser
let many1 p =
  (fun input ->
    let firstResult = runParser p input 
    match firstResult with
    | Error err -> Error err // failed
    | Ok (value, _, _) -> 
        Ok (firstResult 
        |> input.UpdateState
        |> parseZeroOrMore p [value] -1))
  |> parser "many1" "Match one or more occurences of the specified parser"
  |> withParams [("p", box p)]


/// Parses an optional occurrence of p and returns an option value.
let opt p = 
  let some = p |>> Some
  let none = returnP None
  some <|> none
  |> describe "opt" "Parses an optional occurrence of p and returns an option value"
  |> withParams [("p", box p)]

