namespace Backend.ViewModels;

public class AddEventModel
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required int Time { get; set; }
    public required float Price { get; set; }
    public IFormFile? Image0 { get; set; }
    public IFormFile? Image1 { get; set; }
    public IFormFile? Image2 { get; set; }
    public IFormFile? Image3 { get; set; }
    public IFormFile? Image4 { get; set; }
    public IFormFile? Image5 { get; set; }
    public required string Dates { get; set; }
    public required string Location { get; set; }
    public string? CategoryId { get; set; }
    public string? Tags { get; set; }
}