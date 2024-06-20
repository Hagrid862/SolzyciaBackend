using System.Globalization;
using System.Text.Json;
using Backend.Data;
using Backend.Dto;
using Backend.Helpers;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class EventService : IEventService
{
    private readonly MainDbContext _context;
    public EventService(MainDbContext context)
    {
        _context = context;
    }
    public async Task<List<EventDto>?> GetEvents(bool reviews, string orderBy, string order, int page, int limit)
    {
        try
        {
            var events = await _context.Events
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .Include(x => x.Dates)
                .Include(x => x.Images)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            foreach (var ev in events)
            {
                Console.WriteLine(ev.Dates.Count + " " + ev.Dates[0].Date);
            }

            if (events.Count == 0)
            {
                return null;
            }

            List<EventDto> eventsDto = new();

            foreach (var @event in events)
            {
                Category? category = @event.Category;
                EventDate[] dates = @event.Dates.ToArray();
                List<Tag> tags = @event.Tags ?? new();
                List<Review> reviewsList = @event.Reviews ?? new();

                CategoryDto? categoryDto = null;

                if (category != null)
                {
                    categoryDto = new CategoryDto
                    {
                        Id = category.Id.ToString(),
                        Name = category.Name,
                        Icon = category.Icon,
                        Description = category.Description,
                        CreatedAt = category.CreatedAt
                    };
                }

                List<EventDateDto> datesDto = new List<EventDateDto>();

                foreach (var date in dates)
                {
                    var dateDto = new EventDateDto
                    {
                        Id = date.Id.ToString(),
                        Date = date.Date,
                        Seats = date.Seats
                    };
                    datesDto.Add(dateDto);
                }

                List<TagDto>? tagsDto = null;

                foreach (var tag in tags)
                {
                    var tagDto = new TagDto
                    {
                        Id = tag.Id.ToString(),
                        Name = tag.Name,
                        Description = tag.Description,
                        CreatedAt = tag.CreatedAt
                    };
                    tagsDto.Add(tagDto);
                }

                var eventDto = new EventDto
                {
                    Id = @event.Id.ToString(),
                    Name = @event.Name,
                    Description = @event.Description,
                    Time = @event.Time,
                    Price = @event.Price,
                    Images = @event.Images,
                    Category = categoryDto ?? null,
                    Tags = tagsDto,
                    Dates = datesDto ?? null,
                    CreatedAt = @event.CreatedAt
                };
                eventsDto.Add(eventDto);
            }
            return eventsDto;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }

    }

    public async Task<EventDto?> GetEventById(string eventId, bool reviews)
    {
        try
        {
            var @event = await _context.Events
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .Include(x => x.Dates)
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == long.Parse(eventId));

            if (@event == null)
            {
                return null;
            }

            Category? category = @event.Category;
            EventDate[] dates = @event.Dates.ToArray();
            List<Tag> tags = @event.Tags ?? new();
            List<Review> reviewsList = @event.Reviews ?? new();

            CategoryDto? categoryDto = null;

            if (category != null)
            {
                categoryDto = new CategoryDto
                {
                    Id = category.Id.ToString(),
                    Name = category.Name,
                    Icon = category.Icon,
                    Description = category.Description,
                    CreatedAt = category.CreatedAt
                };
            }

            List<EventDateDto> datesDto = new List<EventDateDto>();

            foreach (var date in dates)
            {
                var dateDto = new EventDateDto
                {
                    Id = date.Id.ToString(),
                    Date = date.Date,
                    Seats = date.Seats
                };
                datesDto.Add(dateDto);
            }

            List<TagDto>? tagsDto = null;

            foreach (var tag in tags)
            {
                var tagDto = new TagDto
                {
                    Id = tag.Id.ToString(),
                    Name = tag.Name,
                    Description = tag.Description,
                    CreatedAt = tag.CreatedAt
                };
                tagsDto.Add(tagDto);
            }

            var eventDto = new EventDto
            {
                Id = @event.Id.ToString(),
                Name = @event.Name,
                Description = @event.Description,
                Time = @event.Time,
                Price = @event.Price,
                Images = @event.Images,
                Category = categoryDto ?? null,
                Tags = tagsDto,
                Dates = datesDto ?? null,
                CreatedAt = @event.CreatedAt
            };

            return eventDto;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public async Task<List<EventDto>> GetEventsByCategory(string categoryId, bool reviews, string orderBy, string order, int page, int limit)
    {
        try
        {
            var query = _context.Events
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .Include(x => x.Dates)
                .Include(x => x.Images)
                .Where(x => x.Category.Id == long.Parse(categoryId))
                .Skip((page - 1) * limit)
                .Take(limit);

            switch (orderBy)
            {
                case "price":
                    query = order == "asc" ? query.OrderBy(x => x.Price) : query.OrderByDescending(x => x.Price);
                    break;
                case "name":
                    query = order == "asc" ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
                    break;
                // Add more cases as needed
                default:
                    query = order == "asc" ? query.OrderBy(x => x.CreatedAt) : query.OrderByDescending(x => x.CreatedAt);
                    break;
            }

            var events = await query.ToListAsync();

            if (events.Count == 0)
            {
                return null;
            }

            List<EventDto> eventsDto = new();

            foreach (var @event in events)
            {
                Category? category = @event.Category;
                EventDate[] dates = @event.Dates.ToArray();
                List<Tag> tags = @event.Tags ?? new();
                List<Review> reviewsList = @event.Reviews ?? new();

                CategoryDto? categoryDto = null;

                if (category != null)
                {
                    categoryDto = new CategoryDto
                    {
                        Id = category.Id.ToString(),
                        Name = category.Name,
                        Icon = category.Icon,
                        Description = category.Description,
                        CreatedAt = category.CreatedAt
                    };
                }

                List<EventDateDto> datesDto = new List<EventDateDto>();

                foreach (var date in dates)
                {
                    var dateDto = new EventDateDto
                    {
                        Id = date.Id.ToString(),
                        Date = date.Date,
                        Seats = date.Seats
                    };
                    datesDto.Add(dateDto);
                }

                List<TagDto>? tagsDto = null;

                foreach (var tag in tags)
                {
                    var tagDto = new TagDto
                    {
                        Id = tag.Id.ToString(),
                        Name = tag.Name,
                        Description = tag.Description,
                        CreatedAt = tag.CreatedAt
                    };
                    tagsDto.Add(tagDto);
                }

                var eventDto = new EventDto
                {
                    Id = @event.Id.ToString(),
                    Name = @event.Name,
                    Description = @event.Description,
                    Time = @event.Time,
                    Price = @event.Price,
                    Images = @event.Images,
                    Category = categoryDto ?? null,
                    Tags = tagsDto,
                    Dates = datesDto ?? null,
                    CreatedAt = @event.CreatedAt
                };
                eventsDto.Add(eventDto);
            }

            return eventsDto;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }
    public async Task<string> AddEvent(AddEventModel model, long? userId)
    {
        try
        {
            List<DateSeat>? datesObject = JsonSerializer.Deserialize<List<DateSeat>>(model.Dates); List<EventDate> dates = new();
            if (datesObject == null)
            {
                return "DATESEMPTY";
            }

            for (int i = 0; i < datesObject.Count; i++)
            {
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
                    Date = DateTime.Parse(datesObject[i].dateIso)
                        .ToUniversalTime(),
                    Seats = datesObject[i].seats,
                    Location = null
                };

                await _context.EventDates.AddAsync(date);
                dates.Add(date);
            }

            List<Tag> tags = new();
            if (model.Tags != null)
            {
                List<string> tagsList = model.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(); foreach (var tagName in tagsList)
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

            var imagesList = imagesBase64.Select(i => new Image
            {
                Id = Snowflake.GenerateId(),
                Base64 = i
            }).ToList();

            var @event = new Event
            {
                Id = Snowflake.GenerateId(),
                Name = model.Name,
                Description = model.Description,
                Time = model.Time,
                Price = model.Price,
                Images = imagesList,
                Category = category,
                Tags = tags,
                Dates = dates,
                CreatedAt = DateTime.UtcNow,
            };

            await _context.Events.AddAsync(@event);
            await _context.SaveChangesAsync();

            return "SUCCESS";
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return e.Message;
        }
    }
}

public interface IEventService
{
    Task<List<EventDto>?> GetEvents(bool reviews, string orderBy, string order, int page, int limit);
    Task<EventDto?> GetEventById(string eventId, bool reviews);
    Task<List<EventDto>> GetEventsByCategory(string categoryId, bool reviews, string orderBy, string order, int page, int limit);
    Task<string> AddEvent(AddEventModel model, long? userId);
}