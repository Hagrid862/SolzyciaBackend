namespace Backend.Dto;

public class ReviewDto
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required int Rating { get; set; }
    public required string Username { get; set; }
}