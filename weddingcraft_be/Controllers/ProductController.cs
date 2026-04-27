using Microsoft.AspNetCore.Mvc;
using weddingcraft_be.Interfaces.Services;

namespace WeddingCraft.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService) => _productService = productService;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _productService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }
}
