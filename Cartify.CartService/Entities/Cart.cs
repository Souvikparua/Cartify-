namespace Cartify.CartService.Entities;

public class Cart
{
    public int Id { get; set; }

    public string CustomerEmail { get; set; } = string.Empty;

    // Cart line items, serialized exactly as the client stores them.
    public string ItemsJson { get; set; } = "[]";

    public DateTime UpdatedAt { get; set; }
}
