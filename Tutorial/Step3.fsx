namespace Parsec

open System

type ParseResult<'Result, 'UserState> = 
  | Success of result: 'Result * remainingInut: string * state: 'UserState
  | Fail of message: string

type Parser<'Result, 'UserState> = string -> 'UserState -> ParseResult<'Result, 'UserState>

module Common = 

  let runParser (p: Parser<'Result, 'UserState>) (input: string) (state: 'UserState) = 
    p input state

  let run (p: Parser<'Result, unit>) (input: string) = 
    runParser p input ()

  let printResult (result: ParseResult<'Result, 'UserState>) =
      match result with
      | Success (value, _, state) -> 
        printfn "%A" value
        printfn "State: %A" state
      | Fail error -> 
        printfn "Error parsing: %s" error

namespace Parsec.Combinators

  [<AutoOpen>]
  module Core = 
    
    open Parsec
    open Parsec.Common
    
    /// "bindP" takes a parser-producing function f, and a parser p
    /// and passes the output of p into f, to create a new parser
    let bindP f p =
      fun input state ->
        let result1 = runParser p input state
        match result1 with
        | Fail err -> Fail err // return error from parser1
        | Success (value1, remainingInput, state) ->
          let p2 = f value1                 // apply f to get a new parser
          runParser p2 remainingInput state // run parser with remaining input

    /// Infix version of bindP
    let (>>=) p f = bindP f p

    /// Lift a value to a Parser
    let returnP x = 
      fun input state ->
        Success (x, input, state) // ignore the input and return x

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


  [<AutoOpen>]
  module Logical = 
    
    open Parsec
    open Parsec.Common
    open Core
    
    /// Combine two parsers as "A andThen B"
    let andThen p1 p2 =         
      p1 >>= (fun p1Result -> 
      p2 >>= (fun p2Result -> 
        returnP (p1Result, p2Result) ))

    /// Infix version of andThen
    let (.>>.) = andThen

    let (.>>) p1 p2 = 
      p1 .>>. p2 
      |> mapP fst

    let (>>.) p1 p2 = 
      p1 .>>. p2 
      |> mapP snd

    let orElse p1 p2 =
      fun input state ->
        let result1 = runParser p1 input state
        match result1 with
        | Success _ -> result1
        | Fail err -> 
          let result2 = runParser p2 input state
          result2 

    // infix version of orElse
    let (<|>) = orElse

    /// choose any of a list of parsers
    let choice listOfParsers = 
      List.reduce (<|>) listOfParsers 

    /// Process the list of parsers
    let rec sequence parserList =
      let cons head tail = head::tail
      let consP = lift2 cons
      match parserList with
      | [] -> returnP []
      | head::tail ->
        consP head (sequence tail)

  
  [<AutoOpen>]
  module Quantifiers = 
    
    open Parsec
    open Parsec.Common
    open Core
    open Logical

    /// (helper) match zero or more occurences of the specified parser
    let rec private parseZeroOrMore parser input state =
      let firstResult = runParser parser input state
      match firstResult with
      | Fail err -> 
        ([], input, state)  
      | Success (firstValue, inputAfterFirstParse, state) -> 
        let (subsequentValues,remainingInput,state) = 
          parseZeroOrMore parser inputAfterFirstParse state
        let values = firstValue::subsequentValues
        values, remainingInput, state

    /// match zero or more occurences of the specified parser
    let many parser = 
      fun input state ->
        Success (parseZeroOrMore parser input state)

    /// match one or more occurences of the specified parser
    let many1 parser = 
      fun input state ->
        // run parser with the input
        let firstResult = run parser input 
        // test the result for Fail/Success
        match firstResult with
        | Fail err -> Fail err // failed
        | Success (firstValue, inputAfterFirstParse, state) -> 
          // if first found, look for zeroOrMore now
          let (subsequentValues,remainingInput,state) = 
            parseZeroOrMore parser inputAfterFirstParse state
          let values = firstValue::subsequentValues
          Success (values, remainingInput, state)  

    /// Parses an optional occurrence of p and returns an option value.
    let opt p = 
      let some = p |>> Some
      let none = returnP None
      some <|> none

  
  [<AutoOpen>]
  module Common = 
    
    open Parsec.Common
    open Core
    open Logical
    open Quantifiers

    let between p1 p2 p3 = 
      p1 >>. p2 .>> p3 

    /// Parses one or more occurrences of p separated by sep
    let sepBy1 p sep =
      let sepThenP = sep >>. p            
      p .>>. many sepThenP 
      |>> fun (p,pList) -> p::pList

    /// Parses zero or more occurrences of p separated by sep
    let sepBy p sep =
      sepBy1 p sep <|> returnP []

  module UserState = 

    open Parsec
    open Parsec.Common
    open Core
    open Logical

    let getUserState = 
      fun input state -> Success (state, input, state)

    let setUserState (newUserState: 'UserState) = 
      fun input state -> Success ((), input, newUserState)

    let updateUserState (f: 'u -> 'u) =
      fun input state -> Success ((), input, state |> f)

    let userStateSatisfies (f: 'u -> bool) = 
      fun input state -> 
        match f state with
        | true -> Success ((), input, state)
        | false -> Fail "userStateSatisfies"

    let createParserForwardedToRef () =
      let dummyParser = fun input state -> failwith "a parser created with createParserForwardedToRef was not initialized"
      let r = ref dummyParser
      let p = fun input state -> !r input getUserState
      p, r : Parser<_, 'u> * Parser<_, 'u> ref

  
namespace Parsec
  
  module Primitives = 
    
    open System
    open Parsec.Combinators

    let pchar charToMatch = 
      fun input state ->
        if String.IsNullOrEmpty(input) then
          Fail "No more input"
        else
          let first = input.[0] 
          if first = charToMatch then
            let remainingInput = input.[1..]
            Success (charToMatch, remainingInput, state)
          else
            let err = sprintf "Unexpected '%c'" first
            Fail err

    // match a specific string
    let pstring str = 
      let charListToStr charList = String(List.toArray charList)
      str
      |> List.ofSeq
      |> List.map pchar 
      |> sequence
      |> mapP charListToStr 

