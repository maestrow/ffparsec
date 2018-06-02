namespace Markdown

open Parsec
open Parsec.Combinators
open Parsec.Extensions.Pipes
open Parsec.Extensions.DefaultParsers
open Parsec.Run

module List = 
  type ListType = 
    | Asterisk
    | Dash
    | Plus
    | Ordered
    | Mixed

  type List = List of BulletType: ListType * Items: ListItem list
  and ListItem = ListItem of Bullet: string * ItemText: string * Sublist: List option

  let createList (items: ListItem list) =
    let m = function
      | "-" -> Dash
      | "*" -> Asterisk
      | "+" -> Plus
      | _ -> Ordered
    items
    |> List.map ((fun (ListItem (bullet,_,_)) -> bullet) >> m)
    |> List.distinct
    |> function [el] -> el | _ -> Mixed
    |> fun bulletType -> List (bulletType, items)

  type ListState = ListLevel of int

  type IListState =
    abstract member ListState: ListState

  module Parsers = 

    let getListState = getUserState () |>> (fun (us: IListState) -> us.ListState)
    let indent = %"  "
    let indentOfCurrentLevel = getListState >>= (fun (ListLevel level) -> repeatExactly level indent |>> String.concat "")

    let bullet = +p<int> + %'.'


  runParserOnString Parsers.indent "" (Unchecked.defaultof<IListState>) |> ignore