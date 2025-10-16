using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Data;

namespace WeddingCraft.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public ProductController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Products.Include(p => p.CustomizationOptions).ToListAsync());
}
