namespace Backend.Models;

public class TwoFactorAuth
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required Admin Admin { get; set; }
    public required bool IsUsed { get; set; } = false;
    public required DateTime CreatedAt { get; set; }
}