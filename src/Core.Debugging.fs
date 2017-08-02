namespace Parsec.Core.Debugging

open System.Collections.Generic
open System.Linq
open Parsec
open Parsec.Utils.Trees
open Parsec.Types.Debug


module private Internals = 
  
  type DebugInfoItem with
    member x.SetResult (r: DebugResult) = 
      x.Result <- r

open Internals

type DebugInfo () = 
  let root = new Tree<DebugInfoItem>()
  let mutable pointer = root
  let parents = new Stack<Tree<DebugInfoItem>>()
  let setPointer tree = 
    pointer <- tree

  member private x.Last with get () = pointer.Last()

  interface IDebugInfo with
    member x.LevelDown () =
      parents.Push pointer
      setPointer x.Last.Children
    
    member x.LevelUp ()  = 
      setPointer (parents.Pop())

    member x.Push p = 
      let item = { Parser = p.GetInfo(); Result = DebugResult.None }
      pointer.Add (item |> toNode)

    member x.SaveResult (result: DebugResult) = 
      result |> x.Last.Item.SetResult 


[<AutoOpen>]
module Converters = 
  let toDebugResult (r: ParseResult<'r,'u>) (initialPosition: int) : DebugResult = 
    match r with
    | ParseResult.Success (result, consumed, state) -> 
        let strRes = sprintf "%A" result
        let strState = sprintf "%A" state
        DebugResult.Success (strRes, initialPosition + consumed, strState)
    | ParseResult.Fail err -> DebugResult.Fail err