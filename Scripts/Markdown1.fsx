#load "references.fsx"

open System
open Parsec
open Parsec.Types.ParserInfo
open Parsec.Combinators
open Parsec.Run
open Parsec.Logging
open Parsec.Visualizers

let run p stream = 
  let logger = DebugLogger ()
  runWithLogger p stream () logger, logger

let printDebug result = 
  let r = result |> snd |> (fun (i: DebugLogger) -> i.GetTree ()) |> ConsoleDebugLogVisualizer.serialize
  printfn "\n%s" r

let printRes result = 
  result |> fst |> printResult


///////////////////////////////////////////////////////////////////////////////////////////////////



module Lists = 
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

module Paragraph =

  type Hyperlink = { Url: string; Label: string }
  type Image = { Url: string; Label: string }

  type Paragraph = Span list
  and Span = 
    | Text of string
    | Hyperlink of Hyperlink
    | Image of Image
    | Strong of Span list 
    | Emphasis of Span list
    //| Literal of string


open Lists
open Paragraph

type Block = 
  | CodeBlock
  | Paragraph of Paragraph
  | List of List
  | Table

type Document = Block seq


module ParsingState = 
  type ParserType = 
    | PStrong
    | PEmphasis
    | PExtension of string // Extension parser name

  type State = 
    { 
      Stack: ParserType list
      ListLevel: int; 
    }
    static member Default = { Stack = []; ListLevel = 0; }

open ParsingState

