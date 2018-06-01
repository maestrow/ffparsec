namespace Parsec

open Parsec.Types.ParserInfo

type Parser<'Item, 'Result, 'UserState> = 
  {
    Info: ParserInfo
    Fn: ParseFn<'Item, 'Result, 'UserState>
  } 
  interface IParser with 
    member x.GetInfo () = x.Info

and ParseFn<'Item, 'Result, 'UserState> = Input<'Item, 'UserState> -> ParseResult<'Result, 'UserState>

and Input<'Item, 'UserState> = 
  {
    InputStream: seq<'Item>
    /// Current input stream position
    Position: int
    UserState: 'UserState
    DebugLogger: IDebugLogger option
  } 

and ParseResult<'Result, 'UserState> = 
  | Ok of result: 'Result * position: int * state: 'UserState
  | Error of string

and IDebugLogger = 
  abstract member Position: int with get, set
  abstract member LevelDown: unit -> unit
  abstract member LevelUp: unit -> unit
  abstract member Push: IParser -> unit
  abstract member SaveResult: ParseResult<'Result, 'UserState> -> unit








