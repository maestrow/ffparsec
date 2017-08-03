namespace Parsec.Types.ParserInfo

type ParserInfo = 
  {
    Name: string
    Description: string
    Parameters: (string * ParameterInfo) list
    IsAnonym: bool
  }
and ParameterInfo = 
  | OrdinaryValue of string
  | ParserValue of ParserInfo

type IParser = 
  abstract GetInfo: unit -> ParserInfo




