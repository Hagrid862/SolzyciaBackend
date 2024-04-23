namespace Backend.Models;

public class Reservation
{
    public required long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required string Status { get; set; }
    public required Product Product { get; set; }
    public required DateTime[] When { get; set; }
    public required DateTime CreatedAt { get; set; }
}