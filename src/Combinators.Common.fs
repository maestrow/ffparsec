namespace Parsec.Combinators

[<AutoOpen>]
module Common = 
  
  open Parsec
  open Logical
  open Quantifiers

  let between p1 p2 p3 = 
    p1 >>. p2 .>> p3 

  /// Parses one or more occurrences of p separated by sep
  let sepBy1 p sep =
    let sepThenP = sep >>. p            
    p .>>. many sepThenP 
    |>> fun (p,pList) -> p::pList

  /// Parses zero or more occurrences of p separated by sep
  let sepBy p sep =
    sepBy1 p sep <|> returnp []

  // let attemt p = 
  //   fun input -> 
  //     let result = runParser p input
  //     match result with
  //       | Fail err -> 
  //           input.SuccessState (input)
  //       | _ -> result
