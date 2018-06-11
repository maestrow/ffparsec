(*
  Wordflow здесь следующий:
  - Правка библиотеки - добавление возможности или исправление ошибки
  - fake build -t Build
  - Тестирование с помощью данного скрипта
  - Перенос кода отсюда в тесты
*)

#r "../build/bin/Parsec.dll"
#load "TestFixtures.fsx"

open System

open Parsec
open Parsec.Combinators
open Parsec.Chars

open TestFixtures

// digit

let until untilP doP = untilP |> notP >>. doP |> many

let test1 () = 
  let p = until (seqOf "fin") (any()) |>> String.Concat
  isOk (runr p "123finxyz") (fun res pos _ -> 
    printf "res=%s, pos=%d" res pos
  )

test1()