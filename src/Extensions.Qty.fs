namespace Parsec.Extensions

open Parsec
open Parsec.Combinators

module Qty = 
  
  type ExactCount(count) =
    member __.Count = count
    static member inline (*) (range : ExactCount, p) = repeatExactly range.Count p
    static member inline (*) (p, range : ExactCount) = repeatExactly range.Count p


  type Range(min : int, max : int option) =
    member __.Min = min
    member __.Max = max
    member __.Of (p : Parser<'i,'r,'u>) =
      match min, max with
      | _,    Some 0                     -> returnP []
      | 0,    None                       -> many p
      | 0,    Some till when till > 0    -> repeatNoMore till p
      | from, None                       -> repeatAtLeast from p
      | from, Some till when from = till -> repeatExactly from p
      | from, Some till when from > till -> returnP []
      | from, Some till                  -> repeatFromTo from till p
    static member inline (*) (range : Range, p) = range.Of(p)
    static member inline (*) (p, range : Range) = range.Of(p)


  type RangeDefiner() =
    member __.GetSlice (min : int option, max : int option) =
      Range(defaultArg min 0, max)
    member __.Item (exactCount : int) =
      ExactCount(exactCount)

  let qty = RangeDefiner()