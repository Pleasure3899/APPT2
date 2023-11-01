В главном классе Program есть два метода - ParseTree и GetAllFilesWithExtensionRecursively. 

В ParseTree необходимо открыть файл Structure.txt и распарсить иерархию в Directory структуру. 

Структура имеет следующий синтаксис

```
-C
--Program Files
---*Program.exe
--Windows
---System32
----*calc.exe
----*hosts.exe
```

Знак минуса (`-`) показывает иерархию директорий. То есть у нас есть директории C:\Program Files\, C:\Windows\, C:\Windows\System32\

Знак звездочки (`*`) показывает иерархию файлов. То есть у нас есть файлы C:\Program Files\Program.exe, C:\Windows\System32\calc.exe, C:\Windows\System32\hosts.exe

GetAllFilesWithExtensionRecursively - данный метод должен рекурсивно обходить все Files переданного Directory, а также Files всех SubDirecrtories до самого низа дерева и находить те файлы, имя которых заканчивается на string extension, передаваемый в параметрах вызова.

В задаче нужно использовать циклы for, case, if-else, Enum, по возможности избегать сложных типов данных вроде List<>, но это не будет ошибкой. Парсинг можно сделать либо через рекурсию, либо через Queue. LINQ использовать для решения задачи нельзя, про него вы еще не знаете ;)

Программа должна обрабатываться и завершаться без YouScrewedUpException.
