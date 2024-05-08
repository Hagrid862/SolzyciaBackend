using Backend.Data;
using Backend.Dto;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class CategoryService: ICategoryService
{
    private readonly MainDbContext _context;
    
    public CategoryService(MainDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<CategoryDto>> GetCategories()
    {
        var categories = await _context.Categories.ToListAsync();
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id.ToString(),
            Name = c.Name,
            Icon = c.Icon,
            Description = c.Description,
            CreatedAt = c.CreatedAt
        }).ToList();
    }
    
    public async Task<string> AddCategory(AddCategoryModel model)
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
            return "SUCCESS";
        } catch (Exception e)
        {
            return "ERROR";
        }
    }
    
    public async Task<string> UpdateCategory(UpdateCategoryModel model)
    {
        try
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == long.Parse(model.Id));
            if (category == null)
                return "NOTFOUND";
            
            category.Name = model.Name;
            category.Description = model.Description;
            category.Icon = model.Icon;
            await _context.SaveChangesAsync();
            return "SUCCESS";
        } catch (Exception e)
        {
            return "ERROR";
        }
    }
    
    public async Task<string> DeleteCategory(long id)
    {
        try
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return "NOTFOUND";
            
            Console.WriteLine(category.Name);
            
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return "SUCCESS";
        } catch (Exception e)
        {
            return "ERROR";
        }
    }
}

public interface ICategoryService
{
    Task<List<CategoryDto>> GetCategories();
    Task<string> AddCategory(AddCategoryModel category);
    Task<string> UpdateCategory(UpdateCategoryModel category);
    Task<string> DeleteCategory(long id);
}