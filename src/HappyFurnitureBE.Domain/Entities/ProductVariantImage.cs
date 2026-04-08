using System.ComponentModel.DataAnnotations;

namespace HappyFurnitureBE.Domain.Entities;

public class ProductVariantImage : BaseEntity
{
    public int VariantId { get; set; }

    [Required]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? AltText { get; set; }

    public bool IsPrimary { get; set; } = false;

    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public ProductVariant Variant { get; set; } = null!;
}
