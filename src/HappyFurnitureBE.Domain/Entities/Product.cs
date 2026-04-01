using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HappyFurnitureBE.Domain.Entities;

public class Product : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? OldPrice { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? DimensionsHeight { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? DimensionsWidth { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? DimensionsDepth { get; set; }

    [MaxLength(10)]
    public string DimensionUnit { get; set; } = "cm";

    public string? Detail { get; set; }

    public string? DeliveryInfo { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? Weight { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? DeliveryHeight { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? DeliveryWidth { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? DeliveryDepth { get; set; }

    public bool IsFeatured { get; set; } = false;

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    public ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
    public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}