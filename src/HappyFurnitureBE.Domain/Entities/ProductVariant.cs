using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HappyFurnitureBE.Domain.Entities;

public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }

    [MaxLength(100)]
    public string? ColorName { get; set; }

    [MaxLength(100)]
    public string? ColorNameEn { get; set; }

    [MaxLength(150)]
    public string? Slug { get; set; }

    [MaxLength(7)]
    public string? ColorCode { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDefault { get; set; } = false;

    // Navigation properties
    public Product Product { get; set; } = null!;
    public ICollection<ProductVariantImage> ProductVariantImages { get; set; } = new List<ProductVariantImage>();
}