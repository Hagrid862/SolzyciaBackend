namespace Backend.Models;

public class Product
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required List<string> Images { get; set; }
    public required decimal Price { get; set; }
    public required decimal Discount { get; set; }
    public required DateTime[] When { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required Category Category { get; set; }
    public List<Tag> Tags { get; set; }
    public List<Review> Reviews { get; set; }
}