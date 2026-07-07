namespace Cartify.ProductService.Entities;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Description { get; set; } = string.Empty;

    // Holds either an http(s) image URL or a base64 data URL uploaded by the admin.
    public string? ImageUrl { get; set; }

    // Barcode / SKU used for barcode search.
    public string? Barcode { get; set; }

    // Owning dealer/seller (null = platform-owned).
    public string? DealerEmail { get; set; }

    public int Stock { get; set; }

    public DateTime CreatedAt { get; set; }
}
