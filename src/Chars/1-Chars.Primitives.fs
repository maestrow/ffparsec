namespace Parsec.Chars

open System
open Parsec
open Parsec.Combinators
open Parsec.Extensions

/// Parsing on char sequence as input stream
[<AutoOpen>]
module Primitives =

  let str (s: seq<char>) = seqOf s |>> String.Concat

  let tab () = take '\t'

  let newLine () = choice [skip '\n'; skip '\r' .>>. skip '\n' |>> ignore; skip '\r'] |>> (fun _ -> '\n')

  let spaces () = choice [take ' '; tab()] * qty.[0..] |>> ignore

  let spaces1 () = choice [take ' '; tab()] * qty.[1..] |>> ignore

  // Spaces allowing new line. Zero or more.
  let spacesNl () = choice [take ' '; tab(); newLine()] * qty.[0..] |>> ignore

  // Spaces allowing new line. One or more.
  let spacesNl1 () = choice [take ' '; tab(); newLine()] * qty.[1..] |>> ignore


  // === Numbers

  let digit () : Parser<char, char, 'a> = takeIf (int >> (fun i -> i > 47 && i < 58))

  let intP () = 
    digit() * qty.[1..] |>> (
      List.rev
      >> List.fold 
        (fun (mul, res) i -> mul*10, ((int i - 48) * mul + res))
        (1, 0)
      >> snd
    )