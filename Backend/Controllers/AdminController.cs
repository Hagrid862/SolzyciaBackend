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
        string status = await _adminService.Login(model.Username, model.Password, model.Remember);
        
        if (status == "SUCCESS")
        {
            return Ok(JsonSerializer.Serialize(new {message = "2FA Sent to mail"}));
        }
        else
        {
            return BadRequest(new { message = "Invalid username or password" });
        }
    }
    
    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyModel model)
    {
        var (access, refresh) = await _adminService.Verify(model.Username, model.Password, model.Code);
        if (access == "INTERNALERROR")
        {
            return StatusCode(500, JsonSerializer.Serialize(new { message = "Something went wrong" }));
        }
        else if (access == "INVALID")
        {
            return BadRequest(JsonSerializer.Serialize(new { message = "Invalid username, password or code" }));
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
}