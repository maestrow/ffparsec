module TestFixtures

open Parsec
open Parsec.Logging

open Expecto

let run p stream = 
  let logger = DebugLogger ()
  runWithLogger p stream () logger, logger

/// Run parser on stream and get result
let runr p stream = run p stream |> fst

/// Run parser on stream and get logger
let runl p stream = run p stream |> snd

let isOk r callback = 
  match r with
  | Ok (res, pos, state) -> callback res pos state
  | _ -> failtest "Ok expected"

let isError = function
  | Error _ -> ()
  | Ok _ -> failtest "Error expected"