namespace Backend;

public class OrderProductDto
{
    public required string Id { get; set; }
    public required int Quantity { get; set; }
    public required bool IsEvent { get; set; }
}
