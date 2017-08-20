namespace Parsec.Primitives

open System
open Parsec
open Parsec.Combinators
open Parsec.Primitives

/// Parsing on char sequence as input stream
module Chars =

  let str (s: seq<char>) = seqOf s |>> String.Concat

  let tab () = take '\t'

  let newLine () = choice [skip '\n'; skip '\r' .>>. skip '\n' |>> ignore; skip '\r'] |>> (fun i -> '\n')

  let spaces () = returnP () //ToDo

  let spaces1 () = returnP ()//ToDo

  let spacesNl () = returnP ()//ToDo

  let spacesNl1 () = returnP ()//ToDo


  // === Numbers

  let digit () = returnP ()//ToDo

  let intP () = returnP ()//ToDo