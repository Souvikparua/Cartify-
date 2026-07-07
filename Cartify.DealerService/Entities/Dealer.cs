namespace Cartify.DealerService.Entities;

public class Dealer
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string ShopName { get; set; } = "";
    public string GstNumber { get; set; } = "";
    public double DeliveryRadiusKm { get; set; } = 5;
    public string OpenTime { get; set; } = "09:00";
    public string CloseTime { get; set; } = "21:00";
    public bool IsOpen { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
