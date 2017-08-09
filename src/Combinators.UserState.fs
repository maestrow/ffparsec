namespace Parsec.Combinators

[<AutoOpen>]
module UserState = 

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
