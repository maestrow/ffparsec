# Parsec lite - Облегченная версия библиотеки парсер-комбинаторов на F#

Существует отличная библиотека парсер-комбинаторов [FParsec](http://www.quanttec.com/fparsec/). Она хорошо документирована и оптимизирована по производительности. 

Мотивы создания данной библиотеки:

1. Возможность компилировать библиотеку в Javascript с помощью [Fable](http://fable.io/). 
2. Удобство отладки грамматик. Представление конечной грамматики в виде дерева парсеров (см. например [Irony - .NET Language Implementation Kit](https://irony.codeplex.com/))
3. Удобный синтаксис: `let p = +'a' + "assad" ++ (' ' | "\r\n" | p2) * qty.[0..] => fun x y -> x + y` (работы ведутся)

Данная библиотека создавалась на основе:

- идей и кода [FParsec](http://www.quanttec.com/fparsec/)
- серии статей [Understanding Parser Combinators](https://fsharpforfunandprofit.com/series/understanding-parser-combinators.html)
- Библиотеки http://rspeele.github.io/FParsec-Pipes


## Запуск тестов

Запуск тестов последовательно с детализацией: `./run.cmd --sequenced --summary` или `./runDetailed.bat`.


## Дальнейнее развитие

- Веб-приложение для отладки.
- OMeta синтаксис. 

