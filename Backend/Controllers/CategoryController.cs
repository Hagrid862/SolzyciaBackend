using System.Text.Json;
using Backend.Middlewares;
using Backend.Models;
using Backend.Services;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
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
        return Ok(JsonSerializer.Serialize(categories));
    }

    [HttpPost]
    [AuthenticateAdminTokenMiddleware]
    public async Task<IActionResult> AddCategory([FromBody] AddCategoryModel model)
    {
        var result = await _categoryService.AddCategory(model);
        if (result == "SUCCESS")
        {
            return Ok(JsonSerializer.Serialize(new { Message = "Category added successfully" }));
        }
        else
        {
            return BadRequest(JsonSerializer.Serialize(new { Message = "Something went wrong" }));
        }
    }

    [HttpPut]
    [AuthenticateAdminTokenMiddleware]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryModel model)
    {
        var result = await _categoryService.UpdateCategory(model);
        Console.WriteLine(result);
        if (result == "SUCCESS")
        {
            return Ok(JsonSerializer.Serialize(new { Message = "Category updated successfully" }));
        }
        else if (result == "NOTFOUND")
        {
            return NotFound(JsonSerializer.Serialize(new { Message = "Category not found" }));
        }
        else
        {
            return BadRequest(JsonSerializer.Serialize(new { Message = "Something went wrong" }));
        }
    }

    [HttpDelete("{id}")]
    [AuthenticateAdminTokenMiddleware]
    public async Task<IActionResult> DeleteCategory(long id)
    {
        var result = await _categoryService.DeleteCategory(id);
        if (result == "SUCCESS")
        {
            return Ok(JsonSerializer.Serialize(new { Message = "Category deleted successfully" }));
        }
        else if (result == "NOTFOUND")
        {
            return NotFound(JsonSerializer.Serialize(new { Message = "Category not found" }));
        }
        else
        {
            return BadRequest(JsonSerializer.Serialize(new { Message = "Something went wrong" }));
        }
    }
}