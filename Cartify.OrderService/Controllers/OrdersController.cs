using System.Text.Json;
using Cartify.OrderService.Data;
using Cartify.OrderService.DTOs;
using Cartify.OrderService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cartify.OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _context;

    public OrdersController(OrderDbContext context)
    {
        _context = context;
    }

    // POST /api/orders  (customer places an order at checkout)
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        if (request.Items is null || request.Items.Count == 0)
        {
            return BadRequest("Order must contain at least one item.");
        }

        var itemsTotal = request.Items.Sum(i => i.Price * i.Quantity);

        var order = new Order
        {
            CustomerEmail = request.CustomerEmail,
            CustomerName = request.CustomerName,
            Subtotal = request.Subtotal > 0 ? request.Subtotal : itemsTotal,
            Discount = request.Discount,
            DeliveryCharge = request.DeliveryCharge,
            Tax = request.Tax,
            Total = request.Total > 0 ? request.Total : itemsTotal,
            Status = "Pending",
            CouponCode = request.CouponCode,
            DeliverySlot = request.DeliverySlot,
            Notes = request.Notes,
            PaymentMethod = string.IsNullOrWhiteSpace(request.PaymentMethod) ? "COD" : request.PaymentMethod,
            PaymentStatus = string.IsNullOrWhiteSpace(request.PaymentStatus) ? "Pending" : request.PaymentStatus,
            ShippingAddressJson = request.ShippingAddress is null ? null : JsonSerializer.Serialize(request.ShippingAddress),
            CreatedAt = DateTime.UtcNow,
            ItemsJson = JsonSerializer.Serialize(request.Items)
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, Project(order));
    }

    // GET /api/orders  (admin — all customer orders, newest first)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return Ok(orders.Select(Project));
    }

    // GET /api/orders/user/{email}  (a customer's own orders)
    [HttpGet("user/{email}")]
    public async Task<IActionResult> GetByUser(string email)
    {
        var orders = await _context.Orders
            .Where(o => o.CustomerEmail == email)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return Ok(orders.Select(Project));
    }

    // GET /api/orders/cooccurrence  (AI: product popularity + "bought together" counts)
    [HttpGet("cooccurrence")]
    public async Task<IActionResult> Cooccurrence()
    {
        var orders = await _context.Orders.ToListAsync();
        var popularity = new Dictionary<int, int>();
        var pairs = new Dictionary<(int, int), int>();

        foreach (var o in orders)
        {
            var items = JsonSerializer.Deserialize<List<OrderItemDto>>(o.ItemsJson) ?? new();
            var ids = items.Select(i => i.ProductId).Where(id => id > 0).Distinct().ToList();

            foreach (var id in ids)
                popularity[id] = popularity.GetValueOrDefault(id) + 1;

            for (var i = 0; i < ids.Count; i++)
                for (var j = 0; j < ids.Count; j++)
                    if (i != j)
                    {
                        var key = (ids[i], ids[j]);
                        pairs[key] = pairs.GetValueOrDefault(key) + 1;
                    }
        }

        var cooccurrence = pairs
            .GroupBy(kv => kv.Key.Item1)
            .ToDictionary(
                g => g.Key.ToString(),
                g => g.OrderByDescending(x => x.Value)
                      .Select(x => new { productId = x.Key.Item2, count = x.Value })
                      .ToList());

        return Ok(new
        {
            popularity = popularity.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value),
            cooccurrence
        });
    }

    // GET /api/orders/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        return order is null ? NotFound() : Ok(Project(order));
    }

    // PUT /api/orders/5/status  (admin — update order status)
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is null)
        {
            return NotFound();
        }

        order.Status = status;
        await _context.SaveChangesAsync();
        return Ok(Project(order));
    }

    private static readonly JsonSerializerOptions CaseInsensitive = new() { PropertyNameCaseInsensitive = true };

    private static object Project(Order o) => new
    {
        o.Id,
        o.CustomerEmail,
        o.CustomerName,
        o.Subtotal,
        o.Discount,
        o.DeliveryCharge,
        o.Tax,
        o.Total,
        o.Status,
        o.CouponCode,
        o.DeliverySlot,
        o.Notes,
        o.PaymentMethod,
        o.PaymentStatus,
        o.CreatedAt,
        ShippingAddress = string.IsNullOrWhiteSpace(o.ShippingAddressJson)
            ? null
            : JsonSerializer.Deserialize<OrderAddressDto>(o.ShippingAddressJson, CaseInsensitive),
        Items = JsonSerializer.Deserialize<List<OrderItemDto>>(o.ItemsJson) ?? new List<OrderItemDto>()
    };
}
