using System.ComponentModel.DataAnnotations;

namespace HappyFurnitureBE.Domain.Entities;

public class ProductImage : BaseEntity
{
    public int ProductId { get; set; }

    [Required]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? AltText { get; set; }

    public bool IsPrimary { get; set; } = false;

    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public Product Product { get; set; } = null!;
}