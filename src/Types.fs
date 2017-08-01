namespace Parsec

/// 'i - Item type, 'u - UserState
type Input<'Item, 'UserState> = {
  InputStream: seq<'Item>
  /// Current input stream position
  Position: int
  UserState: 'UserState
} 
/// Result type
type ParseResult<'Result, 'UserState> = 
  | Success of result: 'Result * consumed: int * state: 'UserState
  | Fail of message: string

type Parser<'Item, 'Result, 'UserState> = Input<'Item, 'UserState> -> ParseResult<'Result, 'UserState>