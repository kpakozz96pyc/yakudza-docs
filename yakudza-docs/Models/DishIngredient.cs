namespace yakudza_docs.Models;

public class DishIngredient
{
    public int Id { get; set; }
    public int DishTechCardId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal WeightGrams { get; set; }

    public DishTechCard DishTechCard { get; set; } = null!;
}
