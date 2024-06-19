namespace Backend.Models;

public class Product
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public string? Title { get; set; }
    public required string Description { get; set; }
    public List<string?>? Images { get; set; }
    public required float Price { get; set; }
    public required DateTime CreatedAt { get; set; }
    public Category? Category { get; set; }
    public List<Tag>? Tags { get; set; }
    public List<Review>? Reviews { get; set; }
}