using Backend.Data;
using Backend.Helpers;
using Backend.Models;

namespace Backend;

public class OrderService : IOrderService
{
    private readonly MainDbContext _context;

    public OrderService(MainDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateOrder(CreateNewOrderModel model)
    {
        var order = new Order
        {
            Id = Snowflake.GenerateId(),
            Products = model.Products.Select(x => new OrderProduct { IsEvent = x.IsEvent, Id = x.Id, Quantity = x.Quantity }).ToList(),
            CreatedAt = DateTime.UtcNow,
        };

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        return order;
    }
}

public interface IOrderService
{
    public Task<Order> CreateOrder(CreateNewOrderModel model);
}