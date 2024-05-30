using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend;

public class CartService : ICartService
{
    private readonly MainDbContext _context;

    public CartService(MainDbContext context)
    {
        _context = context;
    }

    public async Task<CartItemDto?> GetCartItem(long productId, int quantity, bool isEvent)
    {
        if (isEvent)
        {
            var product = await _context.Events.FirstOrDefaultAsync(x => x.Id == productId);
            Console.WriteLine(product == null ? "is null" : "is not null");

            if (product == null)
            {
                return null;
            }

            var dto = new CartItemDto
            {
                ItemId = product.Id.ToString(),
                IsEvent = true,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity,
                Image = product.Images != null && product.Images.Any() ? product.Images[0] : "noimage",
                IsOnSale = false,
            };

            return dto;
        }
        else
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);
            Console.WriteLine(product == null ? "is null" : "is not null");

            if (product == null)
            {
                return null;
            }

            var dto = new CartItemDto
            {
                ItemId = product.Id.ToString(),
                IsEvent = false,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity,
                Image = product.Images != null && product.Images.Any() ? product.Images[0] : "noimage",
                IsOnSale = false,
            };

            return dto;
        }
    }
}

public interface ICartService
{
    Task<CartItemDto?> GetCartItem(long productId, int quantity, bool isEvent);
}