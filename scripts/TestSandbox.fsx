(*
  Wordflow здесь следующий:
  - Правка библиотеки - добавление возможности или исправление ошибки
  - fake build -t Build
  - Тестирование с помощью данного скрипта
  - Перенос кода отсюда в тесты
*)

#r "../build/bin/Parsec.dll"
#load "TestFixtures.fsx"

open Parsec
open Parsec.Combinators
open Parsec.Chars

open TestFixtures

// digit

let testDigit1 () = // Parse digit
  let p = digit()
  isOk (runr p "1") (fun res pos _ -> 
    printfn "res: %c pos: %d" res pos
  )

let testDigit2 () = // Fail on not digit
  let p = digit()
  isError (runr p "a")

// intP

let testIntP1 () = 
  let p = intP ()
  isOk (runr p "9132423q") (fun res pos _ -> 
    printfn "res: %d pos: %d" res pos
  )

let testIntP2 () = 
  let p = intP ()
  isError (runr p "a")

testDigit1 ()
testDigit2 ()
testIntP1 ()
testIntP2 ()