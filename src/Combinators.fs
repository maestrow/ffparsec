module Parsec.Combinators

(**
  andThen .>>., .>>, .>>
  orElse <|>
  choice
  sequence
  many, many1
  opt
  between
  sepBy, sepBy1
*)

open Common

/// Combine two parsers as "A andThen B"
let andThen p1 p2 =         
    let label = sprintf "%s andThen %s" (getLabel p1) (getLabel p2)
    p1 >>= (fun p1Result -> 
    p2 >>= (fun p2Result -> 
        returnP (p1Result,p2Result) ))
    <?> label                       // <====== provide a custom label

/// Infix version of andThen
let (.>>.) = andThen

let (.>>) p1 p2 = 
    // create a pair
    p1 .>>. p2 
    // then only keep the first value
    |> mapP (fun (a,b) -> a) 

let (>>.) p1 p2 = 
    // create a pair
    p1 .>>. p2 
    // then only keep the second value
    |> mapP (fun (a,b) -> b) 

/// combine two parsers as "A orElse B"
let orElse parser1 parser2 =
    // construct a new label
    let label = sprintf "%s orElse %s" (getLabel parser1) (getLabel parser2)

    let innerFn input =
        // run parser1 with the input
        let result1 = run parser1 input

        // test the result for Failure/Success
        match result1 with
        | Success result -> 
            // if success, return the result
            result1

        | Failure (_,err) -> 
            // if failed, run parser2 with the input
            let result2 = run parser2 input

            // return parser2's result
            match result2 with
            | Success _ -> 
                // if success, return the result
                result2

            | Failure (_,err) -> 
                // if failed, return the error with overall label
                Failure (label,err)

    // return the Parser
    create innerFn label

// infix version of orElse
let ( <|> ) = orElse

/// choose any of a list of parsers
let choice listOfParsers = 
    List.reduce ( <|> ) listOfParsers 

/// Process the list of parsers
let rec sequence parserList =
    // define the "cons" function, which is a two parameter function
    let cons head tail = head::tail

    // lift it to Parser World
    let consP = lift2 cons

    // process the list of parsers recursively
    match parserList with
    | [] -> 
        returnP []
    | head::tail ->
        consP head (sequence tail)

/// (helper) match zero or more occurences of the specified parser
let rec private parseZeroOrMore parser input =
    // run parser with the input
    let firstResult = run parser input 
    // test the result for Failure/Success
    match firstResult with
    | Failure (label, err) -> 
        // if parse fails, return empty list
        ([], input)  
    | Success (firstValue,inputAfterFirstParse) -> 
        // if parse succeeds, call recursively
        // to get the subsequent values
        let (subsequentValues,remainingInput) = 
            parseZeroOrMore parser inputAfterFirstParse
        let values = firstValue::subsequentValues
        (values, remainingInput)  

/// match zero or more occurences of the specified parser
let many parser = 

    let innerFn input =
        // parse the input -- wrap in Success as it always succeeds
        Success (parseZeroOrMore parser input)

    create innerFn "many"

/// match one or more occurences of the specified parser
let many1 parser = 
    let innerFn input =
        // run parser with the input
        let firstResult = run parser input 
        // test the result for Failure/Success
        match firstResult with
        | Failure (label, err) -> 
            Failure (label, err) // failed
        | Success (firstValue,inputAfterFirstParse) -> 
            // if first found, look for zeroOrMore now
            let (subsequentValues,remainingInput) = 
                parseZeroOrMore parser inputAfterFirstParse
            let values = firstValue::subsequentValues
            Success (values,remainingInput)  
    create innerFn "many1"


/// Parses an optional occurrence of p and returns an option value.
let opt p = 
    let some = p |>> Some
    let none = returnP None
    some <|> none

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