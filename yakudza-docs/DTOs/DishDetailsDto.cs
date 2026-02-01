namespace yakudza_docs.DTOs;

public class DishDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool HasImage { get; set; }
    public List<IngredientDto> Ingredients { get; set; } = new();
}
