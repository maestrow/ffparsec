namespace Parsec.Combinators

module Quantifiers = 
  
  open Parsec
  open Logical

  /// (helper) match zero or more occurences of the specified parser
  let rec private parseZeroOrMore parser result input =
    let parseResult = runParser parser input
    match parseResult with
    | Fail err -> 
      (result |> List.rev, 0, input.UserState)  
    | Success (value, consumed, state) -> 
        parseResult 
        |> input.UpdateState
        |> parseZeroOrMore parser (value::result)

  /// match zero or more occurences of the specified parser
  let many parser = 
    fun input ->
      Success (parseZeroOrMore parser [] input)

  /// match one or more occurences of the specified parser
  let many1 parser = 
    fun input state ->
      let firstResult = runParser parser input 
      match firstResult with
      | Fail err -> Fail err // failed
      | Success (value, consumed, state) -> 
          Success (firstResult 
          |> input.UpdateState
          |> parseZeroOrMore parser [value])

  /// Parses an optional occurrence of p and returns an option value.
  let opt p = 
    let some = p |>> Some
    let none = returnP None
    some <|> none