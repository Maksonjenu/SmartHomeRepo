Проект реализует локальный http сервер для лабораторных работ по курсу ПИОГИ.

## таблицы

3 таблицы:

Apartment (Квартира)

- Id (int)
- Number (string) — номер может быть "12А", используй string.
- Description (string)

Room (Комната)

- Id (int)
- ApartmentId (int)
- Name (string)
- Description (string)
- RoomType (string) — обязательно для иконок (Kitchen, Bedroom, etc).

RoomInfo (Информация о комнате)

- RoomId (int)
- Area (double)
- Temperature (double)
- LightState (bool) — состояние света (Вкл/Выкл).

## функционал сервера

получение

- GET /apartments — Список всех квартир (кратко: ID и Номер).
- GET /apartments/{id}/rooms — Список комнат конкретной квартиры.
- GET /rooms/{id} — Деталка конкретной комнаты (Название + Описание).
- GET /rooms/{id}/info — Состояние датчиков (Площадь, Температура, Свет).

добавление

- PATCH /rooms/{id}/light
- PUT /rooms/{id}/temperature

Симуляция задержки, реальные коды ошибок