namespace Backend;

public class CreateNewOrderModel
{
    public required List<OrderProductDto> Products { get; set; }
    public required string Address { get; set; }
    public string? Address2 { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string Zip { get; set; }
    public required string Country { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required string Status { get; set; }
    public required string PaymentMethod { get; set; }
}
