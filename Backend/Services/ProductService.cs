using Backend.Data;
using Backend.Dto;
using Backend.Helpers;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

    public class ProductService: IProductService
    {
        private readonly MainDbContext _context;
        
        public ProductService(MainDbContext context)
        {
            _context = context;
        }
        
    public async Task<string> AddProduct(AddProductModel model, long? userId)
    {
        try
        {
            if (userId == null)
                return "ERROR";
            
            Category? category = null;

            if (model.CategoryId != null) {
                category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == long.Parse(model.CategoryId));

                if (category == null)
                    return "NOTFOUND";
            }
            
            List<long> tagsId = [];

            if (model.Tags != null) {
                foreach (var tag in model.Tags)
                {
                    var tagEntity = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tag);
                    if (tagEntity == null)
                    {
                        var newTag = new Tag
                        {
                            Id = Snowflake.GenerateId(),
                            Name = tag,
                            Description = "",
                            CreatedAt = default
                        };
                        await _context.Tags.AddAsync(newTag);
                        await _context.SaveChangesAsync();
                        tagsId.Add(newTag.Id);
                    }
                    else
                    {
                        tagsId.Add(tagEntity.Id);
                    }
                }
            }
            
            var images = new List<IFormFile?>
            {
                model.Image0,
                model.Image1,
                model.Image2,
                model.Image3,
                model.Image4,
                model.Image5
            };
            var imagesBase64 = new List<string>();

            foreach (var image in images)
            {
                if (image == null)
                    continue;
                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !extension.Equals(".jpg") && !extension.Equals(".png") && !extension.Equals(".jpeg") && !extension.Equals(".webp"))
                {
                    throw new Exception("Invalid image format");
                }

                string mimeType = extension switch
                {
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".webp" => "image/webp",
                    _ => throw new Exception("Invalid image format")
                };

                using var ms = new MemoryStream();
                await image.CopyToAsync(ms);
                var imageBytes = ms.ToArray();

                var base64 = $"data:{mimeType};base64,{Convert.ToBase64String(imageBytes)}";
                imagesBase64.Add(base64);
            }
            
            var newProduct = new Product
            {
                Id = Snowflake.GenerateId(),
                Name = model.Name,
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                Images = imagesBase64,
                CreatedAt = DateTime.UtcNow,
                Category = category,
                Tags = await _context.Tags.Where(t => tagsId.Contains(t.Id)).ToListAsync(),
            };
            
            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();
            
            return "SUCCESS";
        } catch (Exception e)
        {
            Console.WriteLine(e);
            return "ERROR";
        }
    }
    
    public async Task<List<ProductDto>?> GetAllProducts(bool reviews, string orderBy, string order, int page, int limit)
    {
        try
        {
            var products = await _context.Products.Include(p => p.Category).Include(p => p.Tags).ToListAsync();
            var productsDto = new List<ProductDto>();
            
            foreach (var product in products)
            {
                var productDto = new ProductDto
                {
                    Id = product.Id.ToString(),
                    Name = product.Name,
                    Title = product.Title,
                    Description = product.Description,
                    Price = product.Price,
                    Images = product.Images,
                    CreatedAt = product.CreatedAt,
                    Category = null,
                    Tags = null,
                };
                
                if (product.Category != null)
                {
                    productDto.Category = new CategoryDto
                    {
                        Id = product.Category.Id.ToString(),
                        Name = product.Category.Name,
                        Description = product.Category.Description,
                        CreatedAt = product.Category.CreatedAt,
                        Icon = product.Category.Icon,
                    };
                }
                
                if (product.Tags != null)
                {
                    productDto.Tags = new List<TagDto>();
                    foreach (var tag in product.Tags)
                    {
                        productDto.Tags.Add(new TagDto
                        {
                            Id = tag.Id.ToString(),
                            Name = tag.Name,
                            Description = tag.Description,
                            CreatedAt = tag.CreatedAt,
                        });
                    }
                }
                
                if (reviews)
                {
                    var revs = await _context.Reviews.Where(r => r.Product.Id == product.Id).ToListAsync();
                    
                    foreach (var review in revs)
                    {
                        productDto.Reviews.Add(new ReviewDto
                        {
                            Id = review.Id.ToString(),
                            Rating = review.Rating,
                            CreatedAt = review.CreatedAt,
                            Title = review.Title,
                            Content = review.Content,
                            Username = review.Username,
                        });
                    }
                    
                    productDto.Reviews = productDto.Reviews.OrderByDescending(r => r.CreatedAt).ToList();
                }
                
                productsDto.Add(productDto);
            }
            
            return productsDto.ToList();
        } catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}

public interface IProductService
{
    Task<string> AddProduct(AddProductModel model, long? userId);
    Task<List<ProductDto>?> GetAllProducts(bool reviews, string orderBy, string order, int page, int limit);
}