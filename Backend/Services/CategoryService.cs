using Backend.Data;
using Backend.Dto;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class CategoryService : ICategoryService
{
    private readonly MainDbContext _context;

    public CategoryService(MainDbContext context)
    {
        _context = context;
    }

    public async Task<(bool isSuccess, string status, List<CategoryDto>? categories)> GetCategories()
    {
        try
        {
            var categories = await _context.Categories.ToListAsync();
            var dtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id.ToString(),
                Name = c.Name,
                Icon = c.Icon,
                Description = c.Description,
                CreatedAt = c.CreatedAt
            }).ToList();
            return (true, "SUCCESS", dtos);
        }
        catch
        {
            return (false, "ERROR", null);
        }
    }

    public async Task<(bool isSuccess, string status)> AddCategory(AddCategoryModel model)
    {
        try
        {
            Category category = new Category
            {
                Id = Helpers.Snowflake.GenerateId(),
                Name = model.Name,
                Description = model.Description,
                Icon = model.Icon,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return (true, "SUCCESS");
        }
        catch (Exception e)
        {
            return (false, "ERROR");
        }
    }

    public async Task<(bool isSuccess, string status)> UpdateCategory(UpdateCategoryModel model)
    {
        try
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == long.Parse(model.Id));
            if (category == null)
                return (false, "NOTFOUND");

            category.Name = model.Name;
            category.Description = model.Description;
            category.Icon = model.Icon;
            await _context.SaveChangesAsync();
            return (true, "SUCCESS");
        }
        catch (Exception e)
        {
            return (false, "ERROR");
        }
    }

    public async Task<(bool isSuccess, string status)> DeleteCategory(long id)
    {
        try
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return (false, "NOTFOUND");

            Console.WriteLine(category.Name);

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return (true, "SUCCESS");
        }
        catch (Exception e)
        {
            return (false, "ERROR");
        }
    }
}

public interface ICategoryService
{
    Task<(bool isSuccess, string status, List<CategoryDto>? categories)> GetCategories();
    Task<(bool isSuccess, string status)> AddCategory(AddCategoryModel category);
    Task<(bool isSuccess, string status)> UpdateCategory(UpdateCategoryModel category);
    Task<(bool isSuccess, string status)> DeleteCategory(long id);
}