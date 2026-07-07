using Cartify.DeliveryService.Data;
using Cartify.DeliveryService.DTOs;
using Cartify.DeliveryService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cartify.DeliveryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveryController : ControllerBase
{
    private readonly DeliveryDbContext _context;
    public DeliveryController(DeliveryDbContext context) => _context = context;

    private async Task<DeliveryPartner> GetOrCreate(string email)
    {
        var p = await _context.Partners.FirstOrDefaultAsync(x => x.Email == email);
        if (p is null)
        {
            p = new DeliveryPartner { Email = email, Name = email.Split('@')[0] };
            _context.Partners.Add(p);
            await _context.SaveChangesAsync();
        }
        return p;
    }

    // GET /api/delivery  (all partners)
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _context.Partners.OrderByDescending(p => p.Id).ToListAsync());

    // GET /api/delivery/{email}
    [HttpGet("{email}")]
    public async Task<IActionResult> Get(string email) => Ok(await GetOrCreate(email));

    // PUT /api/delivery  (upsert partner profile)
    [HttpPut]
    public async Task<IActionResult> Save(DeliveryPartnerRequest request)
    {
        var p = await GetOrCreate(request.Email);
        p.Name = request.Name;
        p.VehicleType = request.VehicleType;
        await _context.SaveChangesAsync();
        return Ok(p);
    }

    // PUT /api/delivery/{email}/availability  (online/offline toggle)
    [HttpPut("{email}/availability")]
    public async Task<IActionResult> SetAvailability(string email, [FromBody] bool available)
    {
        var p = await GetOrCreate(email);
        p.IsAvailable = available;
        await _context.SaveChangesAsync();
        return Ok(new { p.Email, p.IsAvailable });
    }
}
