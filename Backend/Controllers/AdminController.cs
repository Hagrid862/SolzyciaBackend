using System.Text.Json;
using Backend.Helpers;
using Backend.Services;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (clientIp == null)
        {
            return BadRequest(JsonSerializer.Serialize(new { message = "Client IP not found" }));
        }
        
        var result = await _adminService.Login(model.Username, model.Password, model.Remember, clientIp);

        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { message = "2FA Sent to mail" }));
        }
        else
        {
            return BadRequest(new { message = "Invalid username or password" });
        }
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> Verify([FromBody] VerifyModel model)
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (clientIp == null)
        {
            return BadRequest(JsonSerializer.Serialize(new { message = "Client IP not found" }));
        }
        
        var result = await _adminService.VerifyOtp(model.Code, clientIp);

        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { message = "Login successful", access = result.access, refresh = result.refresh }));
        }
        else
        {
            if (result.status == "NOTFOUND")
            {
                return NotFound(JsonSerializer.Serialize(new { message = "User or 2FA request not found" }));
            }
            else if (result.status == "INVALID")
            {
                return BadRequest(JsonSerializer.Serialize(new { message = "Invalid code" }));
            }
            else if (result.status == "EXPIRED")
            {
                return BadRequest(JsonSerializer.Serialize(new { message = "Code expired" }));
            }
            else
            {
                return StatusCode(500, JsonSerializer.Serialize(new { message = "Something went wrong" }));
            }
        }
    }

    [HttpPost("token/verify")]
    public async Task<IActionResult> VerifyToken()
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        if (token.IsNullOrEmpty())
        {
            return BadRequest(JsonSerializer.Serialize(new { message = "Token not found" }));
        }

        var result = await _adminService.VerifyToken(token);
        if (result.isSuccess)
        {
            if (result.exists && result.valid)
            {
                return Ok(JsonSerializer.Serialize(new { message = "Token is valid" }));
            }
            else if (result.exists && !result.valid)
            {
                return BadRequest(JsonSerializer.Serialize(new { message = "Token is invalid" }));
            }
            else
            {
                return NotFound(JsonSerializer.Serialize(new { message = "Token not found" }));
            }
        }
        else
        {
            return StatusCode(500, JsonSerializer.Serialize(new { message = "Something went wrong" }));
        }
    }

    [HttpPost("token/refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        if (token.IsNullOrEmpty())
        {
            return BadRequest(JsonSerializer.Serialize(new { message = "Token not found" }));
        }

        var result = await _adminService.RefreshToken(token);
        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { message = "Token refreshed", access = result.access, refresh = result.refresh }));
        }
        else
        {
            if (result.status == "NOTFOUND")
            {
                return NotFound(JsonSerializer.Serialize(new { message = "Token not found" }));
            }
            else if (result.status == "INVALID")
            {
                return BadRequest(JsonSerializer.Serialize(new { message = "Token is invalid" }));
            }
            else
            {
                return StatusCode(500, JsonSerializer.Serialize(new { message = "Something went wrong" }));
            }
        }
    }
}