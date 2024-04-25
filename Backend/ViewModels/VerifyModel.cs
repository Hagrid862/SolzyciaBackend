namespace Backend.ViewModels;

public class VerifyModel
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Code { get; set; }
}