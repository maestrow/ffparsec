module ListsGrammar

#load "Step2.fsx"

open System

open Parsec
open Parsec.Common
open Parsec.Core
open Parsec.Combinators
open Parsec.Primitives

let eof = 
  fun input -> 
    if String.IsNullOrEmpty(input) then 
      Success ((), String.Empty)
    else
      Fail "Expected: EOF"

let eol = 
  choice [
    (pchar '\r' .>>. pchar '\n') |>> ignore
    pchar '\n' |>> ignore
    eof
  ]

let untilEol = 
  fun input -> 
    let rec iterate (result: seq<char>) source = 
      match run eol source with
      | Success (_, remaining) -> 
        let r = result |> Seq.rev |> String.Concat
        r, remaining
      | Fail err -> 
        let newRes = source.[0]::(result |> List.ofSeq)
        iterate newRes source.[1..]
    Success (iterate [] input)

let exactly (count: int) (p: Parser<'a>) : Parser<'a list> = 
  p |>> (fun i -> [i]) // ToDo

let bullet = 
  ['-'; '+'; '*']
  |> List.map pchar
  |> choice

let item = bullet .>> pchar ' ' >>. untilEol



let indent (level: int) = 
  pchar ' '
  |> exactly level

let list (level: int) = 
  indent level
