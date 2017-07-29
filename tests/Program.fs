// Learn more about F# at http://fsharp.org

open System

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#! %i" (Parsec.Main.X 1 2)
    0 // return an integer exit code
