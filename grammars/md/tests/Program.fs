open System

open Expecto
open Swensen.Unquote

open Parsec
open Parsec.Types.ParserInfo
open Parsec.Combinators
open Parsec.Primitives
open Parsec.Run
open Parsec.Logging
open Parsec.Visualizers
open Parsec.Extensions.DefaultParsers
open Parsec.Extensions.Pipes
open Parsec.Extensions.Qty

[<Tests>]
let tests = 
  testList "Input, UpdateState" [
    testCase "It should be allowed to expose an incorrect position in Parse.Ok result" <| fun () ->
      // Why? If the parser succeeds on the last element, it can move the position further. And the next parser should fail if it expects the current element.
      let p: Parser<char, unit, unit> = 
        parser "p" ""
        <| fun input -> 
          Ok ((), -1, input.UserState) // set out of range position
      let logger = DebugLogger ()
      let input = createInput "" () logger
      let inputNew = input.UpdateState (runParser p input)
      input.Position =! 0
      inputNew.Position =! -1

[<EntryPoint>]
let main argv =
  runTestsInAssembly defaultConfig argv
