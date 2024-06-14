namespace Backend.Dto;

public class OrderDto
{
    public required long Id { get; set; }
    public required List<OrderProductDto> Products { get; set; }
    public string? Address { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? Status { get; set; } = "Created";
    public string? PaymentMethod { get; set; }
    public required DateTime CreatedAt { get; set; } = DateTime.Now;
}
