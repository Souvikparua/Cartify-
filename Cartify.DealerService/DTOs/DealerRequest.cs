namespace Cartify.DealerService.DTOs;

public class DealerRequest
{
    public string Email { get; set; } = "";
    public string ShopName { get; set; } = "";
    public string GstNumber { get; set; } = "";
    public double DeliveryRadiusKm { get; set; } = 5;
    public string OpenTime { get; set; } = "09:00";
    public string CloseTime { get; set; } = "21:00";
    public bool IsOpen { get; set; } = true;
}
