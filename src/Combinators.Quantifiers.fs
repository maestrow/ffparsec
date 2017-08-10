namespace Parsec.Combinators

[<AutoOpen>]
module Quantifiers = 
  
  open Parsec
  open Logical

  /// (helper) match zero or more occurences of the specified parser
  let rec private parseZeroOrMore parser result input =
    let parseResult = runParser parser input
    match parseResult with
    | Error err -> 
      (result |> List.rev, input.Position, input.UserState)  
    | Ok (value, _, _) -> 
        parseResult 
        |> input.UpdateState
        |> parseZeroOrMore parser (value::result)

  /// Match zero or more occurences of the specified parser
  let many p = 
    (fun input ->
      Ok (parseZeroOrMore p [] input))
    |> parser "many" "Match zero or more occurences of the specified parser"
    |> withParams [("p", box p)]


  /// Match one or more occurences of the specified parser
  let many1 p =
    (fun input ->
      let firstResult = runParser p input 
      match firstResult with
      | Error err -> Error err // failed
      | Ok (value, _, _) -> 
          Ok (firstResult 
          |> input.UpdateState
          |> parseZeroOrMore p [value]))
    |> parser "many1" "Match one or more occurences of the specified parser"
    |> withParams [("p", box p)]


  /// Parses an optional occurrence of p and returns an option value.
  let opt p = 
    let some = p |>> Some
    let none = returnP None
    some <|> none
    |> describe "opt" "Parses an optional occurrence of p and returns an option value"
    |> withParams [("p", box p)]