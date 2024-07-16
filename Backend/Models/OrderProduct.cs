namespace Backend.Models;


public class OrderProduct
{
    public required long Id { get; set; }
    public required long ProductId { get; set; }
    public required int Quantity { get; set; }
    public required bool IsEvent { get; set; }
}