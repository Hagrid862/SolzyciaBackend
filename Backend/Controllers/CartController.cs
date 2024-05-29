using Microsoft.AspNetCore.Mvc;

namespace Backend;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }
  
    [HttpGet]
    public async Task<ActionResult<CartItemDto>> GetCartItem([FromBody] GetCartItemModel model)
    {
        var cartItem = await _cartService.GetCartItem(model.ItemId, model.Quantity, model.IsEvent);
        if (cartItem == null)
        {
            return NotFound();
        }

        return Ok(cartItem);
    }
}
