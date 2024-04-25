using System.Text.Json;
using Backend.Helpers;
using Backend.Services;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
        string status = await _adminService.Login(model.Username, model.Password);
        
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
        string response = await _adminService.Verify(model.Username, model.Password, model.Code);
        Console.WriteLine(response);

        if (response == "INTERNALERROR")
        {
            return StatusCode(500, JsonSerializer.Serialize(new { message = "Something went wrong" }));
        }
        else if (response == "INVALID")
        {
            return BadRequest(JsonSerializer.Serialize(new { message = "Invalid username, password or code" }));
        }
        else if (response == "EXPIRED")
        {
            return BadRequest(JsonSerializer.Serialize(new { message = "Code expired" }));
        }
        else if (response == "NOTFOUND")
        {
            return NotFound(JsonSerializer.Serialize(new { message = "User or 2FA request not found" }));
        }
        else
        {
            return Ok(JsonSerializer.Serialize(new { message = "Successfully logged in", token = response }));
        }
        
    }
}