module Tests.Chars.DefaultParsers

open Expecto
open Swensen.Unquote

open Parsec.Chars

open TestFixtures
open Tests.GenericPrimitives

[<Tests>]
let tests = 
  testList "DefaultParsers" [
    testList "%'a'" (testParam %'a' ParamTests.take |> List.ofSeq)
  ]