namespace Parsec.Primitives

open System
open Parsec
open Parsec.Combinators
open System.Runtime.CompilerServices

[<AutoOpen>]
module Chars =

  let str = seqOf

  let tab () = take '\t'

  let newLine () = choice [skip '\n'; skip '\r' .>>. skip '\n' |>> ignore; skip '\r'] |>> (fun i -> '\n')

  let spaces () = returnP () //ToDo

  let spaces1 () = returnP ()//ToDo

  let spacesNl () = returnP ()//ToDo

  let spacesNl1 () = returnP ()//ToDo
