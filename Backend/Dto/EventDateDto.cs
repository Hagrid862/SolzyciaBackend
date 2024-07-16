namespace Backend.Dto;

public class EventDateDto
{
    public required string Id { get; set; }
    public required DateTime Date { get; set; }
    public required int Seats { get; set; }
    public required EventLocationDto? Location { get; set; }
}