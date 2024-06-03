using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

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
        if (result.status == "SUCCESS")
        {
            return Ok(JsonSerializer.Serialize(new { orderId = result.orderId }));
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
        if (result.status == "NOT_FOUND")
        {
            return NotFound();
        }
        if (result.status == "INTERNAL")
        {
            return StatusCode(500);
        }
        else
        {
            return Ok(JsonSerializer.Serialize(result.order));
        }
    }
}
