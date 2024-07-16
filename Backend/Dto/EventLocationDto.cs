namespace Backend.Dto;

public class EventLocationDto
{
    public required long Id { get; set; }
    public required string Street { get; set; }
    public required string HouseNumber { get; set; }
    public required string PostalCode { get; set; }
    public required string City { get; set; }
    public required string AdditionalInfo { get; set; }
}
