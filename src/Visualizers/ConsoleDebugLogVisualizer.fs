namespace Parsec.Visuzlizers

open System
open Parsec.Utils.Trees
open Parsec
open Parsec.Types.ParserInfo
open Parsec.Debugging

module ConsoleDebugLogVisualizer = 
  
  module private Internals = 

    let parameterToString = function
      | name, OrdinaryValue v -> sprintf "%s=%s" name v
      | name, ParserValue _ -> name

    let parserInfoToString pi = 
      let strParams = pi.Parameters |> List.map parameterToString |> String.concat ", "
      sprintf "%s: %s" pi.Name strParams

    let debugResultToString = function
      | DebugResult.Success (res, pos, state) -> sprintf "Ok [%i]: %s. State: %s" pos res state
      | DebugResult.Fail msg -> sprintf "Fail: %s" msg
      | _ -> "None"

    let debugInfoItemToString item = 
      sprintf "%s | %s" (parserInfoToString item.Parser) (debugResultToString item.Result)
  
    let indent level = Seq.init level (fun i -> "  ") |> String.Concat

  open Internals

  let serialize (tree: Tree<DebugInfoItem>) = 
    tree
    |> Tree.map debugInfoItemToString
    |> Tree.toFlatList
    |> Seq.map (fun i -> sprintf "%s%s" (i |> fst |> indent) (i |> snd))
    |> String.concat "\n"