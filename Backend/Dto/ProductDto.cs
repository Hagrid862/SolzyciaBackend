using Backend.Models;

namespace Backend.Dto;

public class ProductDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Title { get; set; }
    public required string Description { get; set; }
    public List<Image>? Images { get; set; }
    public required float Price { get; set; }
    public required DateTime CreatedAt { get; set; }
    public CategoryDto? Category { get; set; }
    public List<TagDto>? Tags { get; set; }
    public List<ReviewDto>? Reviews { get; set; }
    public double? Rating { get; set; }
    public int? ReviewsCount { get; set; }
}