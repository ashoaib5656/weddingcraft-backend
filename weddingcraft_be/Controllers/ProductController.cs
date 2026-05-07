using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Dtos;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace WeddingCraft.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductController(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
    {
        var pagedProducts = await _productService.GetAllAsync(filter);
        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(pagedProducts.Data);

        var response = new PagedResponse<IEnumerable<ProductDto>>(
            productDtos ?? Array.Empty<ProductDto>(), 
            pagedProducts.PageNumber, 
            pagedProducts.PageSize, 
            pagedProducts.TotalRecords, 
            "Products retrieved successfully."
        );
        return Ok(response);
    }

    [HttpGet("vendor/{vendorId}")]
    public async Task<IActionResult> GetByVendor(Guid vendorId, [FromQuery] PaginationFilter filter)
    {
        var pagedProducts = await _productService.GetByVendorIdAsync(vendorId, filter);
        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(pagedProducts.Data);

        var response = new PagedResponse<IEnumerable<ProductDto>>(
            productDtos ?? Array.Empty<ProductDto>(),
            pagedProducts.PageNumber,
            pagedProducts.PageSize,
            pagedProducts.TotalRecords,
            "Vendor products retrieved successfully."
        );
        return Ok(response);
    }

    [Authorize(Roles = "Vendor")]
    [HttpGet("mine")]
    public async Task<IActionResult> GetMine([FromQuery] PaginationFilter filter)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var pagedProducts = await _productService.GetByVendorIdAsync(userId, filter);
        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(pagedProducts.Data);

        var response = new PagedResponse<IEnumerable<ProductDto>>(
            productDtos ?? Array.Empty<ProductDto>(),
            pagedProducts.PageNumber,
            pagedProducts.PageSize,
            pagedProducts.TotalRecords,
            "My products retrieved successfully."
        );
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) 
            return NotFound(ApiResponse<object>.Fail("Product not found."));
            
        var dto = _mapper.Map<ProductDto>(product);
        return Ok(ApiResponse<ProductDto>.SuccessResponse(dto, "Product details retrieved."));
    }

    [Authorize(Roles = "Vendor")]
    [HttpPost]
    public async Task<IActionResult> Create(ProductDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var product = _mapper.Map<Product>(dto);
        product.VendorId = userId;
        
        var created = await _productService.CreateAsync(product);
        var resultDto = _mapper.Map<ProductDto>(created);
        
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<ProductDto>.SuccessResponse(resultDto, "Product created successfully."));
    }

    [Authorize(Roles = "Vendor")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var existing = await _productService.GetByIdAsync(id);
        
        if (existing == null) return NotFound(ApiResponse<object>.Fail("Product not found."));
        if (existing.VendorId != userId) return Forbid();
        
        var product = _mapper.Map<Product>(dto);
        await _productService.UpdateAsync(id, product);
        
        return Ok(ApiResponse<object>.SuccessResponse(null, "Product updated successfully."));
    }

    [Authorize(Roles = "Vendor")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var existing = await _productService.GetByIdAsync(id);
        
        if (existing == null) return NotFound(ApiResponse<object>.Fail("Product not found."));
        if (existing.VendorId != userId) return Forbid();
        
        await _productService.DeleteAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Product deleted successfully."));
    }
}
