using Backend.Data;
using Backend.Helpers;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend;

public class OrderService : IOrderService
{
    private readonly MainDbContext _context;

    public OrderService(MainDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> CreateOrder(CreateNewOrderModel model)
    {
        try
        {
            var order = new Order
            {
                Id = Snowflake.GenerateId(),
                Products = model.Products.Select(x => new OrderProduct { Id = Snowflake.GenerateId(), ProductId = long.Parse(x.Id), IsEvent = x.IsEvent, Quantity = x.Quantity }).ToList(),
                CreatedAt = DateTime.UtcNow,
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return order;
        } catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}

public interface IOrderService
{
    public Task<Order?> CreateOrder(CreateNewOrderModel model);
}