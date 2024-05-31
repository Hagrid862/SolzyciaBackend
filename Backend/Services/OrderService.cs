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
            Address = model.Address,
            Address2 = model.Address2,
            City = model.City,
            State = model.State,
            Zip = model.Zip,
            Country = model.Country,
            Phone = model.Phone,
            Email = model.Email,
            Name = model.Name,
            LastName = model.LastName,
            Status = model.Status,
            PaymentMethod = model.PaymentMethod,
            CreatedAt = DateTime.Now
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