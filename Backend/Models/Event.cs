namespace Backend.Models;

public class Event
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Time { get; set; }
    public required float Price { get; set; }
    public Category? Category { get; set; }
    public List<string>? Images { get; set; }
    public List<EventDate> Dates { get; set; }
    public List<Tag>? Tags { get; set; }
    public List<Review>? Reviews { get; set; }
    public DateTime CreatedAt { get; set; }
}