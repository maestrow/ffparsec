(*
  Wordflow здесь следующий:
  - Правка библиотеки - добавление возможности или исправление ошибки
  - fake build -t Build
  - Тестирование с помощью данного скрипта
  - Перенос кода отсюда в тесты
*)

#r "../build/bin/Parsec.dll"
#load "TestFixtures.fsx"

open System

open Parsec
open Parsec.Combinators
open Parsec.Extensions
open Parsec.Chars
open Parsec.Logging
open Parsec.Visualizers

open TestFixtures

module ListMd = 
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

open ListMd

type UserState () = 
  interface IListState
    with 
      member val ListState = ListLevel 0 with get, set
  override this.ToString () =
    let ls = this :> IListState
    let lvl = ls.ListState |> (fun (ListLevel l) -> l)
    sprintf "%d" lvl

let until untilP doP = untilP |> notP >>. doP |> many

let test1 () = 
  //let stream = "- 111\n- 222\n- 333\n"
  let stream = """
- 111
  * aaa
  * bbb
- 222
  1. wef
  12123. wfwef
  2. ferf
- 333
"""


  let state = new UserState() :> IListState
  let logger = DebugLogger ()
  let res = runWithLogger (newLine () >>. Parsers.list) stream state logger
  let log = logger.GetTree () |> ConsoleDebugLogVisualizer.serialize 
  printfn "log: \n%s" log

  isOk res (fun res pos _ -> 
    printfn "res=%A, pos=%d" res pos
  )

test1()