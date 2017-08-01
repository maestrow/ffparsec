namespace Parsec

open System

/// 'i - Item type, 'u - UserState
type Input<'Item, 'UserState> = {
  InputStream: seq<'Item>
  /// Current input stream position
  Position: int
  UserState: 'UserState
} 
/// Result type
type ParseResult<'Result, 'UserState> = 
  | Success of result: 'Result * consumed: int * state: 'UserState
  | Fail of message: string

type Parser<'Item, 'Result, 'UserState> = Input<'Item, 'UserState> -> ParseResult<'Result, 'UserState>


[<AutoOpen>]
module Core =

  type Input<'Item, 'UserState> with
    
    static member FromString (str: string, state: 'u) = 
      { InputStream = str; Position = 0; UserState = state }
    static member FromString (str: string) = 
      Input<string, unit>.FromString (str, ())
    
    member private this.CheckNull () = 
      if isNull this.InputStream then failwith "InputStream is null"
    member this.Length 
      with get () = 
        this.CheckNull ()
        this.InputStream |> Seq.length
    member this.IsEmpty 
      with get () = this.Length = 0
    member private this.IsInRange (pos: int) =
      pos >= 0 && pos < this.Length
    member private this.CheckInRange (pos: int) = 
      if not <| this.IsInRange pos then failwithf "Specified position is not in sequence range: %i" pos
    member this.IsOverEnd 
      with get () = this.IsEmpty || this.Position >= this.Length
    member private this.CheckOverEnd () = if this.IsOverEnd then failwith "No more input"
    member this.Item
      with get(index) = 
        this.CheckInRange index
        this.InputStream |> Seq.item index
    member this.CurrentItem
      with get () = 
        this.CheckOverEnd ()
        this.[this.Position]
    
    member private this.GetForwardPos (increment: int) = 
      if increment < 0 then failwith "Increment can't be less than 0. So it isn't move forward, but backward."
      let max = this.Length - this.Position - 1
      if increment > max then failwithf "Increment can't be more that %i. You try to increment by %i" max increment
      this.Position + increment
    member this.UpdateState parseResult = 
      match parseResult with
      | Success (_, c, s) -> { this with Position = this.GetForwardPos c; UserState = s }
      | Fail _ -> this
    
    member this.SuccessResult  (result, count) = Success (result, count, this.UserState)
    member this.SuccessResult  result          = this.SuccessResult (result, 0)
    member this.SuccessState   (count, state)  = Success ((), count, state)
    member this.SuccessState   state           = this.SuccessState (0, state)
    member this.SuccessConsume count           = Success ((), count, this.UserState)

  let runParser (p: Parser<'i,'r,'u>) (input: Input<'i,'u>) = 
    p input

  let runParserOnString (p: Parser<char,'r,'u>) (inputStr: string) (state: 'u) = 
    let input = Input.FromString (inputStr, state)
    runParser p input

  let run (p: Parser<char,'r, unit>) (input: string) = 
    runParserOnString p input ()

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


  module Logical = 
    
    open Parsec

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
      fun input ->
        let result1 = runParser p1 input
        match result1 with
        | Success _ -> result1
        | Fail err -> 
          let result2 = runParser p2 input
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

  module Quantifiers = 
    
    open Parsec
    open Logical

    /// (helper) match zero or more occurences of the specified parser
    let rec private parseZeroOrMore parser result input =
      let parseResult = runParser parser input
      match parseResult with
      | Fail err -> 
        (result |> List.rev, 0, input.UserState)  
      | Success (value, consumed, state) -> 
          parseResult 
          |> input.UpdateState
          |> parseZeroOrMore parser (value::result)

    /// match zero or more occurences of the specified parser
    let many parser = 
      fun input ->
        Success (parseZeroOrMore parser [] input)

    /// match one or more occurences of the specified parser
    let many1 parser = 
      fun input state ->
        let firstResult = runParser parser input 
        match firstResult with
        | Fail err -> Fail err // failed
        | Success (value, consumed, state) -> 
            Success (firstResult 
            |> input.UpdateState
            |> parseZeroOrMore parser [value])

    /// Parses an optional occurrence of p and returns an option value.
    let opt p = 
      let some = p |>> Some
      let none = returnP None
      some <|> none

  module UserState = 

    open Parsec

    let getUserState = 
      fun input -> Success (input.UserState, 0, input.UserState)

    let setUserState (newUserState: 'UserState) = 
      fun input -> Success ((), 0, newUserState)

    let updateUserState (f: 'u -> 'u) =
      fun input -> Success ((), 0, input.UserState |> f)

    let userStateSatisfies (f: 'u -> bool) = 
      fun input -> 
        match f input.UserState with
        | true -> Success ((), 0, input.UserState)
        | false -> Fail "userStateSatisfies"

    let createParserForwardedToRef () =
      let dummyParser = fun input -> failwith "a parser created with createParserForwardedToRef was not initialized"
      let r = ref dummyParser
      let p = fun input -> !r input
      p, r : Parser<'i,'r,'u> * Parser<'i,'r,'u> ref

  module Common = 
    
    open Parsec
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