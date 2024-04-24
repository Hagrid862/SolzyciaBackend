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
        
        Console.WriteLine(status);
        
        if (status == "SUCCESS")
        {
            return Ok(JsonSerializer.Serialize(new {message = "2FA Sent to mail"}));
        }
        else
        {
            return BadRequest(new { message = "Invalid username or password" });
        }
    }
    
    // [HttpPost("verify")]
    // public async Task<IActionResult> Verify([FromBody] VerifyModel model)
    // {
    //     string status = await _adminService.Verify(model.Username, model.Password, model.Code);
    //     
    //     if (status == "SUCCESS")
    //     {
    //         return Ok(JsonSerializer.Serialize(new {message = "Logged in"}));
    //     }
    //     else
    //     {
    //         return BadRequest(new { message = "Invalid code" });
    //     }
    // }
}