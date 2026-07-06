using Cartify.PromotionsService.Data;
using Cartify.PromotionsService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cartify.PromotionsService.Controllers;

[ApiController]
[Route("api/promotions/[controller]")]
public class CouponsController : ControllerBase
{
    private readonly PromotionsDbContext _context;
    public CouponsController(PromotionsDbContext context) => _context = context;

    // GET /api/promotions/coupons
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _context.Coupons.OrderByDescending(c => c.Id).ToListAsync());

    // POST /api/promotions/coupons
    [HttpPost]
    public async Task<IActionResult> Create(Coupon coupon)
    {
        if (string.IsNullOrWhiteSpace(coupon.Code)) return BadRequest("Coupon code is required.");
        coupon.Id = 0;
        coupon.Code = coupon.Code.Trim().ToUpperInvariant();
        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();
        return Ok(coupon);
    }

    // DELETE /api/promotions/coupons/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await _context.Coupons.FindAsync(id);
        if (c is null) return NotFound();
        _context.Coupons.Remove(c);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
