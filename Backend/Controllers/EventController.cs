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
        Console.WriteLine(model.Dates);
        var result = await _eventService.AddEvent(model, userId);
        if (result.isSuccess)
        {
            return CreatedAtAction(nameof(GetEventById), new { eventId = result.output?.Id }, JsonSerializer.Serialize(new { Status = "SUCCESS", EventId = result.output.Id }));
        }
        else
        {
            if (result.status == "INVALIDPRICE")
            {
                return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDPRICE", Message = "Price must be greater than 0" }));
            }
            else if (result.status == "INVALIDSEATS")
            {
                return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDSEATS", Message = "Seats must be greater than 0" }));
            }
            else if (result.status == "INVALIDDATE")
            {
                return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDDATE", Message = "Date must be greater than current date" }));
            }
            else
            {
                return StatusCode(500, JsonSerializer.Serialize(new { Status = "INTERNAL", Message = "Something went wrong" }));
            }
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents([FromQuery] bool reviews = false, [FromQuery] string orderBy = "created_at", [FromQuery] string order = "desc", [FromQuery] int page = 1, [FromQuery] int limit = 25)
    {
        if (orderBy is not ("created_at" or "price" or "name" or "rating" or "popularity"))
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDORDERBY", Message = "Invalid orderBy parameter" }));
        }
        else if (order is not ("desc" or "asc"))
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDORDER", Message = "Invalid order parameter" }));
        }
        else if (page < 1)
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDPAGE", Message = "Invalid page parameter" }));
        }
        else if (limit < 1)
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDLIMIT", Message = "Invalid limit parameter" }));
        }
        else
        {
            var result = await _eventService.GetEvents(reviews, orderBy, order, page, limit);
            if (result.isSuccess)
            {
                return Ok(JsonSerializer.Serialize(new { Status = "SUCCESS", Events = result.dtos }));
            }
            else
            {
                if (result.status == "NOTFOUND")
                {
                    return NotFound(JsonSerializer.Serialize(new { Status = "NOTFOUND", Message = "No events found" }));
                }
                else
                {
                    return StatusCode(500, JsonSerializer.Serialize(new { Status = "INTERNAL", Message = "Something went wrong" }));
                }
            }
        }
    }

    [HttpGet("{eventId}")]
    public async Task<IActionResult> GetEventById(string eventId, [FromQuery] bool reviews = false)
    {
        var result = await _eventService.GetEventById(eventId, reviews);
        if (result.isSuccess)
        {
            return Ok(JsonSerializer.Serialize(new { Status = "SUCCESS", Event = result.dto }));
        }
        else
        {
            if (result.status == "NOTFOUND")
            {
                return NotFound(JsonSerializer.Serialize(new { Status = "NOTFOUND", Message = "Event not found" }));
            }
            else
            {
                return StatusCode(500, JsonSerializer.Serialize(new { Status = "INTERNAL", Message = "Something went wrong" }));
            }
        }
    }


    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetEventsByCategory(string categoryId, [FromQuery] bool reviews = false, [FromQuery] string orderBy = "created_at", [FromQuery] string order = "desc", [FromQuery] int page = 1, [FromQuery] int limit = 25)
    {
        if (orderBy is not ("created_at" or "price" or "name" or "rating" or "popularity"))
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDORDERBY", Message = "Invalid orderBy parameter" }));
        }
        else if (order is not ("desc" or "asc"))
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDORDER", Message = "Invalid order parameter" }));
        }
        else if (page < 1)
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDPAGE", Message = "Invalid page parameter" }));
        }
        else if (limit < 1)
        {
            return BadRequest(JsonSerializer.Serialize(new { Status = "INVALIDLIMIT", Message = "Invalid limit parameter" }));
        }
        else
        {
            var result = await _eventService.GetEventsByCategory(categoryId, reviews, orderBy, order, page, limit);
            if (result.isSuccess)
            {
                return Ok(JsonSerializer.Serialize(new { Status = "SUCCESS", Events = result.dtos }));
            }
            else
            {
                if (result.status == "NOTFOUND")
                {
                    return NotFound(JsonSerializer.Serialize(new { Status = "NOTFOUND", Message = "No events found" }));
                }
                else
                {
                    return StatusCode(500, JsonSerializer.Serialize(new { Status = "INTERNAL", Message = "Something went wrong" }));
                }
            }
        }
    }
}