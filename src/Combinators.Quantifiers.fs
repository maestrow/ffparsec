namespace Parsec.Combinators

[<AutoOpen>]
module Quantifiers = 
  
  open Parsec
  open Logical

  /// (helper) match zero or more occurences of the specified parser
  let rec private parseZeroOrMore parser result input =
    let parseResult = runParser parser input
    match parseResult with
    | Fail err -> 
      (result |> List.rev, 0, input.UserState)  
    | Success (value, _, _) -> 
        parseResult 
        |> input.UpdateState
        |> parseZeroOrMore parser (value::result)

  /// Match zero or more occurences of the specified parser
  [<Description("Match zero or more occurences of the specified parser")>]
  let many p = 
    (fun input ->
      Success (parseZeroOrMore p [] input))
    |> parser
    |> withParams [("p", box p)]


  /// Match one or more occurences of the specified parser
  [<Description("Match one or more occurences of the specified parser")>]
  let many1 p =
    (fun input ->
      let firstResult = runParser p input 
      match firstResult with
      | Fail err -> Fail err // failed
      | Success (value, _, _) -> 
          Success (firstResult 
          |> input.UpdateState
          |> parseZeroOrMore p [value]))
    |> parser
    |> withParams [("p", box p)]


  /// Parses an optional occurrence of p and returns an option value.
  let opt p = 
    let some = p |>> Some
    let none = returnP None
    some <|> none
    |> describe
    |> withParams [("p", box p)]