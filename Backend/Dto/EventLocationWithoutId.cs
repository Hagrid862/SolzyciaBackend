namespace Backend.Dto;

public class EventLocationWithoutId
{
    public required string Street { get; set; }
    public required string HouseNumber { get; set; }
    public required string PostalCode { get; set; }
    public required string City { get; set; }
    public required string AdditionalInfo { get; set; }
}
