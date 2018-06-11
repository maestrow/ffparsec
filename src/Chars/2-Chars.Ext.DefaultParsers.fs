namespace Parsec.Chars

open Parsec
open Parsec.Combinators
open Parsec.Chars

[<AutoOpen>]
module DefaultParsers = 

  [<AllowNullLiteral>]
  type DefaultParserOf<'input>() = 
    static member inline (%!!~~%) (_: DefaultParser, _: DefaultParserOf<int>) = intP
    static member inline (%!!~~%) (_ : DefaultParser, _: DefaultParserOf< ^x >) =
        (^x : (static member get_DefaultParser : unit -> Parser<char, ^x, unit>)())
  
  and DefaultParser = DefaultParser with
    static member inline (%!!~~%) (DefaultParser, ch: char) = take ch
    static member inline (%!!~~%) (DefaultParser, s: string) = str s

  let p<'i> = null: DefaultParserOf<'i>

  let inline (~%) x = DefaultParser %!!~~% x

