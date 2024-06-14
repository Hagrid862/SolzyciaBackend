namespace Backend.Models;

public class EventLocation
{
  public required long Id { get; set; }
  public required string Street { get; set; }
  public required string HouseNumber { get; set; }
  public required string PostalCode { get; set; }
  public required string City { get; set; }
}
