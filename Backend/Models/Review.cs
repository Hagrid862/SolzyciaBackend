namespace Backend.Models;

public class Review
{
    public required long Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required int Rating { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required Product Product { get; set; }
}