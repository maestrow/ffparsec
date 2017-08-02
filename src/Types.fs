namespace Parsec

open Parsec.Types.ParserInfo
open Parsec.Types.Debug


type Input<'Item, 'UserState> = 
  {
    InputStream: seq<'Item>
    /// Current input stream position
    Position: int
    UserState: 'UserState
    DebugInfo: IDebugInfo
  } 


/// Result type
type ParseResult<'Result, 'UserState> = 
  | Success of result: 'Result * consumed: int * state: 'UserState
  | Fail of message: string


type ParseFn<'Item, 'Result, 'UserState> = Input<'Item, 'UserState> -> ParseResult<'Result, 'UserState>


type Parser<'Item, 'Result, 'UserState> = 
  {
    Info: ParserInfo
    IsAnonym: bool
    Fn: ParseFn<'Item, 'Result, 'UserState>
  } 
  interface IParser with 
    member x.GetInfo () = x.Info