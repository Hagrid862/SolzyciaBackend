namespace Backend.Dto;

public class TagDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required DateTime CreatedAt { get; set; }
}