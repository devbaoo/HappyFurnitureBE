using System.ComponentModel.DataAnnotations;

namespace HappyFurnitureBE.Domain.Entities;

public class Product : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? NameEn { get; set; }

    [Required]
    [MaxLength(255)]
    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? DescriptionEn { get; set; }

    [MaxLength(50)]
    public string? DimensionsHeight { get; set; }

    [MaxLength(50)]
    public string? DimensionsWidth { get; set; }

    [MaxLength(50)]
    public string? DimensionsDepth { get; set; }

    [MaxLength(10)]
    public string DimensionUnit { get; set; } = "cm";

    public string? Detail { get; set; }

    public string? DetailEn { get; set; }

    public string? DeliveryInfo { get; set; }

    public string? DeliveryInfoEn { get; set; }

    [MaxLength(50)]
    public string? Weight { get; set; }

    [MaxLength(50)]
    public string? DeliveryHeight { get; set; }

    [MaxLength(50)]
    public string? DeliveryWidth { get; set; }

    [MaxLength(50)]
    public string? DeliveryDepth { get; set; }

    public bool IsFeatured { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public int? AssemblyId { get; set; }
    public Assembly? Assembly { get; set; }

    // Navigation properties
    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    public ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
    public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}