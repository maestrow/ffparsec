namespace Parsec.Visualizers

open System

open Parsec
open Parsec.Types.ParserInfo

module ConsoleParserVisualizer = 
  let serialize (parser: ParserInfo) (showDescr: bool) = 
    let rec inner parser showDescr level = 
      (level, parser.Name)::(getParams parser.Parameters (level+1))
    and getParams (parameters: (string * ParameterInfo) list) level = 
      let mapParam = function
        | name, OrdinaryValue v -> [level, sprintf "%s = %s" name v]
        | name, ParserValue parserInfo -> (level, name)::(inner parserInfo showDescr (level+1))
      parameters
      |> List.collect mapParam
    
    let indent level = Seq.init level (fun i -> "  ") |> String.Concat
    
    inner parser showDescr 0
    |> List.map (fun i -> sprintf "%s%s" (i |> fst |> indent) (i |> snd))
    |> String.concat "\n"

  