using System.Text.Json;
using Cartify.UserService.Data;
using Cartify.UserService.DTOs;
using Cartify.UserService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cartify.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private static readonly JsonSerializerOptions CamelCase = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly UserDbContext _context;

    public UsersController(UserDbContext context)
    {
        _context = context;
    }

    private async Task<Profile> GetOrCreate(string email)
    {
        var p = await _context.Profiles.FirstOrDefaultAsync(x => x.Email == email);
        if (p is null)
        {
            p = new Profile { Email = email };
            _context.Profiles.Add(p);
            await _context.SaveChangesAsync();
        }
        return p;
    }

    // GET /api/users/profile/{email}
    [HttpGet("profile/{email}")]
    public async Task<IActionResult> GetProfile(string email)
    {
        var p = await GetOrCreate(email);
        return Ok(new { p.Email, p.Phone });
    }

    // PUT /api/users/profile  (contact phone)
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        var p = await GetOrCreate(request.Email);
        p.Phone = request.Phone;
        await _context.SaveChangesAsync();
        return Ok(new { p.Email, p.Phone });
    }

    // GET /api/users/addresses/{email}
    [HttpGet("addresses/{email}")]
    public async Task<IActionResult> GetAddresses(string email)
    {
        var p = await GetOrCreate(email);
        return Content(string.IsNullOrWhiteSpace(p.AddressesJson) ? "[]" : p.AddressesJson, "application/json");
    }

    // PUT /api/users/addresses
    [HttpPut("addresses")]
    public async Task<IActionResult> SaveAddresses(SaveAddressesRequest request)
    {
        var p = await GetOrCreate(request.Email);
        p.AddressesJson = JsonSerializer.Serialize(request.Addresses, CamelCase);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Addresses saved", count = request.Addresses.Count });
    }
}
