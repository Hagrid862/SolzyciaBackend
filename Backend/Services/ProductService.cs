using Backend.Data;
using Backend.Dto;
using Backend.Helpers;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class ProductService : IProductService
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
            Console.WriteLine("Tags: " + model.Tags);
            if (userId == null)
                return "ERROR";

            Category? category = null;

            Console.WriteLine("Category ID: " + model.CategoryId);

            if (model.CategoryId != null)
            {
                category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == long.Parse(model.CategoryId));

                if (category == null)
                    return "NOTFOUND";
            }

            List<long> tagsId = new List<long>();

            if (model.Tags != null)
            {
                var tagsList = model.Tags.Split(",").ToList();

                Console.WriteLine("Tags List: " + tagsList.Count());

                foreach (var tag in tagsList)
                {
                    Console.WriteLine("Tag: " + tag);
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

            var imagesList = new List<Image>();

            foreach (var image in imagesBase64)
            {
                imagesList.Add(new Image
                {
                    Id = Snowflake.GenerateId(),
                    Base64 = image
                });
            }

            var newProduct = new Product
            {
                Id = Snowflake.GenerateId(),
                Name = model.Name,
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                Images = imagesList,
                CreatedAt = DateTime.UtcNow,
                Category = category,
                Tags = await _context.Tags.Where(t => tagsId.Contains(t.Id)).ToListAsync(),
            };

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            return "SUCCESS";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return "ERROR";
        }
    }

    public async Task<List<ProductDto>?> GetAllProducts(bool reviews, string orderBy, string order, int page, int limit)
    {
        try
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Include(p => p.Images)
                .ToListAsync();
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
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<List<ProductDto>?> GetProductsByCategory(string category, bool reviews, string orderBy, string order, int page, int limit)
    {
        try
        {
            var categoryId = long.Parse(category);
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Include(p => p.Images)
                .Where(p => p.Category.Id == categoryId)
                .ToListAsync();
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
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<ProductDto?> GetProductById(string productId)
    {
        try
        {
            var product = await _context.Products
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == long.Parse(productId));

            if (product == null)
            {
                return null;
            }

            Category? category = product.Category;
            List<Tag> tags = product.Tags ?? new();

            CategoryDto? categoryDto = null;

            categoryDto = category != null ? new CategoryDto
            {
                Id = category.Id.ToString(),
                Name = category.Name,
                Icon = category.Icon,
                Description = category.Description,
                CreatedAt = category.CreatedAt
            } : null;

            List<TagDto> tagsDto = new List<TagDto>();

            if (tags.Count > 0)
            {
                foreach (var tag in tags)
                {
                    var tagDto = new TagDto
                    {
                        Id = tag.Id.ToString(),
                        Name = tag.Name,
                        Description = tag.Description,
                        CreatedAt = tag.CreatedAt
                    };
                    tagsDto.Add(tagDto);
                }
            }

            var productDto = new ProductDto
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                Images = product.Images,
                CreatedAt = product.CreatedAt,
                Category = categoryDto ?? null,
                Tags = tagsDto.Count > 0 ? tagsDto : null
            };

            Console.WriteLine("tags: " + productDto.Tags?.Count);

            return productDto;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<(string status, ProductDto? dto)> UpdateProduct(string productId, UpdateProductModel model)
    {
        async static Task<string> makeBase64(IFormFile image)
        {
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

            return $"data:{mimeType};base64,{Convert.ToBase64String(imageBytes)}";
        }

        try
        {
            long id = long.Parse(productId);
            var product = await _context.Products
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return ("NOTFOUND", null);
            }

            if (model.Name != null)
            {
                product.Name = model.Name;
            }

            if (model.Title != null)
            {
                product.Title = model.Title;
            }

            if (model.Description != null)
            {
                product.Description = model.Description;
            }

            if (model.Image0 != null)
            {
                if (product.Images != null)
                {
                    product.Images.Add(new Image()
                    {
                        Id = Snowflake.GenerateId(),
                        Base64 = await makeBase64(model.Image0)
                    });
                }
                else
                {
                    product.Images = new List<Image> { new Image()
                    {
                        Id = Snowflake.GenerateId(),
                        Base64 = await makeBase64(model.Image0)
                    }};
                }
            }

            if (model.Image1 != null)
            {
                if (product.Images != null)
                {
                    product.Images.Add(new Image()
                    {
                        Id = Snowflake.GenerateId(),
                        Base64 = await makeBase64(model.Image1)
                    });
                }
                else
                {
                    product.Images = new List<Image> { new Image()
                    {
                        Id = Snowflake.GenerateId(),
                        Base64 = await makeBase64(model.Image1)
                    }};
                }
            }

            if (model.Image2 != null)
            {
                if (product.Images != null)
                {
                    product.Images.Add(new Image()
                    {
                        Id = Snowflake.GenerateId(),
                        Base64 = await makeBase64(model.Image2)
                    });
                }
                else
                {
                    product.Images = new List<Image> { new Image()
                    {
                        Id = Snowflake.GenerateId(),
                        Base64 = await makeBase64(model.Image2)
                    }};
                }
            }

            if (model.Image3 != null)
            {
                if (product.Images != null)
                {
                    product.Images.Add(new Image()
                    {
                        Id = Snowflake.GenerateId(),
                        Base64 = await makeBase64(model.Image3)
                    });
                }
                else
                {
                    product.Images = new List<Image> { new Image()
                    {
                        Id = Snowflake.GenerateId(),
                        Base64 = await makeBase64(model.Image3)
                    }};
                }
            }

            if (model.Image4 != null)
            {
                if (product.Images != null)
                {
                    product.Images.Add(new Image()
                    {
                        Id = Snowflake.GenerateId(),
                        Base64 = await makeBase64(model.Image4)
                    });
                }
                else
                {
                    product.Images = new List<Image> { new Image()
                    {
                        Id = Snowflake.GenerateId(),
                        Base64 = await makeBase64(model.Image4)
                    }};
                }
            }

            if (model.Image5 != null)
            {
                if (product.Images != null)
                {
                    product.Images.Add(new Image()
                    {
                        Id = Snowflake.GenerateId(),
                        Base64 = await makeBase64(model.Image5)
                    });
                }
                else
                {
                    product.Images = new List<Image> { new Image()
                    {
                        Id = Snowflake.GenerateId(),
                        Base64 = await makeBase64(model.Image5)
                    }};
                }
            }

            if (model.Price != null)
            {
                product.Price = (float)model.Price;
            }

            if (model.CategoryId != null)
            {
                var categoryId = long.Parse(model.CategoryId);
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
                if (category == null)
                {
                    return ("CATEGORYNOTFOUND", null);
                }
                product.Category = category;
            }

            if (model.Tags != null)
            {
                List<long> tagsId = [];
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
                product.Tags = await _context.Tags.Where(t => tagsId.Contains(t.Id)).ToListAsync();
            }

            await _context.SaveChangesAsync();

            return ("SUCCESS", new ProductDto
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                Images = product.Images,
                CreatedAt = product.CreatedAt,
                Category = product.Category != null ? new CategoryDto
                {
                    Id = product.Category.Id.ToString(),
                    Name = product.Category.Name,
                    Description = product.Category.Description,
                    CreatedAt = product.Category.CreatedAt,
                    Icon = product.Category.Icon,
                } : null,
                Tags = product.Tags != null ? product.Tags.Select(t => new TagDto
                {
                    Id = t.Id.ToString(),
                    Name = t.Name,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt,
                }).ToList() : null
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ("ERROR", null);
        }
    }
}

public interface IProductService
{
    Task<string> AddProduct(AddProductModel model, long? userId);
    Task<List<ProductDto>?> GetAllProducts(bool reviews, string orderBy, string order, int page, int limit);
    Task<List<ProductDto>?> GetProductsByCategory(string category, bool reviews, string orderBy, string order, int page, int limit);
    Task<ProductDto?> GetProductById(string productId);
    Task<(string status, ProductDto? dto)> UpdateProduct(string productId, UpdateProductModel model);
}