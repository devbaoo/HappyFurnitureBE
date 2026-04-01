namespace HappyFurnitureBE.Domain.Entities;

public class ProductMaterial
{
    public int ProductId { get; set; }
    public int MaterialId { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
    public Material Material { get; set; } = null!;
}
