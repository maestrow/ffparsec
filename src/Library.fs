namespace Parsec

type Result<'a> =
    | Success of 'a
    | Failure of string 


type Parser<'T> = Parser of (string -> Result<'T * string>)

module Main = 

    let X a b = a + b