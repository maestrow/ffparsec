namespace Parsec.Utils

// See also: https://fsharpforfunandprofit.com/posts/recipe-part2/

[<AutoOpen>]
module Rop = 
    
    open Result

    // pipe a two-track value into a switch function 
    let (>>=) x f = 
        bind f x

    // compose two switches into another switch
    let (>=>) s1 s2 = 
        s1 >> bind s2
