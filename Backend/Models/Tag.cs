namespace Backend.Models;

public class Tag
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required DateTime CreatedAt { get; set; }
}