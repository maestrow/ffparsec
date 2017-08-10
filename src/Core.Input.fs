[<AutoOpen>]
module Parsec.InputExtensions

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
  member this.IsPositionLegal 
    with get () =
      this.IsInRange this.Position
  member private this.CheckInRange (pos: int) = 
    if not <| this.IsInRange pos then failwithf "Specified position is not in sequence range: %i" pos
  member this.IsOverEnd 
    with get () = this.IsEmpty || this.Position >= this.Length
  member this.Item
    with get(index) = 
      this.CheckInRange index
      this.InputStream |> Seq.item index
  member this.CurrentItem
    with get () = 
      this.CheckInRange this.Position
      this.[this.Position]
  
  member this.UpdateState (parseResult: ParseResult<'r,_>) = 
    match parseResult with
    | Ok (_, pos, s) -> { this with Position = pos; UserState = s }
    | Error _ -> this
  
  member this.SuccessResult  (result, posDelta) = ParseResult.Ok (result, this.Position + posDelta, this.UserState)
  member this.SuccessResult  result             = this.SuccessResult (result, this.Position)
  member this.SuccessState   (posDelta, state)  = ParseResult.Ok ((), this.Position + posDelta, state)
  member this.SuccessState   state              = this.SuccessState (this.Position, state)
  member this.SuccessConsume posDelta           = ParseResult.Ok ((), this.Position + posDelta, this.UserState)
  member this.SuccessEmpty                      = ParseResult.Ok ((), this.Position, this.UserState)



