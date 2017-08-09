namespace Parsec.Combinators

[<AutoOpen>]
module Core = 

  // Здесь соблюдается принцип: комбинаторы получают Parser, а возвращают функцию ParseFn. 
  // Парсеры на входе исполняются единой функцией runParser. Которая может собирать отладочную информацию.
  // Полученную функцию можно подставить в Fn нового парсера

  open Parsec

  /// "bindP" takes a parser-producing function f, and a parser p
  /// and passes the output of p into f, to create a new parser
  let bindP f p =
    anonym <| fun input ->
      let result1 = runParser p input
      match result1 with
      | Error err -> Error err // return error from parser1
      | Ok (value1, _, _) ->
          let p2 = f value1    // apply f to get a new parser
          result1
          |> input.UpdateState 
          |> runParser p2                   // run parser with remaining input

  /// Infix version of bindP
  let (>-) p f = bindP f p

  /// Lift a value to a Parser
  let returnP x = anonym <| fun input -> input.SuccessResult x // ignore the input and return x

  /// apply a function to the value inside a parser
  let mapP f = 
    bindP (f >> returnP)

  let (|>>) x f = mapP f x

  /// apply a wrapped function to a wrapped value
  let applyP fp xp =         
    fp >- (fun f -> 
    xp >- (f >> returnP))

  /// infix version of apply
  let (<*>) fp xp = applyP fp xp

  /// lift a two parameter function to Parser World
  let lift2 f xP yP =
    returnP f <*> xP <*> yP