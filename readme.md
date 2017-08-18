# Parsec lite - Облегченная версия библиотеки парсер-комбинаторов на F#

Существует отличная библиотека парсер-комбинаторов [FParsec](http://www.quanttec.com/fparsec/). Она хорошо документирована и оптимизирована по производительности. 

Мотивы создания данной библиотеки:

1. Возможность компилировать библиотеку в Javascript с помощью [Fable](http://fable.io/). 
2. Удобство отладки грамматик. Представление конечной грамматики в виде дерева парсеров (см. например [Irony - .NET Language Implementation Kit](https://irony.codeplex.com/))

Данная библиотека создавалась на основе:

- идей и кода [FParsec](http://www.quanttec.com/fparsec/)
- серии статей [Understanding Parser Combinators](https://fsharpforfunandprofit.com/series/understanding-parser-combinators.html)
- Библиотеки http://rspeele.github.io/FParsec-Pipes

## Запуск тестов

`./run.cmd --sequenced --summary`


## Дальнейнее развитие

- Веб-приложение для отладки.
- OMeta синтаксис. 


## Удобный синтаксис

let p = +'a' + "assad" ++ [' '; "\r\n"; p2] * qty.[0..] => fun x y -> x + y