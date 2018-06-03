[<AutoOpen>]
module Parsec.Run

open Parsec

type Input<'Item, 'UserState> with
  static member FromString (str: string, state: 'u) = 
    { InputStream = str
      Position = 0
      UserState = state
      DebugLogger = None }

  static member FromString (str: string) = 
    Input<string, unit>.FromString (str, ())

let runParser (p: Parser<'i,'r,'u>) (input: Input<'i,'u>) = 
  match input.DebugLogger with
  | None -> 
    p.Fn input
  | Some dl -> 
    dl.Push p
    dl.LevelDown ()
    let result = p.Fn input
    //printfn "%s[%i]: %A" p.Info.Name input.Position result
    dl.LevelUp ()
    dl.SaveResult result
    result

let createInput stream state logger = 
  {
    InputStream = stream
    Position = 0
    UserState = state
    DebugLogger = Some logger
  }

let runWithLogger p stream state logger = 
  createInput stream state logger 
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


