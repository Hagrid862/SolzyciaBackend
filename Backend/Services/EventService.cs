using System.Globalization;
using System.Text.Json;
using Backend.Data;
using Backend.Helpers;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class EventService: IEventService
{
    private readonly MainDbContext _context;
    
    public EventService(MainDbContext context)
    {
        _context = context;
    }
    
    public async Task<string> AddEvent(AddEventModel model, long? userId)
    {
        try
        {
            List<DateSeat>? datesObject = JsonSerializer.Deserialize<List<DateSeat>>(model.Dates);            List<EventDate> dates = new();
            if (datesObject == null)
            {
                return "DATESEMPTY";
            }
            
            for (int i = 0; i < datesObject.Count; i++)
            {
                Console.WriteLine(datesObject[i].seats + " , " + datesObject[i].dateIso);
                if (datesObject[i].seats < 1)
                {
                    return "SEATSINVALID";
                }
                
                if (DateTime.Parse(datesObject[i].dateIso) < DateTime.Now)
                {
                    return "DATEINVALID";
                }
                
                var date = new EventDate
                {
                    Id = Snowflake.GenerateId(),
                    Date = DateTime.Parse(datesObject[i].dateIso).ToUniversalTime(),
                    Seats = datesObject[i].seats
                };
                
                await _context.EventDates.AddAsync(date);
                dates.Add(date);
            }

            Console.WriteLine(model.Tags);
            
            List<Tag> tags = new();
            if (model.Tags != null)
            {
                List<string> tagsList = model.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();                foreach (var tagName in tagsList)
                {
                    if (!string.IsNullOrEmpty(tagName))
                    {
                        var tag = await _context.Tags.FirstOrDefaultAsync(x => x.Name == tagName);
                        if (tag == null)
                        {
                            tag = new Tag
                            {
                                Id = Snowflake.GenerateId(),
                                Name = tagName,
                                CreatedAt = DateTime.UtcNow
                            };
                            await _context.Tags.AddAsync(tag);
                        }
                    }
                }
            }

            IFormFile?[] images =
            [
                model.Image0,
                model.Image1,
                model.Image2,
                model.Image3,
                model.Image4,
                model.Image5
            ];
            
            List<string> imagesBase64 = new();
            
            foreach (var image in images)
            {
                if (image == null)
                    continue;
                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !extension.Equals(".jpg") && !extension.Equals(".png") && !extension.Equals(".jpeg") && !extension.Equals(".webp"))
                {
                    throw new Exception("Invalid image format");
                }

                string mimeType = extension switch
                {
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".webp" => "image/webp",
                    _ => throw new Exception("Invalid image format")
                };

                using var ms = new MemoryStream();
                await image.CopyToAsync(ms);
                var imageBytes = ms.ToArray();

                var base64 = $"data:{mimeType};base64,{Convert.ToBase64String(imageBytes)}";
                imagesBase64.Add(base64);
            }

            Category? category = null;

            if (model.CategoryId != null)
            {
                category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == long.Parse(model.CategoryId));
            }
            
            var @event = new Event
            {
                Id = Snowflake.GenerateId(),
                Name = model.Name,
                Description = model.Description,
                Time = model.Time,
                Price = model.Price,
                Images = imagesBase64,
                Category = category,
                Tags = tags,
                Dates = dates,
                CreatedAt = DateTime.UtcNow,
            };
            
            await _context.Events.AddAsync(@event);
            await _context.SaveChangesAsync();
            
            return "SUCCESS";
        } catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return e.Message;
        }
    }
}

public interface IEventService
{
    Task<string> AddEvent(AddEventModel model, long? userId);
}