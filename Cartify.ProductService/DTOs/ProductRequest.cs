namespace Cartify.ProductService.DTOs;

public class ProductRequest
{
    public string Name { get; set; } = "";

    public string Category { get; set; } = "";

    public decimal Price { get; set; }

    public string Description { get; set; } = "";

    public string? ImageUrl { get; set; }

    public string? Barcode { get; set; }

    public int Stock { get; set; }
}
