using Backend.Dto;

namespace Backend.Dto;

public class DateSeatLocation
{
    public string dateIso { get; set; }
    public int seats { get; set; }
    public EventLocationWithoutId location { get; set; }
}