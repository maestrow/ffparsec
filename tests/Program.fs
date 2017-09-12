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
  match r with
  | Ok (res, pos, state) -> callback res pos state
  | _ -> failtest "Ok expected"

let isError = function
  | Error _ -> ()
  | Ok _ -> failtest "Error expected"

[<Tests>]
let tests = 
  testList "" [
    
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
          isError <| runr (take 'a') "b"
        testCase "one should fail when position is at EOF" <| fun _ -> 
          isError <| runr (take 'a') ""
        testCase "one succeeded" <| fun _ -> 
          isOk (runr (take 'a') "a") (fun res pos _ -> 
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
          let p = take 'a' |> many
          isOk (runr p "aaa") (fun res pos _ -> 
            res =! ['a';'a';'a']
            pos =! 3
          )
        testCase "many 3" <| fun () ->
          let p = take 'a' |> many
          isOk (runr p "aaab") (fun res pos _ -> 
            res =! ['a';'a';'a']
            pos =! 3
          )
      ]
      testList "repeatExactly" [
        testCase "repeatExactly 2 (take 'a')" <| fun () -> 
          let p = repeatExactly 2 (take 'a')
          isOk (runr p "aa") (fun res pos _ -> 
            res =! ['a';'a']
            pos =! 2
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
          let p = any () .>>. take 'x'
          isError <| runr p "aw"
      ]
      
      testList "orElse" [
        testCase "orElse 1" <| fun _ ->
          let p = many (take 'a' <|> take 'b')
          isOk (runr p "abbaabaxyz") (fun res pos _ -> 
            res =! ['a'; 'b'; 'b'; 'a'; 'a'; 'b'; 'a']
            pos =! 7
          )
        testCase "orElse 2" <| fun _ ->
          let concat (a: char, b: char) = [a;b] |> String.Concat
          let p1 = take 'a' .>>. take '1' |>> concat
          let p2 = take 'b' .>>. take '2' |>> concat
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

    testList "Pipes" [
      testList "Two args" [
        testCase "p1 + p2 => (fun () -> \"ok\")" <| fun _ -> 
          let p1 = returnP 1
          let p2 = returnP 2
          let p = p1 + p2 => (fun () -> "ok")
          isOk (runr p "") (fun res pos _ -> 
            res() =! "ok"
            pos =! 0
          )
        testCase "p1 + p2 => \"ok\"" <| fun _ -> 
          let p1 = returnP 1
          let p2 = returnP 2
          let p = p1 + p2 => "ok"
          isOk (runr p "") (fun res pos _ -> 
            res =! "ok"
            pos =! 0
          )
        testCase "+p1 + p2 => id" <| fun _ -> 
          let p1 = returnP 1
          let p2 = returnP 2
          let p = +p1 + p2 => id
          isOk (runr p "") (fun res pos _ -> 
            res =! 1
            pos =! 0
          )
        testCase "p1 + +p2 => id" <| fun _ -> 
          let p1 = returnP 1
          let p2 = returnP 2
          let p = p1 + +p2 => id
          isOk (runr p "") (fun res pos _ -> 
            res =! 2
            pos =! 0
          )
        testCase "+p1 + +p2 => (+)" <| fun _ -> 
          let p1 = returnP 1
          let p2 = returnP 2
          let p = +p1 + +p2 => (+)
          isOk (runr p "") (fun res pos _ -> 
            res =! 3
            pos =! 0
          )
      ]
      testList "Three args" [
        testCase "+p1 + +p2 +p3 => fun a b c -> a + b + c" <| fun _ -> 
          let a1 = 1
          let a2 = 2
          let a3 = 3
          let ar = a1 + a2 + a3
          let p1 = returnP 1
          let p2 = returnP 2
          let p3 = returnP 3
          let p = +p1 + +p2 + +p3 => fun a b c -> a + b + c
          isOk (runr p "") (fun res pos _ -> 
            res =! 6
            pos =! 0
          )
      ]
      testList "Failing, Consuming and State" []
    ]

  ]

[<EntryPoint>]
let main argv =
  runTestsInAssembly defaultConfig argv
