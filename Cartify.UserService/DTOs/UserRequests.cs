namespace Cartify.UserService.DTOs;

public class UpdateProfileRequest
{
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
}

public class SaveAddressesRequest
{
    public string Email { get; set; } = "";
    public List<AddressDto> Addresses { get; set; } = new();
}

public class AddressDto
{
    public string Label { get; set; } = "";
    public string Line1 { get; set; } = "";
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string Zip { get; set; } = "";
    public string Phone { get; set; } = "";
    public bool IsDefault { get; set; }
}
