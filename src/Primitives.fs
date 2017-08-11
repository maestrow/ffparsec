[<AutoOpen>]
module Parsec.Primitives

open System
open Parsec
open Parsec.Combinators.Core
open Parsec.Combinators.Logical
open System.Runtime.CompilerServices

(*

Для ряда парсеров удобно создать конкретные экземпляры для определенного типа элемента последовательности ('Item).
Например, для последовательности символов (парсинг строк):
pAny = Generic.pAny (). И далее в грамматиках использовать pAny вместо pAny ().

pAny
eof
pCurrent

*)


let pCurrent () = 
  (fun (input: Input<'i,'u>) -> 
    match input.IsPositionLegal with
    | true -> 
        input.SuccessResult input.CurrentItem
    | false -> Error (sprintf "Position is out of sequence range. Sequence length: %i, Position index: %i" input.Length input.Position))
  |> parser "pCurrent" "Return current item"

/// Parse single item
let pItem f = 
  let inner current = 
    (fun (input: Input<'i,'u>) -> 
      if f current then
        input.SuccessResult (current, 1)
      else
        Error (sprintf "Unexpected: %A" current))
    |> anonym
  pCurrent () >- inner 
  |> describe "pItem" "Parse single item"

// Match a sequence of items
let pSeq (f: 'a -> 'a -> bool) s =
  (s |> Seq.map (f >> pItem) |> List.ofSeq |> sequence)
  |> describe "pSeq" "Match a sequence of items"
  |> withParams [("sequence", box s)]


let pOne i = pItem (fun current -> current = i)

let pOneOf items = pItem (fun current -> items |> Seq.exists ((=) current))

let pSeqEq s = pSeq (=) s

let pAny () = pItem (fun current -> true) |> describe "pAny" "Parse any item"

let eof () = 
  (fun (input: Input<'i,'u>) -> 
    match input.IsOverEnd with
    | true -> input.SuccessEmpty
    | false -> Error "Position is not over the end")
  |> parser "eof" "Succeeds when position is over the end of sequence"