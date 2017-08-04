[<AutoOpen>]
module Parsec.Primitives

open System
open Parsec
open Parsec.Combinators.Core
open Parsec.Combinators.Logical

/// Parse single item
[<Description("Parse single item")>]
let pItem f = 
  (fun (input: Input<'i,'u>) -> 
    let i = input.CurrentItem
    if f i then
      input.SuccessResult (i, 1)
    else
      Fail (sprintf "Unexpected: %A" i))
  |> parser


// Match a sequence of items
[<Description("Match a sequence of items")>]
let pSeq (f: 'a -> 'a -> bool) s =
  (s |> Seq.map (f >> pItem) |> List.ofSeq |> sequence)
  |> describe
  |> withParams [("sequence", box s)]


let pItemEq i = pItem (fun current -> current = i)

let pSeqEq s = pSeq (=) s

module Generic = 
  // Для данных парсеров удобно создать конкретные экземпляры для определенного типа элемента последовательности ('Item).
  // Например, для последовательности символов (парсинг строк):
  // pAny = Generic.pAny (). И далее в грамматиках использовать pAny вместо pAny ().

  let pAny () = pItem (fun current -> true)

  [<Description("Succeeds when position is over the end of sequence")>]
  let eof () = 
    (fun (input: Input<'i,'u>) -> 
      match input.IsOverEnd with
      | true -> input.SuccessEmpty
      | false -> Fail "Position is not over the end")
    |> parser