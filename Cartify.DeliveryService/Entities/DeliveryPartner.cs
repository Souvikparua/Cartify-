namespace Cartify.DeliveryService.Entities;

public class DeliveryPartner
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = "";
    public string VehicleType { get; set; } = "Bike";
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
