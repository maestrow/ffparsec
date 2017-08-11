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

/// Run parser on stream and get result
let runr p stream = run p stream |> fst

/// Run parser on stream and get logger
let runl p stream = run p stream |> snd

let createInput stream state logger = 
  {
    InputStream = stream
    Position = 0
    UserState = state
    DebugLogger = logger
  }

let isOk r callback = 
  Expect.isOk r ""
  match r with
  | Ok (res, pos, state) -> callback res pos state
  | _ -> ()

let isError r = Expect.isError r ""

[<Tests>]
let tests = 
  testList "" [
    
    testList "Input, UpdateState" [
      testCase "It should be allowed to expose an incorrect position in ParseOk" <| fun () ->
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
    ]

    testList "Primitives" [
      
      testList "current" [
        testCase "current should fail when position is out of range" <| fun _ -> 
          isError <| runr (current ()) ""
        testCase "current succeeded when position is legal. Position should not change." <| fun _ -> 
          isOk (runr (current ()) "x") (fun res pos _ -> 
            res =! 'x'
            pos =! 0
          )
      ]

      testList "one" [
        testCase "one should fail on wrong item" <| fun _ -> 
          isError <| runr (one 'a') "b"
        testCase "one should fail when position is at EOF" <| fun _ -> 
          isError <| runr (one 'a') ""
        testCase "one succeeded" <| fun _ -> 
          isOk (runr (one 'a') "a") (fun res pos _ -> 
            res =! 'a'
            pos =! 1
          )
      ]
    ]

    testList "Quantifiers" [
      testList "many" [ 
        testCase "many 1" <| fun () ->
          let p = any () |> many
          isOk (runr p "abc") (fun res pos _ -> 
            res =! ['a';'b';'c']
            pos =! 3
          )
        testCase "many 2" <| fun () ->
          let p = one 'a' |> many
          isOk (runr p "aaa") (fun res pos _ -> 
            res =! ['a';'a';'a']
            pos =! 3
          )
        testCase "many 3" <| fun () ->
          let p = one 'a' |> many
          isOk (runr p "aaab") (fun res pos _ -> 
            res =! ['a';'a';'a']
            pos =! 3
          )
      ]
    ]

    testList "Logical" [
      
      testList "andThen" [
        testCase "andThen 1" <| fun _ -> 
          let any = any ()
          let p = any .>>. any
          isOk (runr p "ab") (fun res pos _ -> 
            res =! ('a','b')
            pos =! 2
          )
        testCase "andThen 2" <| fun _ -> 
          let any = any ()
          let p = any .>>. any
          isError <| runr p "a"
        testCase "andThen 3" <| fun _ -> 
          let p = any () .>>. one 'x'
          isError <| runr p "aw"
      ]
      
      testList "orElse" [
        testCase "orElse 1" <| fun _ ->
          let p = many (one 'a' <|> one 'b')
          isOk (runr p "abbaabaxyz") (fun res pos _ -> 
            res =! ['a'; 'b'; 'b'; 'a'; 'a'; 'b'; 'a']
            pos =! 7
          )
        testCase "orElse 2" <| fun _ ->
          let concat (a: char, b: char) = [a;b] |> String.Concat
          let p1 = one 'a' .>>. one '1' |>> concat
          let p2 = one 'b' .>>. one '2' |>> concat
          let p = many (p1 <|> p2)
          
          isOk (runr p "a1b2b2a1a1b2XYZ") (fun res pos _ -> 
            res =! ["a1"; "b2"; "b2"; "a1"; "a1"; "b2"]
            pos =! 12
          )
          
          isOk (runr p "a1b3a1") (fun res pos _ -> 
            res =! ["a1"]
            pos =! 2
          )
      ]
      
      testList "sequence" []
    ]



  ]

[<EntryPoint>]
let main argv =
  runTestsInAssembly defaultConfig argv
