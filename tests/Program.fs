open System

open Parsec
open Parsec.Primitives

[<EntryPoint>]
let main argv =
    let p = pitem 'Y'
    let result = run p "Y-factor"
    let result = 1
    printfn "Result: %A" result
    0 // return an integer exit code
