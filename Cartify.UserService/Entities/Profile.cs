namespace Cartify.UserService.Entities;

public class Profile
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    // Saved delivery addresses, serialized as JSON.
    public string AddressesJson { get; set; } = "[]";
}
