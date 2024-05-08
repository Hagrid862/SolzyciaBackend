namespace Backend.Dto;

public class CategoryDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Icon { get; set; }
    public required string Description { get; set; }
    public required DateTime CreatedAt { get; set; }
}