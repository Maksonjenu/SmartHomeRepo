using System.ComponentModel;


namespace SmartHomeRepo.DTO;

public class FlatRoomDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string RoomType { get; set; } = null!;
    public int ApartmentId { get; set; }

}


public class UpdateRoomDto
{
    [Description("ID комнаты")]
    public int Id { get; set; }
    [Description("Название комнаты")]
    public string? Name { get; set; }
    [Description("Описание комнаты")]
    public string? Description { get; set; }

    [Description("Тип комнаты")]
    public string? RoomType { get; set; }
    [Description("Площадь комнаты в квадратных метрах")]
    public double? Area { get; set; }
    [Description("Температура в комнате в градусах Цельсия")]
    public double? Temperature { get; set; }
    [Description("Состояние света в комнате (true - включен, false - выключен)")]
    public bool? LightState { get; set; }

}

public class CreateRoomDto
{
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

public class CreateApartmentRequest
{
    [Description("Номер квартиры")]
    public string Number { get; set; }

    [Description("Описание квартиры")]
    public string Description { get; set; }
}