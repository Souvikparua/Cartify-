using Cartify.PromotionsService.Data;
using Cartify.PromotionsService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cartify.PromotionsService.Controllers;

[ApiController]
[Route("api/promotions/[controller]")]
public class BannersController : ControllerBase
{
    private readonly PromotionsDbContext _context;
    public BannersController(PromotionsDbContext context) => _context = context;

    // GET /api/promotions/banners
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _context.Banners.OrderByDescending(b => b.Id).ToListAsync());

    // POST /api/promotions/banners
    [HttpPost]
    public async Task<IActionResult> Create(Banner banner)
    {
        if (string.IsNullOrWhiteSpace(banner.Title)) return BadRequest("Banner title is required.");
        banner.Id = 0;
        _context.Banners.Add(banner);
        await _context.SaveChangesAsync();
        return Ok(banner);
    }

    // DELETE /api/promotions/banners/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var b = await _context.Banners.FindAsync(id);
        if (b is null) return NotFound();
        _context.Banners.Remove(b);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
