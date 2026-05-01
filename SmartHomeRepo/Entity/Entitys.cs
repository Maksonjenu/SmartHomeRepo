using System.Text.Json.Serialization;

namespace SmartHomeRepo.Entitys;


public class Apartment {
    public int Id { get; set; }
    public string Number { get; set; } = "";
    public string Description { get; set; } = "";
    public List<Room> Rooms { get; set; } = new();
}

public class Room {
    public int Id { get; set; }
    public int ApartmentId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string RoomType { get; set; } = ""; // Kitchen, Bedroom...

    [JsonIgnore] // Чтобы не было циклической ссылки
    public Apartment? Apartment { get; set; }
    public RoomInfo? Info { get; set; }
}

public class RoomInfo {
    public int Id { get; set; }
    public int RoomId { get; set; }
    public double Area { get; set; }
    public double Temperature { get; set; }
    public bool LightState { get; set; }

    [JsonIgnore]
    public Room? Room { get; set; }
}