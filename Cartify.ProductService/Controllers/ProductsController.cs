using Cartify.ProductService.Data;
using Cartify.ProductService.DTOs;
using Cartify.ProductService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cartify.ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductDbContext _context;

    public ProductsController(ProductDbContext context)
    {
        _context = context;
    }

    // GET /api/products  (public — used by the storefront)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(products);
    }

    // GET /api/products/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    // POST /api/products  (admin — add a product)
    [HttpPost]
    public async Task<IActionResult> Create(ProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Product name is required.");
        }

        var product = new Product
        {
            Name = request.Name,
            Category = string.IsNullOrWhiteSpace(request.Category) ? "General" : request.Category,
            Price = request.Price,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Barcode = request.Barcode,
            Stock = request.Stock,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT /api/products/5  (admin — update a product)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProductRequest request)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        product.Name = request.Name;
        product.Category = request.Category;
        product.Price = request.Price;
        product.Description = request.Description;
        product.ImageUrl = request.ImageUrl;
        product.Barcode = request.Barcode;
        product.Stock = request.Stock;

        await _context.SaveChangesAsync();
        return Ok(product);
    }

    // DELETE /api/products/5  (admin — remove a product)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
