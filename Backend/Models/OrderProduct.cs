namespace Backend;


public class OrderProduct
{
    public required string Id { get; set; }
    public required int Quantity { get; set; }
    public required bool IsEvent { get; set; }
}