namespace Parsec

/// Aliases 
type ParserLabel = string
type ParserError = string

/// Result type
type Result<'a> =
    | Success of 'a * string
    | Failure of ParserLabel * ParserError 

/// A Parser structure has a parsing function & label
type Parser<'a> = {
    parseFn : (string -> Result<'a>)
    label:  ParserLabel 
    }

