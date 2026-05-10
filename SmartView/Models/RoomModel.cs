namespace SmartView.Models;

public class RoomModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string RoomType { get; set; } = null!;
    public double Area { get; set; }
    public double Temperature { get; set; }
    public bool LightState { get; set; }
}
