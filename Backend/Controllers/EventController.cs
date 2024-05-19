using Backend.Middlewares;
using Backend.Services;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class EventController: ControllerBase
{
    private readonly IEventService _eventService;
    
    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }
    
    [HttpPost]
    [AuthenticateAdminTokenMiddleware]
    public async Task<IActionResult> AddEvent(AddEventModel model)
    {
        long? userId = (long?) HttpContext.Items["UserId"];
        var result = await _eventService.AddEvent(model, userId);
        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! " + result);
        Console.WriteLine(model.Dates);
        if (result == "success")
        {
            return Ok(new {message = "event created successfully"});
        }
        else
        {
            if (result == "DATESEMPTY")
            {
                return BadRequest(new {message = "dates array is empty"});
            }
            else if (result == "SEATSINVALID")
            {
                return BadRequest(new {message = "seats must be greater than 0"});
            }
            else if (result == "DATEINVALID")
            {
                return BadRequest(new {message = "date must be greater than current date"});
            }
            else
            {
                return StatusCode(500, new {message = result});
            }
        }
    }
}