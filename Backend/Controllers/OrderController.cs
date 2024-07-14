using Backend.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Npgsql.Replication;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateNewOrderModel model)
    {
        var result = await _orderService.CreateOrder(model);
        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { Status = "SUCCESS", Message = "Order created successfully", OrderId = result.orderId.ToString() }));
        }
        else
        {
            return StatusCode(500, JsonSerializer.Serialize(new { Status = "INTERNAL", Message = "Something went wrong" }));
        }
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder(long orderId)
    {
        var result = await _orderService.GetOrder(orderId);
        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { Status = "SUCCESS", Order = result.order }));
        }
        else
        {
            if (result.status == "NOTFOUND")
            {
                return NotFound(JsonSerializer.Serialize(new { Status = "NOTFOUND", Message = "Order not found" }));
            }
            else
            {
                return StatusCode(500, JsonSerializer.Serialize(new { Status = "INTERNAL", Message = "Something went wrong" }));
            }
        }
    }

    [HttpGet("{orderId}/products")]
    public async Task<IActionResult> GetOrderProducts(long orderId)
    {
        var result = await _orderService.GetOrderProducts(orderId);
        Console.WriteLine(result.products.Count);
        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { Status = "SUCCESS", Products = result.products, Events = result.events }));
        }
        else
        {
            if (result.status == "NOTFOUND")
            {
                return NotFound(JsonSerializer.Serialize(new { Status = "NOTFOUND", Message = "Order not found" }));
            }
            else if (result.status == "TOOMANY")
            {
                return BadRequest(JsonSerializer.Serialize(new { Status = "TOOMANY", Message = "Too many products in order" }));
            }
            else
            {
                return StatusCode(500, JsonSerializer.Serialize(new { Status = "INTERNAL", Message = "Something went wrong" }));
            }
        }
    }
}
