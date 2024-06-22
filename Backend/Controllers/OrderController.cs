using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Npgsql.Replication;

namespace Backend;

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
            return Ok(JsonSerializer.Serialize(new { orderId = result.orderId.ToString() }));
        }
        else
        {
            return StatusCode(500);
        }
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder(long orderId)
    {
        var result = await _orderService.GetOrder(orderId);
        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { order = result.order }));
        }
        else
        {
            if (result.status == "NOTFOUND")
            {
                return NotFound();
            }
            else
            {
                return StatusCode(500);
            }
        }
    }

    [HttpGet("{orderId}/products")]
    public async Task<IActionResult> GetOrderProducts(long orderId)
    {
        var result = await _orderService.GetOrderProducts(orderId);
        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { products = result.products }));
        }
        else
        {
            if (result.status == "NOTFOUND")
            {
                return NotFound();
            }
            else if (result.status == "TOOMANY")
            {
                return BadRequest();
            }
            else
            {
                return StatusCode(500);
            }
        }
    }
}
