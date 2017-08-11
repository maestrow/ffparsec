namespace Markdown

open Parsec

module Say =
    let hello name =
        printfn "Hello %s" name
