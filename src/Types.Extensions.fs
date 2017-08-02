[<AutoOpen>]
module Parsec.TypesExtensions

type Input<'Item, 'UserState> with
  
  member private this.CheckNull () = 
    if isNull this.InputStream then failwith "InputStream is null"
  member this.Length 
    with get () = 
      this.CheckNull ()
      this.InputStream |> Seq.length
  member this.IsEmpty 
    with get () = this.Length = 0
  member private this.IsInRange (pos: int) =
    pos >= 0 && pos < this.Length
  member private this.CheckInRange (pos: int) = 
    if not <| this.IsInRange pos then failwithf "Specified position is not in sequence range: %i" pos
  member this.IsOverEnd 
    with get () = this.IsEmpty || this.Position >= this.Length
  member private this.CheckOverEnd () = if this.IsOverEnd then failwith "No more input"
  member this.Item
    with get(index) = 
      this.CheckInRange index
      this.InputStream |> Seq.item index
  member this.CurrentItem
    with get () = 
      this.CheckOverEnd ()
      this.[this.Position]
  
  member private this.GetForwardPos (increment: int) = 
    if increment < 0 then failwith "Increment can't be less than 0. So it isn't move forward, but backward."
    let max = this.Length - this.Position - 1
    if increment > max then failwithf "Increment can't be more that %i. You try to increment by %i" max increment
    this.Position + increment
  member this.UpdateState parseResult = 
    match parseResult with
    | Success (_, c, s) -> { this with Position = this.GetForwardPos c; UserState = s }
    | Fail _ -> this
  
  member this.SuccessResult  (result, count) = Success (result, count, this.UserState)
  member this.SuccessResult  result          = this.SuccessResult (result, 0)
  member this.SuccessState   (count, state)  = Success ((), count, state)
  member this.SuccessState   state           = this.SuccessState (0, state)
  member this.SuccessConsume count           = Success ((), count, this.UserState)


type Parser<'Item, 'Result, 'UserState> with
  static member Default = { Unchecked.defaultof<Parser<'Item, 'Result, 'UserState>> with IsAnonym = false } 
  static member Anonym = { Unchecked.defaultof<Parser<'Item, 'Result, 'UserState>> with IsAnonym = true }
