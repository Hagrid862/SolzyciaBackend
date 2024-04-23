namespace Backend.Models;

public class Admin
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string PasswordSalt { get; set; }
    public required DateTime CreatedAt { get; set; }
}