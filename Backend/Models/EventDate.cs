namespace Backend.Models;

public class EventDate
{
    public required long Id { get; set; }
    public required DateTime Date { get; set; }
    public required int Seats { get; set; }
    public long EventId { get; set; }
    
    public Event Event { get; set; }

}