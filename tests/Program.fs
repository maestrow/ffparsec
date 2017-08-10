open System

open Expecto
open Swensen.Unquote

open Parsec
open Parsec.Types.ParserInfo
open Parsec.Combinators
open Parsec.Run
open Parsec.Logging
open Parsec.Visualizers
  
let run p stream = 
  let logger = DebugLogger ()
  runWithLogger p stream () logger, logger

let createInput stream state logger = 
  {
    InputStream = stream
    Position = 0
    UserState = state
    DebugLogger = logger
  }

[<Tests>]
let tests = 
  testList "" [
    
    testList "Input, UpdateState" [
      testCase "It should be allowed to expose an incorrect position in ParseOk" <| fun () ->
        // Why? If the parser succeeds on the last element, it can move the position further. And the next parser should fail if it expects the current element.
        let p = 
          parser "p" ""
          <| fun (input: Input<'i,'u>) -> 
            Ok ((), -1, input.UserState) // set out of range position
        let logger = DebugLogger ()
        let input = createInput "" () logger
        let inputNew = input.UpdateState (runParser p input)
        input.Position =! 0
        inputNew.Position =! -1
    ]

    testList "Primitives" [
      
      testList "pCurrent" [
        testCase "pCurrent should fail when position is out of range" <| fun _ -> 
          let (r,_) = run (pCurrent ()) ""
          Expect.isError r ""
        testCase "pCurrent succeeded when position is legal. Position should not change." <| fun _ -> 
          let (Ok (res, pos, _),_) = run (pCurrent ()) "x"
          res =! 'x'
          pos =! 0
      ]

      testList "pItemEq" [
        testCase "pItemEq on other item" <| fun _ -> 
          let p = pItemEq 'a'
          let (r,_) = run p "b"
          Expect.isError r ""
        testCase "pItemEq when EOF" <| fun _ -> 
          let p = pItemEq 'a'
          let (r,_) = run p ""
          Expect.isError r ""
        testCase "pItemEq succeeded" <| fun _ -> 
          let p = pItemEq 'a'
          let (Ok(res, pos, _),_) = run p "a"
          res =! 'a'
          pos =! 1
      ]
    ]

    testList "Quantifiers" [
      testList "many" [ 
        testCase "many 1" <| fun () ->
          let p = pAny () |> many
          let (Ok (res, pos, _),_) = run p "abc"
          res =! ['a';'b';'c']
          pos =! 3
        testCase "many 2" <| fun () ->
          let p = pItemEq 'a' |> many
          let (Ok (res, pos, _),_) = run p "aaa"
          res =! ['a';'a';'a']
          pos =! 3
        testCase "many 3" <| fun () ->
          let p = pItemEq 'a' |> many
          let (Ok (res, pos, _),_) = run p "aaab"
          res =! ['a';'a';'a']
          pos =! 3
      ]
    ]

    testList "Logical" [
      
      testList "andThen" [
        testCase "andThen 1" <| fun _ -> 
          let any = pAny ()
          let p = any .>>. any
          let (Ok (res, pos, _), _) = run p "ab"
          res =! ('a','b')
          pos =! 2
        testCase "andThen 2" <| fun _ -> 
          let any = pAny ()
          let p = any .>>. any
          let (Error err, _) = run p "a"
          ()
        testCase "andThen 3" <| fun _ -> 
          let p = pAny () .>>. pItemEq 'x'
          let (Error err, _) = run p "aw"
          () 
        ]
      
      testList "orElse" [
        testCase "orElse 1" <| fun _ ->
          let p = many (pItemEq 'a' <|> pItemEq 'b')
          let (Ok(res, pos, _), _) = run p "abbaabaxyz"
          res =! ['a'; 'b'; 'b'; 'a'; 'a'; 'b'; 'a']
          pos =! 7
        testCase "orElse 2" <| fun _ ->
          let p1: Parser<char, string, unit> = pItemEq 'a' .>>. pItemEq '1' |>> (fun (a, b) -> [a;b] |> String.Concat)
          let p2: Parser<char, string, unit> = (pItemEq 'b' .>>. pItemEq '2') |>> (fun (a, b) -> [a;b] |> String.Concat)
          let p: Parser<char, string list, unit> = many (p1 <|> p2)
          
          let (Ok(res1, pos1, _), _) = run p "a1b2b2a1a1b2XYZ"
          res1 =! ["a1"; "b2"; "b2"; "a1"; "a1"; "b2"]
          pos1 =! 12
          
          let (Ok(res2, pos2, _), _) = run p "a1b3a1"
          res2 =! ["a1"]
          pos2 =! 2

      ]
      
      testList "sequence" []
    ]



  ]

[<EntryPoint>]
let main argv =
  runTestsInAssembly defaultConfig argv
