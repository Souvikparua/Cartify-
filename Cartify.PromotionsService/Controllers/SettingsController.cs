using Cartify.PromotionsService.Data;
using Cartify.PromotionsService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cartify.PromotionsService.Controllers;

[ApiController]
[Route("api/promotions/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly PromotionsDbContext _context;
    public SettingsController(PromotionsDbContext context) => _context = context;

    private async Task<StoreSetting> GetRow()
    {
        var s = await _context.Settings.FirstOrDefaultAsync();
        if (s is null)
        {
            s = new StoreSetting();
            _context.Settings.Add(s);
            await _context.SaveChangesAsync();
        }
        return s;
    }

    // GET /api/promotions/settings
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await GetRow());

    // PUT /api/promotions/settings
    [HttpPut]
    public async Task<IActionResult> Update(StoreSetting incoming)
    {
        var s = await GetRow();
        s.DeliveryCharge = incoming.DeliveryCharge;
        s.FreeShipThreshold = incoming.FreeShipThreshold;
        s.TaxPercent = incoming.TaxPercent;
        s.GstPercent = incoming.GstPercent;
        s.CodEnabled = incoming.CodEnabled;
        s.CardEnabled = incoming.CardEnabled;
        s.UpiEnabled = incoming.UpiEnabled;
        s.EmailFrom = incoming.EmailFrom;
        s.SmtpHost = incoming.SmtpHost;
        s.SmsProvider = incoming.SmsProvider;
        s.SmsKey = incoming.SmsKey;
        await _context.SaveChangesAsync();
        return Ok(s);
    }
}
