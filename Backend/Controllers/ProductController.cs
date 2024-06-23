using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backend.Dto;
using Backend.Middlewares;
using Backend.Services;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    [AuthenticateAdminTokenMiddleware]
    public async Task<IActionResult> CreateProduct([FromForm] AddProductModel model)
    {
        long? userId = HttpContext.Items["userId"] as long?;
        if (userId == null)
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "UNAUTHORIZED", Message = "Unauthorized" }));
        }
        var result = await _productService.AddProduct(model, userId);
        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { Status = "SUCCESS", Message = "Product added successfully" }));
        }
        else
        {
            if (result.status == "UNAUTHORIZED")
            {
                return Unauthorized(JsonSerializer.Serialize(new { Status = "UNAUTHORIZED", Message = "Unauthorized" }));
            }
            else if (result.status == "CATEGORYNOTFOUND")
            {
                return NotFound(JsonSerializer.Serialize(new { Status = "CATEGORYNOTFOUND", Message = "Category not found" }));
            }
            else if (result.status == "INVALIDIMAGEFORMAT")
            {
                return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDIMAGEFORMAT", Message = "Invalid image format" }));
            }
            else
            {
                return BadRequest(JsonSerializer.Serialize(new { Status = "INTERNAL", Message = "Something went wrong" }));
            }
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] bool reviews = false, [FromQuery] string orderBy = "created_at", [FromQuery] string order = "desc", [FromQuery] int page = 1, [FromQuery] int limit = 25)
    {
        if (orderBy is not ("created_at" or "price" or "name" or "rating" or "popularity"))
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDORDERBY", Message = "Invalid orderBy parameter" }));
        }
        else if (order is not ("desc" or "asc"))
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDORDER", Message = "Invalid order parameter" }));
        }
        else if (page < 1)
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDPAGE", Message = "Invalid page parameter" }));
        }
        else if (limit < 1)
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDLIMIT", Message = "Invalid limit parameter" }));
        }

        var result = await _productService.GetAllProducts(reviews, orderBy, order, page, limit);

        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { Status = "SUCCESS", Products = result.dtos }));
        }
        else
        {
            if (result.status == "NOTFOUND")
            {
                return NotFound(JsonSerializer.Serialize(new { Status = "NOTFOUND", Message = "No products found" }));
            }
            else
            {
                return BadRequest(JsonSerializer.Serialize(new { Status = "INTERNAL", Message = "Something went wrong" }));
            }
        }

    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetProductsByCategory(string categoryId, [FromQuery] bool reviews = false, [FromQuery] string orderBy = "created_at", [FromQuery] string order = "desc", [FromQuery] int page = 1, [FromQuery] int limit = 25)
    {
        if (orderBy is not ("created_at" or "price" or "name" or "rating" or "popularity"))
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDORDERBY", Message = "Invalid orderBy parameter" }));
        }
        else if (order is not ("desc" or "asc"))
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDORDER", Message = "Invalid order parameter" }));
        }
        else if (page < 1)
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDPAGE", Message = "Invalid page parameter" }));
        }
        else if (limit < 1)
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDLIMIT", Message = "Invalid limit parameter" }));
        }

        var result = await _productService.GetProductsByCategory(categoryId, reviews, orderBy, order, page, limit);

        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { Status = "SUCCESS", Products = result.dtos }));
        }
        else
        {
            if (result.status == "NOTFOUND")
            {
                return NotFound(JsonSerializer.Serialize(new { Status = "NOTFOUND", Message = "No products found" }));
            }
            else
            {
                return BadRequest(JsonSerializer.Serialize(new { Status = "INTERNAL", Message = "Something went wrong" }));
            }
        }
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductById(string productId)
    {
        var result = await _productService.GetProductById(productId);
        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { Status = "SUCCESS", Product = result.dto }));
        }
        else
        {
            if (result.status == "NOTFOUND")
            {
                return NotFound(JsonSerializer.Serialize(new { Status = "NOTFOUND", Message = "Product not found" }));
            }
            else
            {
                return BadRequest(JsonSerializer.Serialize(new { Status = "INTERNAL", Message = "Something went wrong" }));
            }
        }
    }

    [HttpPut("{productId}")]
    [AuthenticateAdminTokenMiddleware]
    public async Task<IActionResult> UpdateProduct([FromRoute] string productId, [FromForm] UpdateProductModel model)
    {
        long? userId = HttpContext.Items["userId"] as long?;
        if (userId == null || userId == -1)
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "UNAUTHORIZED", Message = "Unauthorized" }));
        }
        var result = await _productService.UpdateProduct(productId, model);
        if (result.status == "SUCCESS")
        {
            return Ok(JsonSerializer.Serialize(new { Status = "SUCCESS", Message = "Product updated successfully" }));
        }
        else if (result.status == "NOTFOUND")
        {
            return NotFound(JsonSerializer.Serialize(new { Status = "NOTFOUND", Message = "Product not found" }));
        }
        else
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INTERNAL", Message = "Something went wrong" }));
        }
    }
}