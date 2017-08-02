namespace Parsec.Types.Debug

open Parsec.Utils.Trees
open Parsec.Types.ParserInfo

(* 
  Future improvement: 
  Make result and state parts of Result.Success as (stringified) properties tree. 
  That gives ability of nicer visualisation way in html debugging output.
*)
type DebugResult = 
  | Success of result: string * position: int * state: string
  | Fail of message: string
  | None

type DebugInfoItem = 
  {
    Parser: ParserInfo
    mutable Result: DebugResult
  }

type IDebugInfo = 
  abstract member LevelDown: unit -> unit
  abstract member LevelUp: unit -> unit
  abstract member Push: IParser -> unit
  abstract member SaveResult: DebugResult -> unit
