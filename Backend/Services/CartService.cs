using Backend.Data;

namespace Backend;

public class CartService
{
    private readonly MainDbContext _context;

    public CartService(MainDbContext context)
    {
        _context = context;
    }

    public async Task<CartItemDto> GetCartItem(long productId, int quantity,  bool isEvent)
    {
        var product = await _context.Products.FindAsync(productId);
        Console.WriteLine(product == null ? "is null" : "is not null");

        if (product == null)
        {
            return null;
        }

        var dto = new CartItemDto {
          ItemId = product.Id,
          IsEvent = false,
          Name = product.Name,
          Price = product.Price,
          Quantity = quantity,
          Image = product.Images?[0]??"noimage",
          IsOnSale = false,
        };

        return dto;
    }
}
