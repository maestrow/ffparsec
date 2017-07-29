open System

open Parsec
open Parsec.Common
open Parsec.Primitives

[<EntryPoint>]
let main argv =
    let p = pchar 'X'
    let result = run p "X-factor"
    printfn "Result: %A" result
    0 // return an integer exit code
