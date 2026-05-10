# SmartHome Server - Инструкции для студентов

## 🚀 Быстрый старт

### Вариант 1: Windows Executable (Самый простой)
1. Скачайте файл `SmartHomeServer-win-x64.zip`
2. Распакуйте архив
3. Запустите `SmartHomeRepo.exe`
4. Откройте браузер: `http://localhost:5000`

### Вариант 2: Docker (Кроссплатформенно)
Если установлен Docker Desktop:

```bash
docker-compose up -d
```

Сервер будет доступен на: `http://localhost:5000`

### Вариант 3: .NET CLI

Если установлен .NET 10.0 SDK:

```bash
dotnet SmartHomeRepo.dll
```

---

## 📚 API Endpoints

### Квартиры (Apartments)
- `GET /api/apartments` - Список всех квартир
- `POST /api/apartments` - Создать квартиру
- `PUT /api/apartments/{id}` - Обновить квартиру
- `DELETE /api/apartments/{id}` - Удалить квартиру
- `GET /api/apartments/{id}/rooms` - Комнаты в квартире

### Комнаты (Rooms)
- `GET /api/rooms` - Список всех комнат
- `POST /api/rooms` - Создать комнату
- `GET /api/rooms/{id}` - Информация о комнате
- `PATCH /api/rooms/{id}` - Обновить метаданные
- `PATCH /api/rooms/{id}/sensors` - Обновить датчики
- `PATCH /api/rooms/{id}/light` - Переключить свет
- `DELETE /api/rooms/{id}` - Удалить комнату

---

## 📖 Swagger Documentation

Откройте в браузере: `http://localhost:5000/swagger`

---

## ⚙️ Требования

- **Windows .exe**: Windows 7+, x64
- **Docker**: Docker Desktop
- **.NET**: .NET 10.0 Runtime или SDK

---

## 🔧 Переменные окружения

```
ASPNETCORE_ENVIRONMENT=Development  # Development или Production
ASPNETCORE_URLS=http://+:5000      # URL сервера
```

---

## 🐛 Troubleshooting

**Ошибка: "Порт 5000 занят"**
- Измените ASPNETCORE_URLS на другой порт, например `http://+:5001`

**Database ошибки**
- Сервер автоматически инициализирует БД при запуске

**SmartView WPF клиент**
- На Windows: скачайте SmartView-windows.zip и запустите SmartView.exe

---

## 📝 Примеры запросов

### Создать квартиру
```bash
curl -X POST http://localhost:5000/api/apartments \
  -H "Content-Type: application/json" \
  -d '{"number": "101", "description": "Apartment in building A"}'
```

### Создать комнату
```bash
curl -X POST http://localhost:5000/api/rooms \
  -H "Content-Type: application/json" \
  -d '{
    "apartmentId": 1,
    "name": "Спальня",
    "description": "Master bedroom",
    "roomType": "Bedroom",
    "area": 25.5,
    "temperature": 22.0,
    "lightState": false
  }'
```

---

**Вопросы?** Обратитесь к преподавателю! 📧
