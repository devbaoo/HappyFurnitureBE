using System.ComponentModel.DataAnnotations;

namespace HappyFurnitureBE.Domain.Entities;

public class Category : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public int? ParentId { get; set; }

    /// <summary>
    /// Số thứ tự hiển thị - chỉ dùng cho root category (ParentId == null).
    /// Dùng để sắp xếp thứ tự hiển thị các category cha.
    /// </summary>
    public int? SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();
    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}