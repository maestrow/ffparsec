module Parsec.Common

/// Creates parser with parseFn and label
let create parseFn label =
    { parseFn = parseFn; label = label }
    
/// Run a parser with some input
let run (parser: Parser<'a>) input = 
    // get inner function
    let innerFn = parser.parseFn 
    // call inner function with input
    innerFn input

/// Working with parsers labels
[<AutoOpen>]
module Labels = 
    /// get the label from a parser
    let getLabel parser = 
        // get label
        parser.label

    /// update the label in the parser
    let setLabel parser newLabel = 
        // change the inner function to use the new label
        let newInnerFn input = 
            let result = parser.parseFn input
            match result with
            | Success s ->
                // if Success, do nothing
                Success s 
            | Failure (oldLabel,err) -> 
                // if Failure, return new label
                Failure (newLabel,err)         // <====== use newLabel here
        // return the Parser
        create newInnerFn newLabel

    /// infix version of setLabel
    let ( <?> ) = setLabel

[<AutoOpen>]
module Core = 
    /// "bindP" takes a parser-producing function f, and a parser p
    /// and passes the output of p into f, to create a new parser
    let bindP f p =
        let label = "unknown"                     // <====== "label" is new! 
        let innerFn input =
            let result1 = run p input 
            match result1 with
            | Failure (label,err) ->              // <====== "label" is new! 
                // return error from parser1
                Failure (label,err)  
            | Success (value1,remainingInput) ->
                // apply f to get a new parser
                let p2 = f value1
                // run parser with remaining input
                run p2 remainingInput
        create innerFn label            // <====== "parseFn" and "label" are new!

    /// Infix version of bindP
    let ( >>= ) p f = bindP f p

    /// Lift a value to a Parser
    let returnP x = 
        let label = sprintf "%A" x
        let innerFn input =
            // ignore the input and return x
            Success (x,input)
        // return the inner function
        {parseFn=innerFn; label=label}

    /// apply a function to the value inside a parser
    let mapP f = 
        bindP (f >> returnP)

    /// infix version of mapP
    let ( <!> ) = mapP

    /// "piping" version of mapP
    let ( |>> ) x f = mapP f x

    /// apply a wrapped function to a wrapped value
    let applyP fP xP =         
        fP >>= (fun f -> 
        xP >>= (fun x -> 
            returnP (f x) ))

    /// infix version of apply
    let ( <*> ) = applyP

    /// lift a two parameter function to Parser World
    let lift2 f xP yP =
        returnP f <*> xP <*> yP