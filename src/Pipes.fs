namespace Parsec.Pipes

open Parsec
open Parsec.Combinators

[<AutoOpen>]
module Implementation =


  type CapturedResult<'r> = 'r * bool

  let (+) (p1: Parser<'i, CapturedResult<int>, 'u>) (p2: Parser<'i, int, 'u>) =
    anonym <| fun i -> 
      let r1 = runParser p1 i
      let r2 = runParser p2 i
      match r1, r2 with
      | Error err, _ 
      | _, Error err -> Error err
      | Ok ((res1, capture), _,  _), Ok (res2, pos, state) -> Ok (res1 + res2, pos, state)


