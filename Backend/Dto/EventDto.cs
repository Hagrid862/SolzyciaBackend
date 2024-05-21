namespace Backend.Dto;

public class EventDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Time { get; set; }
    public required float Price { get; set; }
    public CategoryDto? Category { get; set; }
    public List<string>? Images { get; set; }
    public List<EventDateDto>? Dates { get; set; }
    public List<EventDto> Tags { get; set; }
    public List<ReviewDto>? Reviews { get; set; }
    public DateTime CreatedAt { get; set; }
}