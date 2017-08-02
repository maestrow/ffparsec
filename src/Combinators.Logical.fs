namespace Parsec.Combinators

module Logical = 
  
  open Parsec

  /// Combine two parsers as "A andThen B"
  [<Description("Combine two parsers as A andThen B")>]
  let andThen p1 p2 = 
    p1 >>= (fun r1 -> 
    p2 >>= (fun r2 -> returnp (r1, r2)))
    |> describe
    |> withParams [
        "p1", box p1
        "p2", box p2 ] 


  /// Infix version of andThen
  let (.>>.) = andThen

  let (.>>) p1 p2 = p1 .>>. p2 |>> fst

  let (>>.) p1 p2 = p1 .>>. p2 |>> snd

  /// Combine two parsers as A orElse B
  [<Description("Combine two parsers as A orElse B")>]
  let orElse p1 p2 =
    (fun input ->
      let result1 = runParser p1 input
      match result1 with
      | Success _ -> result1
      | Fail err -> 
          let result2 = runParser p2 input
          result2)
    |> parser
    |> withParams [
        "p1", box p1
        "p2", box p2 ]

  // infix version of orElse
  let (<|>) = orElse

  /// choose any of a list of parsers
  let choice listOfParsers = 
    listOfParsers 
    |> List.reduce (<|>)

  let rec private sequenceFn parserList = 
    let cons head tail = head::tail
    let consP = lift2 cons
    match parserList with
    | [] -> returnp []
    | head::tail -> consP head (sequenceFn tail)

  /// Process the list of parsers
  [<Description("Process the list of parsers")>]
  let sequence parserList =
    sequenceFn parserList
    |> describe
    |> withParams (parserList |> List.map (fun p -> "p", box p))
