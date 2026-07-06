namespace Cartify.PromotionsService.Entities;

public class Coupon
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Type { get; set; } = "percent"; // percent | flat | free_shipping
    public decimal Value { get; set; }
    public decimal MinOrder { get; set; }
    public string? Expiry { get; set; }
}

public class Banner
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Subtitle { get; set; }
    public string Type { get; set; } = "Homepage";
    public string Color { get; set; } = "#3bb77e";
    public bool Active { get; set; } = true;
}

public class StoreSetting
{
    public int Id { get; set; }
    public decimal DeliveryCharge { get; set; } = 5;
    public decimal FreeShipThreshold { get; set; } = 50;
    public decimal TaxPercent { get; set; } = 8;
    public decimal GstPercent { get; set; } = 18;
    public bool CodEnabled { get; set; } = true;
    public bool CardEnabled { get; set; } = true;
    public bool UpiEnabled { get; set; } = true;
    public string EmailFrom { get; set; } = "store@cartify.com";
    public string SmtpHost { get; set; } = "";
    public string SmsProvider { get; set; } = "None";
    public string SmsKey { get; set; } = "";
}
