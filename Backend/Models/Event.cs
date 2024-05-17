namespace Backend.Models;

public class Event
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Time { get; set; }
    public List<(DateTime date, int seats)> Dates { get; set; }
    public required float Price { get; set; }
    public required Category Category { get; set; }
    public List<Tag>? Tags { get; set; }
    public List<Review> Reviews { get; set; }
}