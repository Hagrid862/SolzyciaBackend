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
        return Ok(JsonSerializer.Serialize(new { orderId = result.Id }));
    }
}
