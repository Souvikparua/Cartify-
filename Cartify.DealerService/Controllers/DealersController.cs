using Cartify.DealerService.Data;
using Cartify.DealerService.DTOs;
using Cartify.DealerService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cartify.DealerService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DealersController : ControllerBase
{
    private readonly DealerDbContext _context;
    public DealersController(DealerDbContext context) => _context = context;

    private async Task<Dealer> GetOrCreate(string email)
    {
        var d = await _context.Dealers.FirstOrDefaultAsync(x => x.Email == email);
        if (d is null)
        {
            d = new Dealer { Email = email, ShopName = email.Split('@')[0] + "'s Shop" };
            _context.Dealers.Add(d);
            await _context.SaveChangesAsync();
        }
        return d;
    }

    // GET /api/dealers  (admin — all dealers)
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _context.Dealers.OrderByDescending(d => d.Id).ToListAsync());

    // GET /api/dealers/{email}
    [HttpGet("{email}")]
    public async Task<IActionResult> Get(string email) => Ok(await GetOrCreate(email));

    // PUT /api/dealers  (upsert dealer profile: shop, GST, radius, hours)
    [HttpPut]
    public async Task<IActionResult> Save(DealerRequest request)
    {
        var d = await GetOrCreate(request.Email);
        d.ShopName = request.ShopName;
        d.GstNumber = request.GstNumber;
        d.DeliveryRadiusKm = request.DeliveryRadiusKm;
        d.OpenTime = request.OpenTime;
        d.CloseTime = request.CloseTime;
        d.IsOpen = request.IsOpen;
        await _context.SaveChangesAsync();
        return Ok(d);
    }

    // PUT /api/dealers/{email}/status  (open/closed toggle)
    [HttpPut("{email}/status")]
    public async Task<IActionResult> SetOpen(string email, [FromBody] bool isOpen)
    {
        var d = await GetOrCreate(email);
        d.IsOpen = isOpen;
        await _context.SaveChangesAsync();
        return Ok(new { d.Email, d.IsOpen });
    }
}
