namespace Cartify.OrderService.Entities;

public class Order
{
    public int Id { get; set; }

    public string CustomerEmail { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    // Amount breakdown
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal DeliveryCharge { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    public string Status { get; set; } = "Pending";

    // Checkout details
    public string? CouponCode { get; set; }
    public string? DeliverySlot { get; set; }
    public string? Notes { get; set; }
    public string PaymentMethod { get; set; } = "COD";
    public string PaymentStatus { get; set; } = "Pending";
    public string? ShippingAddressJson { get; set; }

    public DateTime CreatedAt { get; set; }

    // Line items serialized as JSON (productId, name, price, quantity).
    public string ItemsJson { get; set; } = "[]";
}
