namespace Backend.ViewModels;

public class UpdateCategoryModel
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Icon { get; set; }
}