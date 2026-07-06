using System.Text.Json;
using Cartify.CartService.Data;
using Cartify.CartService.DTOs;
using Cartify.CartService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cartify.CartService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartDbContext _context;

    public CartController(CartDbContext context)
    {
        _context = context;
    }

    // GET /api/cart/{email}  -> the customer's saved cart items
    [HttpGet("{email}")]
    public async Task<IActionResult> Get(string email)
    {
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerEmail == email);
        return Content(cart?.ItemsJson ?? "[]", "application/json");
    }

    // PUT /api/cart/{email}  -> replace the customer's cart
    [HttpPut("{email}")]
    public async Task<IActionResult> Save(string email, CartRequest request)
    {
        var json = JsonSerializer.Serialize(request.Items);
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerEmail == email);
        if (cart is null)
        {
            cart = new Cart { CustomerEmail = email, ItemsJson = json, UpdatedAt = DateTime.UtcNow };
            _context.Carts.Add(cart);
        }
        else
        {
            cart.ItemsJson = json;
            cart.UpdatedAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
        return Ok(new { count = request.Items.Count });
    }

    // DELETE /api/cart/{email}  -> empty the cart
    [HttpDelete("{email}")]
    public async Task<IActionResult> Clear(string email)
    {
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerEmail == email);
        if (cart is not null)
        {
            cart.ItemsJson = "[]";
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        return NoContent();
    }
}
