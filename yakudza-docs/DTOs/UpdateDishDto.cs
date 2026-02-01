using System.ComponentModel.DataAnnotations;

namespace yakudza_docs.DTOs;

public class UpdateDishDto
{
    [Required(ErrorMessage = "Dish name is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Dish name must be between 1 and 200 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string Description { get; set; } = string.Empty;

    public string? ImageBase64 { get; set; }

    [Required(ErrorMessage = "At least one ingredient is required")]
    [MinLength(1, ErrorMessage = "At least one ingredient is required")]
    public List<CreateIngredientDto> Ingredients { get; set; } = new();
}
