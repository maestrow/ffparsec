namespace Parsec

open System
open System.Reflection
open System.Diagnostics
open System.Runtime.CompilerServices

open Parsec.Types.ParserInfo

[<AutoOpen>]
module ParserDescriptionAttr =

  [<AllowNullLiteral>]
  [<AttributeUsageAttribute(AttributeTargets.All)>]
  type DescriptionAttribute (value: string) = 
    inherit Attribute ()
    member this.Value = value


[<AutoOpen>]
module ParserDescriptionFunctions =

  type private DescriptionHelper = 
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    static member GetParserNameDescr (frame: int) = 
      let st = StackTrace()
      let sf = st.GetFrame(frame)
      let m = sf.GetMethod()
      let attr = m.GetCustomAttribute<DescriptionAttribute>()
      let descr = if isNull attr then String.Empty else attr.Value
      m.Name, descr

  let private createParser (p: Parser<'i,'r,'u>) (fn: ParseFn<'i,'r,'u>) = 
    let frame = 3 // Frames count in stack trace from farser definition function. 
    // 0 - parser definition function
    // 1 - anonym | parser | describe
    // 2 - createParser (this function)
    // 3 - DescriptionHelper.GetParserNameDescr, from which parser's name and description will be exctacted
    let name, descr = DescriptionHelper.GetParserNameDescr (frame)
    { 
      Parser<'i,'r,'u>.Default with 
        Info = 
          { 
            Name = name
            Description = descr
            Parameters = [] 
          }
        Fn = fn 
    }

  /// Creates parser from parse function
  let parser (fn: ParseFn<'i,'r,'u>) = createParser Parser<'i,'r,'u>.Default fn

  /// Creates anonymous parser from parse function
  let anonym (fn: ParseFn<'i,'r,'u>) = createParser Parser<'i,'r,'u>.Anonym fn

  /// Adds to parser name and description, extracted from parser definition function (a function, from where describe was called)
  let describe (p: Parser<'i,'r,'u>) = createParser p p.Fn

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

    
