namespace Parsec.Logging

open System.Collections.Generic
open System.Linq

open Parsec
open Parsec.Types.ParserInfo
open Parsec.Utils.Trees

type LogResultItem = 
  | Success of result: string * position: int * state: string
  | Fail of message: string
  | None

type LogItem = 
  {
    Parser: ParserInfo
    mutable Result: LogResultItem
  } with
      member x.SetResult (r: LogResultItem) = 
        x.Result <- r

module private Internals = 

  let toDebugResult (r: ParseResult<'r,'u>) (initialPosition: int) : LogResultItem = 
    match r with
    | Result.Ok (result, pos, state) -> 
        let strRes = sprintf "%A" result
        let strState = sprintf "%A" state
        LogResultItem.Success (strRes, pos, strState)
    | Result.Error err -> LogResultItem.Fail err

open Internals

type DebugLogger () = 
  let root = new Tree<LogItem>()
  let mutable pointer = root
  let parents = new Stack<Tree<LogItem>>()
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
      let item = { Parser = p.GetInfo(); Result = LogResultItem.None }
      pointer.Add (item |> toNode)
    member x.SaveResult result = 
      (toDebugResult result position) |> x.Last.Item.SetResult
