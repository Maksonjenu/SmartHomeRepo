using System.ComponentModel;


namespace SmartHomeRepo.DTO;

/// <summary>
/// DTO для передачи информации о комнате без вложенных данных. Содержит только базовые свойства комнаты, без информации о площади, температуре и состоянии света.
/// </summary>
[Description("DTO для передачи информации о комнате без вложенных данных")]
public class FlatRoomDto
{

[Description("ID комнаты")]
    public int Id { get; set; }
    [Description("Название комнаты")]
    public string Name { get; set; } = null!;
    [Description("Описание комнаты")]
    public string Description { get; set; } = null!;
    [Description("Тип комнаты")]
    public string RoomType { get; set; } = null!;
    [Description("ID квартиры")]
    public int ApartmentId { get; set; }

}


/// <summary>
/// DTO для передачи информации о комнате с возможностью обновления. Все свойства, кроме ID, являются nullable, что позволяет обновлять только те поля, которые были указаны в запросе. Если поле не указано (null), оно не будет изменено в базе данных.
/// </summary>
public class UpdateRoomMetadataDto
{
    [Description("ID комнаты")]
    public int Id { get; set; }

    [Description("Название комнаты")]
    public string? Name { get; set; }
    [Description("Описание комнаты")]
    public string? Description { get; set; }
    [Description("Тип комнаты")]
    public string? RoomType { get; set; }
}

/// <summary>
/// DTO для передачи информации сенсоров для возможностью обновления. Все свойства, кроме ID, являются nullable, что позволяет обновлять только те поля, которые были указаны в запросе. Если поле не указано (null), оно не будет изменено в базе данных.
/// </summary>
public class InUpdateSensorDto
{
    [Description("Площадь комнаты в квадратных метрах")]
    public double? Area { get; set; }
    [Description("Температура в комнате в градусах Цельсия")]
    public double? Temperature { get; set; }
    [Description("Состояние света в комнате (true - включен, false - выключен)")]
    public bool? LightState { get; set; }

}

/// <summary>
/// DTO для получения информации сенсоров после обновления.
/// </summary>
public class OutUpdateSensorDto
{
    [Description("ID комнаты")]
    public int Id { get; set; }
    [Description("Площадь комнаты в квадратных метрах")]
    public double? Area { get; set; }
    [Description("Температура в комнате в градусах Цельсия")]
    public double? Temperature { get; set; }
    [Description("Состояние света в комнате (true - включен, false - выключен)")]
    public bool? LightState { get; set; }

}

/// <summary>
/// DTO для создания новой комнаты. Все поля являются обязательными для заполнения. ApartmentId указывает, в какой квартире будет создана комната. Id самой комнаты не указывается, так как он будет сгенерирован базой данных при сохранении новой комнаты.
/// </summary>
public class CreateRoomDto
{
    [Description("ID квартиры, в которой создается комната")]
    public int ApartmentId { get; set; }

    [Description("Название комнаты")]
    public string Name { get; set; }
    [Description("Описание комнаты")]
    public string Description { get; set; }

    [Description("Тип комнаты")]
    public string RoomType { get; set; }
    [Description("Площадь комнаты в квадратных метрах")]
    public double Area { get; set; }
    [Description("Температура в комнате в градусах Цельсия")]
    public double Temperature { get; set; }
    [Description("Состояние света в комнате (true - включен, false - выключен)")]
    public bool LightState { get; set; }

}


/// <summary>
/// DTO для передачи информации о комнате с полной информацией. Содержит все свойства комнаты, включая площадь, температуру и состояние света. Используется для получения полной информации о комнате клиентом.
/// </summary>
public class RoomDto
{
    [Description("ID комнаты")]
    public int Id { get; set; }
    [Description("Название комнаты")]
    public string Name { get; set; }
    [Description("Описание комнаты")]
    public string Description { get; set; }

    [Description("Тип комнаты")]
    public string RoomType { get; set; }
    [Description("Площадь комнаты в квадратных метрах")]
    public double Area { get; set; }
    [Description("Температура в комнате в градусах Цельсия")]
    public double Temperature { get; set; }
    [Description("Состояние света в комнате (true - включен, false - выключен)")]
    public bool LightState { get; set; }

}

/// <summary>
/// DTO для передачи информации о квартире. Содержит базовую информацию о квартире, включая ID, номер, описание и количество комнат. Не содержит вложенных данных о комнатах внутри квартиры.
/// </summary>
public class ApartmentDto
{
    [Description("ID квартиры (НЕ номер квартиры)")]
    public int Id { get; set; }
    [Description("Номер квартиры")]
    public string Number { get; set; }
    [Description("Описание квартиры")]
    public string Description { get; set; }

    [Description("Количество комнат")]
    public int RoomsCount { get; set; }
}

/// <summary>
/// DTO для создания новой квартиры. Содержит обязательные поля для создания квартиры, включая номер и описание. Используется при отправке запроса на создание новой квартиры клиентом. В запросе не указывается ID, так как он будет сгенерирован базой данных при сохранении новой квартиры.
/// </summary>
public class CreateApartmentRequest
{
    [Description("Номер квартиры")]
    public string Number { get; set; }

    [Description("Описание квартиры")]
    public string Description { get; set; }
}