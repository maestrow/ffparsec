namespace Parsec

module private ParserInfoExtensions = 
  type ParserInfo with
    static member Default = { Name = ""; Description = ""; Parameters = []; IsAnonym = false }
    static member Anonym = { ParserInfo.Default with IsAnonym = true }
  
[<AutoOpen>]
module ParserDescription =

  open ParserInfoExtensions
  /// Adds to parser name and description, extracted from parser definition function (a function, from where describe was called)
  let describe name descr (p: Parser<'i,'r,'u>) = 
    { 
      p with 
        Info = 
          { 
            p.Info with
              Name = name
              Description = descr
          }
    }

  /// Creates parser from parse function
  let parser name descr (fn: ParseFn<'i,'r,'u>) = 
    { Info = ParserInfo.Default; Fn = fn } |> describe name descr

  /// Creates anonymous parser from parse function
  let anonym (fn: ParseFn<'i,'r,'u>) = //{ Parser<'i,'r,'u>.Anonym with Fn = fn }
    {
      Info = ParserInfo.Anonym
      Fn = fn
    }

  /// Get parse function from parser
  let fn (p: Parser<_, _, _>) = p.Fn

  /// Add a name to parser
  let withName (name: string) (p: Parser<_, _, _>) = 
    { p with 
        Info = { p.Info with Name = name } 
    }

  /// Add description to parser
  let withDescr (description: string) (p: Parser<_, _, _>) = 
    { p with 
        Info = { p.Info with Description = description } 
    }

  /// Add parameters info to parser
  let withParams parameters (parser: Parser<_, _, _>) = 
    let getParams (parameters: (string * obj) list) = 
      parameters
      |> List.map (function
                   | name, (:? IParser as p) -> name, ParserValue   (p.GetInfo ())
                   | name, v                 -> name, OrdinaryValue (sprintf "%A" v))
    { parser with 
        Info = { parser.Info with Parameters = getParams parameters } 
    }

    
