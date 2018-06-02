namespace Parsec.Chars

open System
open Parsec.Combinators

/// Parsing on char sequence as input stream
[<AutoOpen>]
module Primitives =

  let str (s: seq<char>) = seqOf s |>> String.Concat

  let tab () = take '\t'

  let newLine () = choice [skip '\n'; skip '\r' .>>. skip '\n' |>> ignore; skip '\r'] |>> (fun _ -> '\n')

  let spaces () = returnP () //ToDo

  let spaces1 () = returnP ()//ToDo

  let spacesNl () = returnP ()//ToDo

  let spacesNl1 () = returnP ()//ToDo


  // === Numbers

  let digit () = returnP ()//ToDo

  let intP () = returnP ()//ToDo