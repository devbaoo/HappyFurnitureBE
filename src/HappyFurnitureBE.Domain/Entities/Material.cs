using System.ComponentModel.DataAnnotations;

namespace HappyFurnitureBE.Domain.Entities;

public class Material : BaseEntity
{
    [MaxLength(255)]
    public string NameVi { get; set; } = string.Empty;

    [MaxLength(255)]
    public string NameEn { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? DescriptionVi { get; set; }

    [MaxLength(1000)]
    public string? DescriptionEn { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
}
