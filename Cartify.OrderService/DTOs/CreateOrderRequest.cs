namespace Cartify.OrderService.DTOs;

public class CreateOrderRequest
{
    public string CustomerEmail { get; set; } = "";

    public string CustomerName { get; set; } = "";

    public List<OrderItemDto> Items { get; set; } = new();

    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal DeliveryCharge { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    public string? CouponCode { get; set; }
    public string? DeliverySlot { get; set; }
    public string? Notes { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentStatus { get; set; }
    public OrderAddressDto? ShippingAddress { get; set; }
}

public class AssignOrderRequest
{
    public string? DealerEmail { get; set; }
    public string? DeliveryPartnerEmail { get; set; }
    public string? Status { get; set; }
}

public class OrderItemDto
{
    public int ProductId { get; set; }

    public string Name { get; set; } = "";

    public decimal Price { get; set; }

    public int Quantity { get; set; }
}

public class OrderAddressDto
{
    public string Label { get; set; } = "";
    public string Line1 { get; set; } = "";
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string Zip { get; set; } = "";
    public string Phone { get; set; } = "";
}
