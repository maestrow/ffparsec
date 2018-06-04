(*
  Test fixtures without referencing Expecto
  to use in TestSandbox.fsx
*)

#r "../build/bin/Parsec.dll"

open Parsec
open Parsec.Logging

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
  | _ -> failwith "Ok expected"

let isError = function
  | Error _ -> ()
  | Ok _ -> failwith "Error expected"