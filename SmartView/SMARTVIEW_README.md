# SmartView - WPF приложение для управления SmartHome

Простое WPF приложение на MVVM с **3 экранами** и навигацией между ними.

## 📱 Экраны приложения

### 1. **ConnectionPage** - Проверка соединения
- Попытка подключиться к серверу API
- При успехе → автоматический переход на список квартир
- При ошибке → показ ошибки + кнопка **Retry**

### 2. **ApartmentsPage** - Список квартир
- Получает все квартиры из API
- Показывает номер квартиры, описание и количество комнат
- Клик на квартиру → открыть список комнат

### 3. **RoomsPage** - Список комнат в квартире
- Получает комнаты выбранной квартиры
- Показывает название, тип, температуру, площадь и состояние света
- Кнопки:
  - **← Назад** - вернуться к списку квартир
  - **Подробнее →** - открыть детали комнаты

### 4. **RoomDetailsPage** - Детали комнаты
- Показывает полную информацию о комнате
- **Интерактивные элементы** (учебные примеры):
  - 📊 Слайдер площади (10-100 м²)
  - 🌡️ Слайдер температуры (15-30°C)
  - 💡 Чекбокс состояния света
- Кнопка **← Назад** - вернуться на главный экран (список квартир)

## 📁 Структура проекта

```
SmartView/
├── Services/
│   └── ApiService.cs           # Упрощённые запросы к API
├── Models/
│   ├── ApartmentModel.cs       # Модель квартиры
│   └── RoomModel.cs            # Модель комнаты
├── ViewModels/
│   ├── BaseViewModel.cs        # Базовый класс MVVM
│   ├── MainViewModel.cs        # Управление навигацией
│   ├── ConnectionViewModel.cs  # Проверка соединения
│   ├── ApartmentsViewModel.cs  # Список квартир
│   ├── RoomsViewModel.cs       # Список комнат
│   └── RoomDetailsViewModel.cs # Детали комнаты
├── Views/
│   ├── ConnectionPage.xaml     # Экран проверки соединения
│   ├── ApartmentsPage.xaml     # Экран квартир
│   ├── RoomsPage.xaml          # Экран комнат
│   └── RoomDetailsPage.xaml    # Экран деталей
├── Converters.cs               # Конвертеры для XAML
├── MainWindow.xaml             # Главное окно с ContentControl
└── App.xaml                    # Регистрация ресурсов
```

## 🔌 API Endpoints

Приложение использует эти простые endpoints:

```
GET /api/apartments                    # Получить все квартиры
GET /api/apartments/{id}/rooms         # Получить комнаты квартиры
GET /api/rooms/{id}                    # Получить информацию о комнате
```

## 🎯 Как использовать в обучении

### Для студентов - примеры:

1. **ApiService.cs** - показывает как делать HTTP запросы в .NET
   ```csharp
   var apartments = await _httpClient.GetFromJsonAsync<List<ApartmentModel>>("/api/apartments");
   ```

2. **MVVM паттерн** - навигация через ViewModel commands
   ```csharp
   SelectApartmentCommand = new RelayCommand(_ => SelectApartment());
   ```

3. **Data Binding** - привязка данных в XAML
   ```xaml
   <TextBlock Text="{Binding CurrentApartment.Number}"/>
   ```

4. **Converters** - преобразование типов данных
   ```xaml
   Visibility="{Binding IsLoading, Converter={StaticResource BoolToVisibility}}"
   ```

## 🚀 Запуск приложения

1. Убедитесь что **SmartHomeRepo (backend)** запущен на `http://localhost:5000`
2. Откройте **SmartView** проект
3. Запустите приложение (`F5` в Visual Studio)

## ⚙️ Настройка адреса сервера

В `ApiService.cs` можно изменить адрес сервера:

```csharp
public ApiService(string? baseAddress = null)
{
    _baseAddress = baseAddress ?? "http://localhost:5000";  // ← Здесь
    // ...
}
```

## 📝 Примечания

- ✅ Слайдеры и чекбоксы на RoomDetailsPage - это **учебные примеры** как работает binding
- ✅ Приложение использует **асинхронные запросы** (async/await)
- ✅ Навигация реализована через **ContentControl** (не NavigationService)
- ✅ Все компоненты написаны для **максимальной простоты** и понимания студентами

---

**Готово к показу студентам!** 🎓
