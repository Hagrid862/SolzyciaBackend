using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.Dto;
using Backend.Helpers;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend;

public class OrderService : IOrderService
{
    private readonly MainDbContext _context;

    public OrderService(MainDbContext context)
    {
        _context = context;
    }

    public async Task<(string status, long? orderId)> CreateOrder(CreateNewOrderModel model)
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

            return ("SUCCESS", order.Id);
        }
        catch
        {
            return ("INTERNAL", null);
        }
    }

    public async Task<(string status, OrderDto? order)> GetOrder(long orderId)
    {
        try
        {
            var order = await _context.Orders.Include(o => o.Products).FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
            {
                return ("NOT_FOUND", null);
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                Products = order.Products.Select(x => new OrderProductDto { Id = x.Id.ToString(), IsEvent = x.IsEvent, Quantity = x.Quantity }).ToList(),
                Address = order.Address,
                Address2 = order.Address2,
                City = order.City,
                State = order.State,
                Zip = order.Zip,
                Country = order.Country,
                Phone = order.Phone,
                Email = order.Email,
                Name = order.Name,
                LastName = order.LastName,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                CreatedAt = order.CreatedAt,
            };

            return ("SUCCESS", orderDto);
        }
        catch
        {
            return ("INTERNAL", null);
        }
    }
}

public interface IOrderService
{
    public Task<(string status, long? orderId)> CreateOrder(CreateNewOrderModel model);
    public Task<(string status, OrderDto? order)> GetOrder(long orderId);
}