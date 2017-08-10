[<AutoOpen>]
module Parsec.Run

open Parsec
open Parsec.Types.ParserInfo
open Parsec.Types

type DummyDebugLogger () =
  interface IDebugLogger with
    member x.Position with get () = 0 and set (value) = ()
    member x.LevelDown () = ()
    member x.LevelUp () = ()
    member x.Push p = ()
    member x.SaveResult result = ()

type Input<'Item, 'UserState> with
  static member FromString (str: string, state: 'u) = 
    { InputStream = str
      Position = 0
      UserState = state
      DebugLogger = DummyDebugLogger() }

  static member FromString (str: string) = 
    Input<string, unit>.FromString (str, ())

let runParser (p: Parser<'i,'r,'u>) (input: Input<'i,'u>) = 
  let di = input.DebugLogger
  di.Push p
  di.LevelDown ()
  let result = p.Fn input
  //printfn "%s[%i]: %A" p.Info.Name input.Position result
  di.LevelUp ()
  di.SaveResult result
  result


let runWithLogger p stream state logger = 
  {
    InputStream = stream
    Position = 0
    UserState = state
    DebugLogger = logger
  }
  |> runParser p


let runParserOnString (p: Parser<char,'r,'u>) (inputStr: string) (state: 'u) = 
  let input = Input.FromString (inputStr, state)
  runParser p input

let run (p: Parser<char,'r, unit>) (input: string) = 
  runParserOnString p input ()

let printResult (result: ParseResult<'Result, 'UserState>) =
  match result with
  | Ok (value, pos, state) -> 
      printfn "%A" value
      printfn "Position: %i" pos
      printfn "State: %A" state
  | Error error -> 
      printfn "Error: %s" error


