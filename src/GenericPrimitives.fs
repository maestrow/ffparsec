[<AutoOpen>]
module Parsec.Combinators.GenericPrimitives

open Parsec

//let private moveNext = anonym <| fun i -> i.SuccessConsume 1

//=== Base

/// Parse single item
let takeIf f = 
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
  (s |> Seq.map (f >> takeIf) |> List.ofSeq |> sequence)
  |> describe "pSeq" "Match a sequence of items"
  |> withParams [("sequence", box s)]


//=== One Element

let take i = takeIf (fun current -> current = i)

let oneOf items = takeIf (fun current -> items |> Seq.exists ((=) current))

let noneOf items = takeIf (fun current -> items |> Seq.exists ((=) current) |> not)

let any () = takeIf (fun current -> true) |> describe "any" "Parse any item"


//=== Sequence

let seqOf s = seqP (=) s


//=== Skip 

let skip i = take i |>> ignore

let skipIf f = takeIf f |>> ignore

let skipAnyOf items = oneOf items |>> ignore

let skipUntil items = noneOf items |>> ignore