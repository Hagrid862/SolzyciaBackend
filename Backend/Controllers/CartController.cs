﻿using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Backend;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("{itemId}")]
    public async Task<ActionResult<CartItemDto>> GetCartItem([FromRoute] long itemId, [FromQuery] GetCartItemModel model)
    {
        var result = await _cartService.GetCartItem(itemId, model.Quantity, model.IsEvent);
        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { Status = "SUCCESS", Item = result.item }));
        }
        else
        {
            if (result.status == "NOTFOUND")
            {
                return NotFound(JsonSerializer.Serialize(new { Status = "NOTFOUND", Message = "Item not found" }));
            }
            else
            {
                return StatusCode(500, new { Status = "INTERNAL", Message = "Something went wrong" });
            }
        }
    }
}
