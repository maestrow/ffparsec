namespace Parsec.Combinators

module Logical = 
  
  open Parsec

  /// Combine two parsers as "A andThen B"
  let andThen p1 p2 =         
    p1 >>= (fun p1Result -> 
    p2 >>= (fun p2Result -> 
      returnP (p1Result, p2Result) ))

  /// Infix version of andThen
  let (.>>.) = andThen

  let (.>>) p1 p2 = 
    p1 .>>. p2 
    |> mapP fst

  let (>>.) p1 p2 = 
    p1 .>>. p2 
    |> mapP snd

  let orElse p1 p2 =
    fun input ->
      let result1 = runParser p1 input
      match result1 with
      | Success _ -> result1
      | Fail err -> 
        let result2 = runParser p2 input
        result2 

  // infix version of orElse
  let (<|>) = orElse

  /// choose any of a list of parsers
  let choice listOfParsers = 
    List.reduce (<|>) listOfParsers 

  /// Process the list of parsers
  let rec sequence parserList =
    let cons head tail = head::tail
    let consP = lift2 cons
    match parserList with
    | [] -> returnP []
    | head::tail ->
      consP head (sequence tail)