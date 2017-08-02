namespace Parsec


type Input<'Item, 'UserState> = 
  {
    InputStream: seq<'Item>
    /// Current input stream position
    Position: int
    UserState: 'UserState
  } 


/// Result type
type ParseResult<'Result, 'UserState> = 
  | Success of result: 'Result * consumed: int * state: 'UserState
  | Fail of message: string


type ParseFn<'Item, 'Result, 'UserState> = Input<'Item, 'UserState> -> ParseResult<'Result, 'UserState>


type ParserInfo = 
  {
    Name: string
    Description: string
    Parameters: (string * ParameterInfo) list
  }
and ParameterInfo = 
  | OrdinaryValue of string
  | ParserValue of ParserInfo

type IParser = 
  abstract GetInfo: unit -> ParserInfo

type Parser<'Item, 'Result, 'UserState> = 
  {
    Info: ParserInfo
    IsAnonym: bool
    Fn: ParseFn<'Item, 'Result, 'UserState>
  } 
  interface IParser with 
    member x.GetInfo () = x.Info