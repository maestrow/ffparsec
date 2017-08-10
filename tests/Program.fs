open System

open Expecto
open Swensen.Unquote

open Parsec
open Parsec.Types.ParserInfo
open Parsec.Combinators
open Parsec.Run
open Parsec.Logging
open Parsec.Visualizers
  
[<Tests>]
let tests = 
  testList "group1" [
    testCase "A simple test" <| fun () ->
      test <@ (1+2)/3 = 1 @>
      1 =! 1
    testCase "A simple test 2" <| fun () ->
      test <@ (1+2)/3 = 1 @>
      1 =! 1
  ]

[<EntryPoint>]
let main argv =
  runTestsInAssembly defaultConfig argv
