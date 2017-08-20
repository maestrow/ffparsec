namespace Parsec.Extensions

open Parsec
open Parsec.Combinators

module Pipes =

  let wrap p = anonym <| fun i -> runParser p i

  type Capture<'i,'r,'u> = Capture of Parser<'i,'r,'u>
  type PipedParser<'i,'a,'b,'u> = Piped of Parser<'i,'a->'b,'u>
  
  let inline (~+) (p: Parser<'i,'r,'u>) =
     p 
     |> wrap 
     |> withName "PipeArg.Capture" 
     |> withParams [("p", box p)]
     |> Capture

  let private plus p1 p2 resultComposer = 
    anonym <| fun input ->
      let r1 = runParser p1 input
      match r1 with
      | Error err -> Error (sprintf "Error on p1: %s" err)
      | Ok (res1, pos1, state1) -> 
          let r2 = r1 |> input.UpdateState |> runParser p2
          match r2 with
          | Error err -> Error (sprintf "Error on p2: %s" err)
          | Ok (res2, pos2, state2) -> 
              Ok (resultComposer res1 res2, pos2, state2)

  type PlusMarker = PlusMarker with
    static member (?<-) (PlusMarker, (Piped p1): PipedParser<'i,'f->'r2,'fr->'t,'u>, Capture p2) = 
      plus p1 p2 (fun r1 r2 -> (fun f -> (r1 f) r2))
      |> withName "Pipe + Capture"
      |> withParams [("p1", box p1); ("p2", box p2)]
      |> Piped
    static member (?<-) (PlusMarker, (Piped p1): PipedParser<'i,'a,'b,'u>, p2: Parser<'i,'r,'u>) = 
      plus p1 p2 (fun r1 r2 -> r1)
      |> withName "Pipe + Skip"
      |> withParams [("p1", box p1); ("p2", box p2)]
      |> Piped
    static member (?<-) (PlusMarker, Capture p1, Capture p2) = 
      plus p1 p2 (fun r1 r2 -> (fun f -> f r1 r2))
      |> withName "Capture + Capture"
      |> withParams [("p1", box p1); ("p2", box p2)]
      |> Piped
    static member (?<-) (PlusMarker, Capture p1, p2) = 
      plus p1 p2 (fun r1 r2 -> (fun f -> f r1))
      |> withName "Capture + Skip"
      |> withParams [("p1", box p1); ("p2", box p2)]
      |> Piped
    static member (?<-) (PlusMarker, p1, Capture p2) = 
      plus p1 p2 (fun r1 r2 -> (fun f -> f r2))
      |> withName "Skip + Capture"
      |> withParams [("p1", box p1); ("p2", box p2)]
      |> Piped
    static member (?<-) (PlusMarker, p1, p2) = 
      plus p1 p2 (fun r1 r2 -> id)
      |> withName "Skip + Skip"
      |> withParams [("p1", box p1); ("p2", box p2)]
      |> Piped
    static member inline (?<-) (PlusMarker, p1, p2) = p1 + p2

  let inline (+) p1 p2 = (?<-) PlusMarker p1 p2

  type DPlusMarker = DPlusMarker with
    static member (?<-) (DPlusMarker, p1: PipedParser<'i,'a->'b,'c,'u>, p2) = p1 + +p2
    static member (?<-) (DPlusMarker, p1: Capture<'i,'r,'u>, p2) = p1 + +p2
    static member (?<-) (DPlusMarker, p1: Parser<'i,'r,'u>, p2) = p1 + +p2

  let inline (++) p1 p2 = (?<-) DPlusMarker p1 p2

  let (=>) ((Piped p1): PipedParser<'i,'a,'b,'u>) (f: 'a) = 
    plus p1 (returnP f) (fun r f -> r f)
    |> withName "=>"
    |> withParams [("p1", box p1); ("f", box f)]

  