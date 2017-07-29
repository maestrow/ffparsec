module Parsec.Utils

let printResult result =
    match result with
    | Success (value,input) -> 
        printfn "%A" value
    | Failure (label,error) -> 
        printfn "Error parsing %s\n%s" label error