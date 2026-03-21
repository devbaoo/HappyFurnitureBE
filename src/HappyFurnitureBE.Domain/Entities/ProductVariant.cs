using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HappyFurnitureBE.Domain.Entities;

public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }

    [MaxLength(100)]
    public string? ColorName { get; set; }

    [MaxLength(7)]
    public string? ColorCode { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Price { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Product Product { get; set; } = null!;
}