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


/// Return current item
let current () = 
  (fun (input: Input<'i,'u>) -> 
    match input.IsPositionLegal with
    | true -> 
        input.SuccessResult input.CurrentItem
    | false -> Error (sprintf "Position is out of sequence range. Sequence length: %i, Position index: %i" input.Length input.Position))
  |> parser "pCurrent" "Return current item"

/// Get current input position
let getPos () = 
  parser "getPos" "Get current input position"
  <| fun (input: Input<'i,'u>) -> 
    input.SuccessResult input.Position

// let skip () = 
//   fun (input: Input<'i,'u>) -> 


/// Parse single item
let item f = 
  let inner current = 
    (fun (input: Input<'i,'u>) -> 
      if f current then
        input.SuccessResult (current, 1)
      else
        Error (sprintf "Unexpected: %A" current))
    |> anonym
  current () >>= inner 
  |> describe "pItem" "Parse single item"

// Match a sequence of items
let seqP (f: 'a -> 'a -> bool) s =
  (s |> Seq.map (f >> item) |> List.ofSeq |> sequence)
  |> describe "pSeq" "Match a sequence of items"
  |> withParams [("sequence", box s)]

let one i = item (fun current -> current = i)

let oneOf items = item (fun current -> items |> Seq.exists ((=) current))

let noneOf items = item (fun current -> items |> Seq.exists ((=) current) |> not)

let seqEq s = seqP (=) s

let any () = item (fun current -> true) |> describe "pAny" "Parse any item"

let eof () = 
  (fun (input: Input<'i,'u>) -> 
    match input.IsOverEnd with
    | true -> input.SuccessEmpty
    | false -> Error "Position is not over the end")
  |> parser "eof" "Succeeds when position is over the end of sequence"