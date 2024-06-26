namespace Backend.ViewModels;

public class UpdateProductModel
{
    public string? Name { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public IFormFile? Image0 { get; set; }
    public IFormFile? Image1 { get; set; }
    public IFormFile? Image2 { get; set; }
    public IFormFile? Image3 { get; set; }
    public IFormFile? Image4 { get; set; }
    public IFormFile? Image5 { get; set; }
    public float? Price { get; set; }
    public string? CategoryId { get; set; }
    public List<string>? Tags { get; set; }
    public string? RemovedImages { get; set; }
}
