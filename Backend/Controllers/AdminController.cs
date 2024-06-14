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
        string status = await _adminService.Login(model.Username, model.Password, model.Remember, clientIp);

        if (status == "SUCCESS")
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
        var (access, refresh) = await _adminService.VerifyOtp(model.Code, clientIp);
        if (access == "INTERNALERROR")
        {
            return StatusCode(500, JsonSerializer.Serialize(new { message = "Something went wrong" }));
        }
        else if (access == "INVALID")
        {
            return BadRequest(JsonSerializer.Serialize(new { message = "Invalid code" }));
        }
        else if (access == "EXPIRED")
        {
            return BadRequest(JsonSerializer.Serialize(new { message = "Code expired" }));
        }
        else if (access == "NOTFOUND")
        {
            return NotFound(JsonSerializer.Serialize(new { message = "User or 2FA request not found" }));
        }
        else
        {
            if (refresh.IsNullOrEmpty())
            {
                return Ok(JsonSerializer.Serialize(new { access = access }));
            }
            else
            {
                return Ok(JsonSerializer.Serialize(new { access = access, refresh = refresh }));
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

        var (exists, valid) = await _adminService.VerifyToken(token);
        if (!exists)
        {
            return BadRequest(JsonSerializer.Serialize(new { message = "Invalid token", isValid = false }));
        }
        else if (exists && !valid)
        {
            return Ok(JsonSerializer.Serialize(new { message = "Token expired", isValid = false }));
        }
        else if (exists && valid)
        {
            return Ok(JsonSerializer.Serialize(new { message = "Token valid", isValid = true }));
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

        var (access, refresh) = await _adminService.RefreshToken(token);
        if (access == "INTERNALERROR")
        {
            return StatusCode(500, JsonSerializer.Serialize(new { message = "Something went wrong" }));
        }
        else if (access == "INVALID")
        {
            return BadRequest(JsonSerializer.Serialize(new { message = "Invalid token" }));
        }
        else if (access == "EXPIRED")
        {
            return BadRequest(JsonSerializer.Serialize(new { message = "Token expired" }));
        }
        else
        {
            return Ok(JsonSerializer.Serialize(new { access = access, refresh = refresh }));
        }
    }
}