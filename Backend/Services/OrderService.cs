﻿using System.Security.Claims;
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

    public async Task<(string status, List<ProductDto>? products, List<EventDto>? events)> GetOrderProducts(long orderId)
    {
        try
        {
            var order = await _context.Orders.Include(o => o.Products).FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
            {
                return ("NOT_FOUND", null, null);
            }

            if (order.Products.Count == 0)
            {
                return ("SUCCESS", new List<ProductDto>(), new List<EventDto>());
            }
            else if (order.Products.Count > 64)
            {
                return ("TOO_MANY", null, null);
            }

            var products = new List<ProductDto>();
            var events = new List<EventDto>();

            order.Products.ForEach(x =>
            {
                if (x.IsEvent)
                {
                    var eventData = _context.Events.Include(e => e.Dates).Include(e => e.Category).Include(e => e.Tags).FirstOrDefault(e => e.Id == x.ProductId);

                    if (eventData == null)
                    {
                        return;
                    }

                    Console.WriteLine(eventData.Name);

                    if (eventData.Dates.Count == 0)
                    {
                        return;
                    }
                    else if (eventData.Dates.Count > 64)
                    {
                        return;
                    }
                    else if (eventData.Tags?.Count > 64)
                    {
                        return;
                    }

                    Console.WriteLine(eventData.Name + " pass");

                    events.Add(new EventDto
                    {
                        Id = eventData.Id.ToString(),
                        Name = eventData.Name,
                        Description = eventData.Description,
                        Time = eventData.Time,
                        Price = eventData.Price,
                        Category = eventData.Category != null ? new CategoryDto
                        {
                            Id = eventData.Category.Id.ToString(),
                            Name = eventData.Category.Name,
                            Icon = eventData.Category.Icon,
                            Description = eventData.Category.Description,
                            CreatedAt = eventData.Category.CreatedAt,
                        } : null,
                        Images = eventData.Images,
                        Dates = eventData.Dates.Select(d => new EventDateDto
                        {
                            Id = d.Id.ToString(),
                            Date = d.Date,
                            Seats = d.Seats,
                        }).ToList(),
                        Tags = eventData.Tags?.Select(t => new TagDto
                        {
                            Id = t.Id.ToString(),
                            Name = t.Name,
                            Description = t.Description,
                            CreatedAt = t.CreatedAt,
                        }).ToList(),
                        CreatedAt = eventData.CreatedAt,
                    });
                }
                else
                {
                    var productData = _context.Products.Include(p => p.Category).Include(p => p.Tags).FirstOrDefault(p => p.Id == x.ProductId);

                    if (productData == null)
                    {
                        return;
                    }

                    products.Add(new ProductDto
                    {
                        Id = productData.Id.ToString(),
                        Name = productData.Name,
                        Description = productData.Description,
                        Price = productData.Price,
                        Category = productData.Category != null ? new CategoryDto
                        {
                            Id = productData.Category.Id.ToString(),
                            Name = productData.Category.Name,
                            Icon = productData.Category.Icon,
                            Description = productData.Category.Description,
                            CreatedAt = productData.Category.CreatedAt,
                        } : null,
                        Images = productData.Images,
                        Tags = productData.Tags?.Select(t => new TagDto
                        {
                            Id = t.Id.ToString(),
                            Name = t.Name,
                            Description = t.Description,
                            CreatedAt = t.CreatedAt,
                        }).ToList(),
                        CreatedAt = productData.CreatedAt,
                    });
                }
            });

            return ("SUCCESS", products, events);
        }
        catch
        {
            return ("INTERNAL", null, null);
        }
    }
}

public interface IOrderService
{
    public Task<(string status, long? orderId)> CreateOrder(CreateNewOrderModel model);
    public Task<(string status, OrderDto? order)> GetOrder(long orderId);
    public Task<(string status, List<ProductDto> products, List<EventDto> events)> GetOrderProducts(long orderId);
}