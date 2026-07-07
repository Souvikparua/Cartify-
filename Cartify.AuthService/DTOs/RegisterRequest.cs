namespace Cartify.AuthService.DTOs;

public class RegisterRequest
{
    public string FullName { get; set; } = "";

    public string Email { get; set; } = "";

    public string Password { get; set; } = "";

    // Customer (default), Dealer, or DeliveryPartner.
    public string? Role { get; set; }
}
