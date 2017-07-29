module Parsec.Primitives

open System
open Common
open Combinators

/// parse a char 
let pchar charToMatch = 
    let label = sprintf "%c" charToMatch 
    let innerFn input =
        if String.IsNullOrEmpty(input) then
            Failure (label,"No more input")
        else
            let first = input.[0] 
            if first = charToMatch then
                let remainingInput = input.[1..]
                Success (charToMatch,remainingInput)
            else
                let err = sprintf "Unexpected '%c'" first
                Failure (label,err)
    // return the parser
    create innerFn label

    /// Helper to create a string from a list of chars
let private charListToStr charList = 
     String(List.toArray charList)

// match a specific string
let pstring str = 
    str
    // convert to list of char
    |> List.ofSeq
    // map each char to a pchar
    |> List.map pchar 
    // convert to Parser<char list>
    |> sequence
    // convert Parser<char list> to Parser<string>
    |> mapP charListToStr 