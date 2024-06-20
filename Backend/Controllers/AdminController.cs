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
        var clientIp = HttpContext.Connection.RemoteIpAddress.ToString();
        var (isSuccess, status) = await _adminService.Login(model.Username, model.Password, model.Remember, clientIp);

        if (isSuccess)
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
        var clientIp = HttpContext.Connection.RemoteIpAddress.ToString();
        var (isSuccess, status, access, refresh) = await _adminService.VerifyOtp(model.Code, clientIp);

        if (isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { message = "Login successful", access = access, refresh = refresh }));
        }
        else
        {
            if (status == "NOTFOUND")
            {
                return NotFound(JsonSerializer.Serialize(new { message = "User or 2FA request not found" }));
            }
            else if (status == "INVALID")
            {
                return BadRequest(JsonSerializer.Serialize(new { message = "Invalid code" }));
            }
            else if (status == "EXPIRED")
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

        var (isSuccess, status, exists, valid) = await _adminService.VerifyToken(token);
        if (isSuccess)
        {
            if (exists && valid)
            {
                return Ok(JsonSerializer.Serialize(new { message = "Token is valid" }));
            }
            else if (exists && !valid)
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

        var (isSuccess, status, access, refresh) = await _adminService.RefreshToken(token);
        if (isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { message = "Token refreshed", access = access, refresh = refresh }));
        }
        else
        {
            if (status == "NOTFOUND")
            {
                return NotFound(JsonSerializer.Serialize(new { message = "Token not found" }));
            }
            else if (status == "INVALID")
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