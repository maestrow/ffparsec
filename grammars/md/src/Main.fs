namespace Markdown

open Parsec
open Parsec.Combinators
open Parsec.Extensions.Pipes
open Parsec.Extensions.DefaultParsers
open Parsec.Run

open Markdown.List

[<AutoOpen>]
module Main = 
  
  type UserState = 
    { 
      ListState: ListState      
    }
    interface IListState with
      member this.ListState with get() = this.ListState
    static member Default = { ListState = ListLevel 0 }

  runParserOnString Parsers.indent "" UserState.Default |> ignore