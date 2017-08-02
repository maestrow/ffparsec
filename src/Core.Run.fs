[<AutoOpen>]
module Parsec.Run

open Parsec
open Parsec.Types.ParserInfo
open Parsec.Types
open Parsec.Core.Debugging

type Input<'Item, 'UserState> with
  static member FromString (str: string, state: 'u) = 
    { InputStream = str
      Position = 0
      UserState = state
      DebugInfo = new DebugInfo () }

  static member FromString (str: string) = 
    Input<string, unit>.FromString (str, ())

let runParser (p: Parser<'i,'r,'u>) (input: Input<'i,'u>) = 
  let di = input.DebugInfo
  di.Push p
  let pos = input.Position
  di.LevelDown ()
  let result = p.Fn input
  di.LevelUp ()
  di.SaveResult (toDebugResult result pos)
  result

let runParserOnString (p: Parser<char,'r,'u>) (inputStr: string) (state: 'u) = 
  let input = Input.FromString (inputStr, state)
  runParser p input

let run (p: Parser<char,'r, unit>) (input: string) = 
  runParserOnString p input ()

let printResult (result: ParseResult<'Result, 'UserState>) =
  match result with
  | Success (value, _, state) -> 
      printfn "%A" value
      printfn "State: %A" state
  | Fail error -> 
      printfn "Error parsing: %s" error


