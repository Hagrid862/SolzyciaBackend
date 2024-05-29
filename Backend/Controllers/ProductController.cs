using System.Text;
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
            // Handle the error
            return BadRequest(new { message = "User ID not found in the request" });
        }
        var result = await _productService.AddProduct(model, userId);
        if (result == "SUCCESS")
        {
            return Ok(new { message = "Product added successfully" });
        }
        else if (result == "NOTFOUND")
        {
            return NotFound(new { message = "Category not found" });
        }
        else
        {
            return BadRequest(new { message = "Something went wrong" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] bool reviews = false, [FromQuery] string orderBy = "created_at", [FromQuery] string order = "desc", [FromQuery] int page = 1, [FromQuery] int limit = 25)
    {
        if (orderBy is not ("created_at" or "price" or "name" or "rating" or "popularity"))
        {
            return BadRequest(new { message = "Invalid orderBy parameter" });
        }
        else if (order is not ("desc" or "asc"))
        {
            return BadRequest(new { message = "Invalid order parameter" });
        }
        else if (page < 1)
        {
            return BadRequest(new { message = "Invalid page parameter" });
        }
        else if (limit < 1)
        {
            return BadRequest(new { message = "Invalid limit parameter" });
        }

        List<ProductDto> products = await _productService.GetAllProducts(reviews, orderBy, order, page, limit);

        if (products == null)
        {
            return NotFound(new
            {
                message = "No products found"
            });
        }
        else
        {
            return Ok(new
            {
                products = products
            });
        }

    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetProductsByCategory(string categoryId, [FromQuery] bool reviews = false, [FromQuery] string orderBy = "created_at", [FromQuery] string order = "desc", [FromQuery] int page = 1, [FromQuery] int limit = 25)
    {
        if (orderBy is not ("created_at" or "price" or "name" or "rating" or "popularity"))
        {
            return BadRequest(new { message = "Invalid orderBy parameter" });
        }
        else if (order is not ("desc" or "asc"))
        {
            return BadRequest(new { message = "Invalid order parameter" });
        }
        else if (page < 1)
        {
            return BadRequest(new { message = "Invalid page parameter" });
        }
        else if (limit < 1)
        {
            return BadRequest(new { message = "Invalid limit parameter" });
        }

        List<ProductDto> products = await _productService.GetProductsByCategory(categoryId, reviews, orderBy, order, page, limit);

        if (products == null)
        {
            return NotFound(new
            {
                message = "No products found"
            });
        }
        else
        {
            return Ok(new
            {
                products = products
            });
        }
    }
}