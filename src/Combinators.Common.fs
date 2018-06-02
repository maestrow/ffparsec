[<AutoOpen>]
module Parsec.Combinators.Common
  
open Parsec
open Quantifiers

let between p1 p2 p3 = 
  p1 >>. p2 .>> p3 

/// Parses one or more occurrences of p separated by sep
let sepBy1 p sep =
  let sepThenP = sep >>. p            
  p .>>. many sepThenP 
  |>> fun (p, pList) -> p::pList

/// Parses zero or more occurrences of p separated by sep
let sepBy p sep =
  sepBy1 p sep <|> returnP []

let attempt p = 
  fun input -> 
    let result = runParser p input
    match result with
    | Error err -> 
        input.SuccessEmpty
    | _ -> result

/// Returns parser result if it satisfies condition
let (>>?) p f = 
  let satisfy f r = 
    anonym <| fun (input: Input<'i,'u>) -> 
      match f r with
      | true -> input.SuccessResult r
      | _ -> Error (sprintf "Condition is false. Arg: %A" r)
  p >>= satisfy f
  |> describe ">>?" "Post-Condition (condition checked by function)"
  |> withParams [("p", box p); ("f", box f)]
