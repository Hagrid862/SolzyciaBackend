using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Backend;

public class CartService : ICartService
{
    private readonly MainDbContext _context;

    public CartService(MainDbContext context)
    {
        _context = context;
    }

    public async Task<(bool isSuccess, string status, CartItemDto? item)> GetCartItem(long productId, int quantity, bool isEvent)
    {
        try
        {
            if (isEvent)
            {
                var product = await _context.Events
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(x => x.Id == productId);
                Console.WriteLine(product == null ? "is null" : "is not null");

                if (product == null)
                {
                    return (false, "NOTFOUND", null);
                }

                var dto = new CartItemDto
                {
                    ItemId = product.Id.ToString(),
                    IsEvent = true,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    Image = product.Images != null && product.Images.Any() ? product.Images[0] : null,
                    IsOnSale = false,
                };

                return (true, "OK", dto);
            }
            else
            {
                var product = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(x => x.Id == productId);
                Console.WriteLine(product == null ? "is null" : "is not null");

                if (product == null)
                {
                    return (false, "NOTFOUND", null);
                }

                var dto = new CartItemDto
                {
                    ItemId = product.Id.ToString(),
                    IsEvent = false,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    Image = product.Images != null && product.Images.Any() ? product.Images[0] : null,
                    IsOnSale = false,
                };

                Console.WriteLine(dto.Price);

                return (true, "OK", dto);
            }
        }
        catch
        {
            return (false, "ERROR", null);
        }
    }
}

public interface ICartService
{
    Task<(bool isSuccess, string status, CartItemDto? item)> GetCartItem(long productId, int quantity, bool isEvent);
}