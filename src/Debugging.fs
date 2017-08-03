namespace Parsec.Debugging

open System.Collections.Generic
open System.Linq

open Parsec
open Parsec.Types.ParserInfo
open Parsec.Utils.Trees

type DebugResult = 
  | Success of result: string * position: int * state: string
  | Fail of message: string
  | None

type DebugInfoItem = 
  {
    Parser: ParserInfo
    mutable Result: DebugResult
  } with
      member x.SetResult (r: DebugResult) = 
        x.Result <- r

module private Internals = 

  let toDebugResult (r: ParseResult<'r,'u>) (initialPosition: int) : DebugResult = 
    match r with
    | ParseResult.Success (result, consumed, state) -> 
        let strRes = sprintf "%A" result
        let strState = sprintf "%A" state
        DebugResult.Success (strRes, initialPosition + consumed, strState)
    | ParseResult.Fail err -> DebugResult.Fail err

open Internals

type DebugLogger () = 
  let root = new Tree<DebugInfoItem>()
  let mutable pointer = root
  let parents = new Stack<Tree<DebugInfoItem>>()
  let setPointer tree = 
    pointer <- tree
  let mutable position = 0
  member private x.Last with get () = pointer.Last()
  member x.GetTree () = root
  
  interface IDebugLogger with
    member x.Position 
      with get () = position
      and set (value) = position <- value
    member x.LevelDown () =
      parents.Push pointer
      setPointer x.Last.Children
    member x.LevelUp ()  = 
      setPointer (parents.Pop())
    member x.Push p = 
      let item = { Parser = p.GetInfo(); Result = DebugResult.None }
      pointer.Add (item |> toNode)
    member x.SaveResult result = 
      (toDebugResult result position) |> x.Last.Item.SetResult


[<AutoOpen>]
module Implementation = 
  let runParserWithLogger (p: Parser<'i,'r,'u>) stream state = 
    {
      InputStream = stream
      Position = 0
      UserState = state
      DebugLogger = DebugLogger()
    }
    |> runParser p