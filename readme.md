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

## Структура проекта

Структуру проекта можно увидеть в панели Project Explorer в VS Code и посмотреть комментарии в `Parsec.fsproj`.

Несмотря на то, что процесс компиляции настроен так, чтобы эти папки `src/bin` и `src/obj` не создавались (благодаря опции `BaseIntermediateOutputPath` в `Parsec.fsproj` и опции `--output` в `build.fsx` результат компиляции попадает в папку `/build`), они создаются расширением `ionide-fsharp` при работе в VS Code. Поэтому, чтобы `src/bin` и `src/obj` не отвлекали от структуры проекта, для VS Code создана настройка в `.vscode/settings.json` не показывать эти папки в списке файлов (панель explorer).

## Компиляция и запуск

Предварительные требования:

- [Paket-Bootstrap](https://gist.github.com/maestrow/94d99017380adbcadff29f048f423729#file-paket-bootstrap-md)
- [FAKE5](https://fake.build/) `dotnet tool install fake-cli -g --version=5.0.0-*` или другие способы получить fake: https://github.com/FakeBuild/fake-bootstrap/.

Компиляция:

    fake build


### Компиляция в javascript

    npm i
    cd src
    dotnet restore
    dotnet fable npm-run build

Результат будет помещен в `output-js`

### Запуск тестов

- `run.bat` параллельный запуск
- `./runDetailed.bat` (или `./run.cmd --sequenced --summary`) - последовательно с детализацией.


## Дальнейнее развитие

- Веб-приложение для отладки.
- OMeta синтаксис. 

