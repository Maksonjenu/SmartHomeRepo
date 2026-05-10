# SmartHomeRepo

Локальный учебный HTTP-сервер для управления квартирами, комнатами и сенсорной информацией.

## О проекте

Сервер хранит данные о квартирах, комнатах и датчиках (площадь, температура, состояние света). Проект реализован на .NET 10 с использованием минимальных Web API и SQLite.

Основные сущности:
- `Apartment` — квартира с номером и описанием.
- `Room` — комната с типом, названием, описанием и ссылкой на квартиру.
- `RoomInfo` — сенсорные параметры комнаты: площадь, температура, состояние света.

## Доступные эндпоинты

- `GET /api/apartments` — список квартир
- `POST /api/apartments` — создать квартиру
- `PUT /api/apartments/{id}` — обновить квартиру
- `DELETE /api/apartments/{id}` — удалить квартиру
- `GET /api/apartments/{id}/rooms` — комнаты конкретной квартиры
- `GET /api/rooms` — список комнат
- `GET /api/rooms/{id}` — данные по комнате
- `POST /api/rooms` — создать комнату
- `PATCH /api/rooms/{id}` — обновить метаданные комнаты
- `PATCH /api/rooms/{id}/sensors` — обновить сенсорные параметры
- `PATCH /api/rooms/{id}/light` — переключить свет
- `DELETE /api/rooms/{id}` — удалить комнату

## Запуск из исходников

### Через .NET CLI

Склонируйте проект:

```cmd
git clone https://github.com/sibsutis/piogi-sem-2-lab-7.git
```

либо скачайте ZIP-архив исходного кода.

Запуск проекта-сервера:

```powershell
cd SmartHomeRepo
dotnet restore
dotnet run --project SmartHomeRepo
```

### Параметры командной строки

Вы можеет указать дополнительные аргументы при запуске через консоль:

- `--port <номер>` — порт для сервера (по умолчанию `5000`)
- `--EnableSwagger true` — включить Swagger UI в режиме Release (включен по умолчанию)
- `--enable-network` — включить реальную сетевую симуляцию (для сдачи лабораторной работы)

Пример:

```powershell
dotnet run --port 5000 --enable-network --project SmartHomeRepo
```

## Запуск из релиза

Если вы подготовили релизный пакет, запустите файл из папки `publish`:

```powershell
SmartHomeRepo.exe --port 5000 --EnableSwagger true --enable-network
# Вариант команды, можно не указывать аргументы
SmartHomeRepo.exe
```

Либо просто дважды кликнуть по `SmartHomeRepo.exe`.

## Swagger

**✅ Swagger UI включён по умолчанию при EnableSwagger=true или в режиме Development.**

Swagger UI доступен, если приложение запущено в режиме Development или если задана опция `EnableSwagger=true`.

## Сетевая симуляция

- По умолчанию используется `DebugNetwork`.
- При `--enable-network` включается `RealNetwork`, которая имитирует задержки и ошибки.

## Примечания

- База данных хранится в `smart_home.db` по умолчанию.
- Для изменения пути к базе используйте параметр `--db <имя файла>`.
- База данных создается автоматически во время первого запуска проекта, если отсутствует файл `smart_home.db` в папке где находится `SmartHomeRepo.exe`.
