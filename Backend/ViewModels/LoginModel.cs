namespace Backend.ViewModels;

public class LoginModel
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required bool Remember { get; set; }
}