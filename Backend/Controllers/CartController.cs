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
        var cartItem = await _cartService.GetCartItem(itemId, model.Quantity, model.IsEvent);
        if (cartItem == null)
        {
            return NotFound();
        }

        return Ok(cartItem);
    }
}
