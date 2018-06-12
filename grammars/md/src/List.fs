namespace Markdown

open System
open Parsec
open Parsec.Combinators
open Parsec.Extensions
open Parsec.Chars

module List = 
  type ListType = 
    | Asterisk
    | Dash
    | Plus
    | Ordered
    | Mixed

  type List = 
    List of 
      BulletType: ListType * 
      Items: ListItem list
  and ListItem = 
    ListItem of 
      Bullet: char * 
      ItemText: string * 
      Sublist: List option

  let createList (items: ListItem list) =
    let m = function
      | '-' -> Dash
      | '*' -> Asterisk
      | '+' -> Plus
      | _ -> Ordered
    items
    |> List.map ((fun (ListItem (bullet,_,_)) -> bullet) >> m)
    |> List.distinct
    |> function [el] -> el | _ -> Mixed
    |> fun bulletType -> List (bulletType, items)

  type ListState = ListLevel of int

  type IListState =
    abstract member ListState: ListState with get, set

  module Parsers = 
    // State
    let getListState = 
      getUserState () |>> (fun (us: IListState) -> us.ListState) 
      |> newParser "getListState"
    let updateListState s = 
      updateUserState (fun (us: IListState) -> us.ListState <- s; us)
      |> newParser "updateListState"
    let down = 
      getListState |>> ((fun (ListLevel l) -> l) >> ((+)1) >> ListLevel) >>= updateListState 
      |> newParser "down"
    let up = 
      getListState |>> ((fun (ListLevel l) -> l) >> (fun x -> x-1) >> ListLevel) >>= updateListState 
      |> newParser "up"

    // Item & List
    let indent = %"  " |> newParser "indent"
    let indentOfCurrentLevel = 
      getListState >>= (fun (ListLevel level) -> repeatExactly level indent |>> String.concat "") 
      |> newParser "indentOfCurrentLevel"
    let bullet = 
      choice [
        intP() >>. %'.' |> newParser "ordered"
        %'*' |> newParser "asterisk"
        %'-' |> withName "dash"
        %'+' |> newParser "plus"
      ] 
      |> newParser "bullet"
    let sublistP, sublistR = createParserForwardedToRef ()
    let item = 
      indentOfCurrentLevel 
      ++ bullet 
      +  %' ' 
      ++ ((until (newLine ()) (any())) .>> (newLine()) |>> String.Concat) 
      ++ sublistP
      => (fun bullet text sublist -> ListItem (bullet, text, sublist))
      |> newParser "item"
    let list = item |> many1 |>> createList |> newParser "list"
    
    do sublistR := down >>. opt list .>> up |> newParser "sublist"
