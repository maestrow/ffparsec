План итерационных усовершенствований, которые можно оформить в серию статей (как https://fsharpforfunandprofit.com/series/understanding-parser-combinators.html)

1. Простая реализация
2. Функции bind, apply, map, lift

3.1. UserState
3.2. Рекурсивные парсеры. См. [Recursive grammars with nesting restrictions](http://www.quanttec.com/fparsec/users-guide/parsing-with-user-state.html#recursive-grammars-with-nesting-restrictions) и генератор `createParserForwardedToRef`

4. Обобщение: возможность обрабатывать не только поток симполов, а поток любых объектов. 
5. Оптимизация: Парсер возвращает не оставщийся текст, а количество употребленных (consumed) элементов в исходном потоке
6. Представление грамматики в виде дерева парсеров для удобства отладки
7. Оптимизация дерева: a ~ (b ~ (c ~ d)) = op (a b c d). ~ = op. Касается таких парсеров как И, ИЛИ. Однако следует сделать обобщенную реализацию, т.к. могут быть и другие подобные парсеры.