namespace Parsec.Combinators

[<AutoOpen>]
module Input = 

  open Parsec

  let getUserState () = 
    anonym <| fun input -> input.SuccessResult input.UserState

  let setUserState (newUserState: 'UserState) = 
    anonym <| fun input -> input.SuccessState newUserState

  let updateUserState (f: 'u -> 'u) =
    anonym <| fun input -> input.SuccessState (f input.UserState)

  let userStateSatisfies (f: 'u -> bool) = 
    anonym <| fun input -> 
      match f input.UserState with
      | true -> input.SuccessEmpty
      | false -> Error "userStateSatisfies"

  let createParserForwardedToRef () =
    let dummyParser = anonym <| fun input -> failwith "a parser created with createParserForwardedToRef was not initialized"
    let r = ref dummyParser
    let p = anonym <| fun input -> !r |> (fun a -> a.Fn) <| input
    p, r : Parser<'i,'r,'u> * Parser<'i,'r,'u> ref

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


  let eof () = 
    (fun (input: Input<'i,'u>) -> 
      match input.IsOverEnd with
      | true -> input.SuccessEmpty
      | false -> Error "Position is not over the end")
    |> parser "eof" "Succeeds when position is over the end of sequence"