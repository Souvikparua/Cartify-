using System.Text.Json;

namespace Cartify.CartService.DTOs;

public class CartRequest
{
    // Opaque line items — stored and returned exactly as sent by the client.
    public List<JsonElement> Items { get; set; } = new();
}
