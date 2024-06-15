using System.Text.Json;
using Backend.Dto;
using Backend.Middlewares;
using Backend.Services;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class EventController : ControllerBase
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
        long? userId = (long?)HttpContext.Items["UserId"];
        var result = await _eventService.AddEvent(model, userId);
        if (result == "success")
        {
            return Ok(new { message = "event created successfully" });
        }
        else
        {
            if (result == "DATESEMPTY")
            {
                return BadRequest(new { message = "dates array is empty" });
            }
            else if (result == "SEATSINVALID")
            {
                return BadRequest(new { message = "seats must be greater than 0" });
            }
            else if (result == "DATEINVALID")
            {
                return BadRequest(new { message = "date must be greater than current date" });
            }
            else
            {
                return StatusCode(500, new { message = result });
            }
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents([FromQuery] bool reviews = false, [FromQuery] string orderBy = "created_at", [FromQuery] string order = "desc", [FromQuery] int page = 1, [FromQuery] int limit = 25)
    {
        if (orderBy is not ("created_at" or "price" or "name" or "rating" or "popularity"))
        {
            return BadRequest(new { message = "Invalid orderBy parameter" });
        }
        else if (order is not ("desc" or "asc"))
        {
            return BadRequest(new { message = "Invalid order parameter" });
        }
        else if (page < 1)
        {
            return BadRequest(new { message = "Invalid page parameter" });
        }
        else if (limit < 1)
        {
            return BadRequest(new { message = "Invalid limit parameter" });
        }
        else
        {
            List<EventDto> events = await _eventService.GetEvents(reviews, orderBy, order, page, limit);
            if (events == null)
            {
                return NotFound(new { message = "No events found" });
            }
            return Ok(JsonSerializer.Serialize(new { events = events }));
        }
    }

    [HttpGet("{eventId}")]
    public async Task<IActionResult> GetEventById(string eventId, [FromQuery] bool reviews = false)
    {
        EventDto eventDto = await _eventService.GetEventById(eventId, reviews);
        if (eventDto == null)
        {
            return NotFound(new { message = "Event not found" });
        }

        return Ok(JsonSerializer.Serialize(new { eventDto = eventDto }));
    }


    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetEventsByCategory(string categoryId, [FromQuery] bool reviews = false, [FromQuery] string orderBy = "created_at", [FromQuery] string order = "desc", [FromQuery] int page = 1, [FromQuery] int limit = 25)
    {
        if (orderBy is not ("created_at" or "price" or "name" or "rating" or "popularity"))
        {
            return BadRequest(new { message = "Invalid orderBy parameter" });
        }
        else if (order is not ("desc" or "asc"))
        {
            return BadRequest(new { message = "Invalid order parameter" });
        }
        else if (page < 1)
        {
            return BadRequest(new { message = "Invalid page parameter" });
        }
        else if (limit < 1)
        {
            return BadRequest(new { message = "Invalid limit parameter" });
        }
        else
        {
            List<EventDto> events = await _eventService.GetEventsByCategory(categoryId, reviews, orderBy, order, page, limit);
            if (events == null)
            {
                return NotFound(new { message = "No events found" });
            }
            return Ok(JsonSerializer.Serialize(new { events = events }));
        }
    }

    // [HttpPatch("{eventId}")]
    // [AuthenticateAdminTokenMiddleware]
    // public async Task<IActionResult> UpdateEvent(string eventId, UpdateEventModel model)
    // {
    //     var result = await _eventService.UpdateEvent(eventId, model);
    //     if (result == "success")
    //     {
    //         return Ok(new { message = "event updated successfully" });
    //     }
    //     else
    //     {
    //         return StatusCode(500, new { message = result });
    //     }
    // }
}