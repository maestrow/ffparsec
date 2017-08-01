[<AutoOpen>]
module Parsec.Combinators.Core
  
open Parsec
open Parsec.Core

/// "bindP" takes a parser-producing function f, and a parser p
/// and passes the output of p into f, to create a new parser
let bindP f p =
  fun input ->
    let result1 = runParser p input
    match result1 with
    | Fail err -> Fail err // return error from parser1
    | Success (value1, _, _) ->
        let p2 = f value1                 // apply f to get a new parser
        result1
        |> input.UpdateState 
        |> runParser p2                   // run parser with remaining input

/// Infix version of bindP
let (>>=) p f = bindP f p

/// Lift a value to a Parser
let returnP x = 
  fun input ->
    Success (x, 0, input.UserState) // ignore the input and return x

/// apply a function to the value inside a parser
let mapP f = 
  bindP (f >> returnP)

/// infix version of mapP
let (<!>) = mapP

/// "piping" version of mapP
let (|>>) x f = mapP f x

/// apply a wrapped function to a wrapped value
let applyP fP xP =         
  fP >>= (fun f -> 
  xP >>= (fun x -> 
    returnP (f x) ))

/// infix version of apply
let (<*>) = applyP

/// lift a two parameter function to Parser World
let lift2 f xP yP =
  returnP f <*> xP <*> yP