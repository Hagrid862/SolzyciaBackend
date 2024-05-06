using Backend.Data;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class CategoryService: ICategoryService
{
    private readonly MainDbContext _context;
    
    public CategoryService(MainDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Category>> GetCategories()
    {
        return await _context.Categories.ToListAsync();
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
}

public interface ICategoryService
{
    Task<List<Category>> GetCategories();
    Task<string> AddCategory(AddCategoryModel category);
}