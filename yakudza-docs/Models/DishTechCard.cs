namespace yakudza_docs.Models;

public class DishTechCard
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public byte[]? Image { get; set; }

    public ICollection<DishIngredient> Ingredients { get; set; } = new List<DishIngredient>();
}
