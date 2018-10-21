# Экспертная система "Распознай разъем"

### Запуск
Linux / MacOS X
```sh
./script/generate_proto.sh
# Консоль 1:
dotnet run --project ./src/ExpertSystem.Server/
# Консоль 2:
dotnet run --project ./src/ExpertSystem.Client/
```
Windows
```sh
./script/generate_proto.bat
# Консоль 1:
dotnet run --project .\src\ExpertSystem.Server\
# Консоль 2:
dotnet run --project .\src\ExpertSystem.Client\
```

```sh
dotnet run src/
```

### Контекст
На предмете по "проектированию экспертных систем" была поставлена задача реализации в ходе серии лабораторных работ реализовать различные экспертные подсистемы и реализовать конечный прототипа экспертной системы.На основании поставленой задачи и собранной команды было выбрано направление определения типа разъёма на основе переданных параметров.

### Содержание
На основе сервиса [octopart.com](http://octopart.com) был собран [экземпляр данных](https://github.com/zharkov-eu/recognize-connector/blob/master/data/1.csv) по существующим на сегодняшний день разъёмам внезависимости от их свойств и назначений. Полученные данные в формате в формате .csv были перобразобраны в формат .xlsx для изучения их значений и диапазонов используемых в дальнейшем параметров. В ходе решения поставленной задачи был написан программный продукт на языке C#, позволяющий с высокой точностью с помощью методов продукционного и логического вывода определить, является ли на основе предложенных фактов (параметров и характеристик) выбранный электронный элемент разъёмом определённого типа.

### Команда
- [@zharkov-eu](https://github.com/zharkov-eu) - Жарков Евгений - лучший эксперт в любом вопросе
- [@kadyrov-ruslan](https://github.com/kadyrov-ruslan) - Кадыров Руслан - математик и истинный C#-гуру
- [@matthewpoletin](https://github.com/matthewpoletin) - Полётин Матвей - энтузиаст в разработке электроники

## Экспертная система
*В процессе реализации*
* Интерфейс пользователя командной строки
* Модуль обработки решения
* Модуль логического вывода
* База знаний
* Модуль верификации знаний
* Интерфейс эксперта командной строки

## Реализованные подсистемы
### Продукционный вывод (Прямой / обратный)
* Построение графа фактов (домен:значение)
* Поиск в ширину / глубину

### Логический вывод
* Генерация первоночальных утверждений в форме домен:значение - конъюнкция - ... - домен:значение - импликация - разъем
* Конъктивная нормальная форма
* Алгоритм резолюции (отрицание утверждения и вывод пустой посылки)

### Нечеткий вывод (Мамдани / Сугэно)
* Генерация кластеров для доменов (с числовым типом) алгоритом нечёткой кластеризации c-means

### Нейро-нечёткий вывод (ANFIS)
*В процессе реализации*
* Построение 5-уровневой нейронной сети
