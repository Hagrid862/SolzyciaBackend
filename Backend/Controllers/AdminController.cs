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
            return BadRequest(JsonSerializer.Serialize(new { status = "NOIP", message = "Client IP not found" }));
        }

        var result = await _adminService.Login(model.Username, model.Password, model.Remember, clientIp);

        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { Status = "2FASENT", Message = "2FA Sent to mail" }));
        }
        else
        {
            if (result.status == "INVALID")
            {
                return BadRequest(JsonSerializer.Serialize(new { Status = "INVALID", Message = "Invalid username or password" }));
            }
            else
            {
                return StatusCode(500, JsonSerializer.Serialize(new { Status = "ERROR", Message = "Something went wrong" }));
            }
        }
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> Verify([FromBody] VerifyModel model)
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (clientIp == null)
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "NOIP", Message = "Client IP not found" }));
        }

        var result = await _adminService.VerifyOtp(model.Code, clientIp);

        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { Status = "LOGGEDIN", Message = "Login successful", Access = result.access, Refresh = result.refresh }));
        }
        else
        {
            if (result.status == "NOTFOUND")
            {
                return NotFound(JsonSerializer.Serialize(new { Status = "NOTFOUND", Message = "User or 2FA request not found" }));
            }
            else if (result.status == "INVALID")
            {
                return BadRequest(JsonSerializer.Serialize(new { Status = "INVALID", Message = "Invalid code" }));
            }
            else if (result.status == "EXPIRED")
            {
                return BadRequest(JsonSerializer.Serialize(new { Status = "EXPIRED", Message = "Code expired" }));
            }
            else
            {
                return StatusCode(500, JsonSerializer.Serialize(new { Status = "INTERNAL", Message = "Something went wrong" }));
            }
        }
    }

    [HttpPost("token/verify")]
    public async Task<IActionResult> VerifyToken()
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        if (token.IsNullOrEmpty())
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "NOTOKEN", Message = "Token not found" }));
        }

        var result = await _adminService.VerifyToken(token);
        if (result.isSuccess)
        {
            if (result.exists && result.valid)
            {
                return Ok(JsonSerializer.Serialize(new { Status = "VALID", Message = "Token is valid" }));
            }
            else if (result.exists && !result.valid)
            {
                return BadRequest(JsonSerializer.Serialize(new { Status = "INVALID", Message = "Token is invalid" }));
            }
            else
            {
                return NotFound(JsonSerializer.Serialize(new { Status = "NOTFOUND", Message = "Token not found" }));
            }
        }
        else
        {
            return StatusCode(500, JsonSerializer.Serialize(new { Status = "INTERNAL", Message = "Something went wrong" }));
        }
    }

    [HttpPost("token/refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        if (token.IsNullOrEmpty())
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "NOTOKEN", Message = "Token not found" }));
        }

        var result = await _adminService.RefreshToken(token);
        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { Status = "SUCCESS", Message = "Token refreshed", Access = result.access, Refresh = result.refresh }));
        }
        else
        {
            if (result.status == "NOTFOUND")
            {
                return NotFound(JsonSerializer.Serialize(new { Status = "NOTFOUND", Message = "Token not found" }));
            }
            else if (result.status == "INVALID")
            {
                return BadRequest(JsonSerializer.Serialize(new { Status = "INVALID", Message = "Token is invalid" }));
            }
            else
            {
                return StatusCode(500, JsonSerializer.Serialize(new { Status = "INTERNAL", Message = "Something went wrong" }));
            }
        }
    }
}