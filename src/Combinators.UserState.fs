namespace Parsec.Combinators

module UserState = 

  open Parsec

  let getUserState = 
    fun input -> Success (input.UserState, 0, input.UserState)

  let setUserState (newUserState: 'UserState) = 
    fun input -> Success ((), 0, newUserState)

  let updateUserState (f: 'u -> 'u) =
    fun input -> Success ((), 0, input.UserState |> f)

  let userStateSatisfies (f: 'u -> bool) = 
    fun input -> 
      match f input.UserState with
      | true -> Success ((), 0, input.UserState)
      | false -> Fail "userStateSatisfies"

  let createParserForwardedToRef () =
    let dummyParser = fun input -> failwith "a parser created with createParserForwardedToRef was not initialized"
    let r = ref dummyParser
    let p = fun input -> !r input
    p, r : Parser<'i,'r,'u> * Parser<'i,'r,'u> ref
