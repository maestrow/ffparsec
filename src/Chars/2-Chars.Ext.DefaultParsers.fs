namespace Parsec.Primitives.Chars.Extensions

open Parsec
open Parsec.Combinators
open Parsec.Chars

module DefaultParsers = 
  
  type DefaultParser = DefaultParser with
    static member inline (%!!~~%) (DefaultParser, ch: char) = take ch
    static member inline (%!!~~%) (DefaultParser, s: string) = str s
    static member inline (%!!~~%) (DefaultParser, list: _ list) = 
      list |> List.map (fun p -> DefaultParser %!!~~% p) |> choice
  
  let inline (~%) x = DefaultParser %!!~~% x