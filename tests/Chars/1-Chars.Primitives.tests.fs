module Tests.Chars.Primitives

open Expecto
open Swensen.Unquote

open Parsec.Chars

open TestFixtures

[<Tests>]
let tests = 
  testList "Chars.Primitives" [

    testList "digit" [
      testCase "Parse digit" <| fun _ ->
        let p = digit()
        isOk (runr p "1") (fun res pos _ -> 
          res =! '1'
          pos =! 1
        )
      testCase "Fail on non digit" <| fun _ ->
        let p = digit()
        isError (runr p "a")
    ]

    testList "intP" [
      testCase "Parse number" <| fun _ ->
        let p = intP ()
        isOk (runr p "9132423q") (fun res pos _ -> 
          res =! 9132423
          pos =! 7
        )
      testCase "Fail on non number" <| fun _ ->
        let p = intP ()
        isError (runr p "a")
    ]
  ]