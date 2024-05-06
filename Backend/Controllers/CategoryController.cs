using Backend.Middlewares;
using Backend.Models;
using Backend.Services;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController: ControllerBase
{
    private readonly ICategoryService _categoryService;
    
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    
    [HttpGet]
    [AuthenticateAdminTokenMiddleware]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _categoryService.GetCategories();
        return Ok(categories);
    }
    
    [HttpPost]
    [AuthenticateAdminTokenMiddleware]
    public async Task<IActionResult> AddCategory([FromBody] AddCategoryModel model)
    {
        var result = await _categoryService.AddCategory(model);
        if (result == "SUCCESS")
        {
            return Ok(new { message = "Category added successfully" });
        }
        else
        {
            return BadRequest(new { message = "Something went wrong" });
        }
    }
}