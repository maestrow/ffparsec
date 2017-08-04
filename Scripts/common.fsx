#load "references.fsx"

open Parsec
open Parsec.Combinators
open Parsec.Run
open Parsec.Logging
open Parsec.Visualizers


let run p stream state = 
  let logger = DebugLogger ()
  runWithLogger p stream state logger, logger