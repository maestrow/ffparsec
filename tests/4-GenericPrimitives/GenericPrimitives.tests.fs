module Tests.GenericPrimitives

open System

open Expecto
open Swensen.Unquote

open Parsec
open Parsec.Combinators
open TestFixtures

module ParamTests = 
  let take =
    [
      "Fail on wrong item", fun p () -> 
        isError <| runr p "b"
      "Fail when position is at EOF", fun p () -> 
        isError <| runr p ""
      "Return specified item if it equals to current. Pos += 1.", fun p () -> 
        isOk (runr p "a") (fun res pos _ -> 
          res =! 'a'
          pos =! 1
        )
    ]

[<Tests>]
let tests = 
  testList "GenericPrimitives" [
    testList "take" (testParam (take 'a') ParamTests.take |> List.ofSeq)
    testList "until" [
      testCase "Success" <| fun _ ->
        let p = until (seqOf "fin") (any()) |>> String.Concat
        isOk (runr p "123finxyz") (fun res pos _ -> 
          res =! "123"
          pos =! 3
        )
    ]
  ]