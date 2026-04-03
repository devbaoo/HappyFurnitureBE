using System.ComponentModel.DataAnnotations;

namespace HappyFurnitureBE.Application.DTOs.Category;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int? ParentId { get; set; }
    /// <summary>Số thứ tự - chỉ có giá trị với root category</summary>
    public int? SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public CategoryDto? Parent { get; set; }
    public List<CategoryDto> Children { get; set; } = new();
}

public class CreateCategoryRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    public string? ImageUrl { get; set; }

    public int? ParentId { get; set; }

    /// <summary>Số thứ tự hiển thị - chỉ dùng cho root category (khi ParentId == null)</summary>
    public int? SortOrder { get; set; }

    public bool IsActive { get; set; } = true;
}

public class CreateCategoryWithImageRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;

    public int? ParentId { get; set; }

    /// <summary>Số thứ tự hiển thị - chỉ dùng cho root category (khi ParentId == null)</summary>
    public int? SortOrder { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdateCategoryRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    public string? ImageUrl { get; set; }

    public int? ParentId { get; set; }

    /// <summary>Số thứ tự hiển thị - chỉ dùng cho root category (khi ParentId == null)</summary>
    public int? SortOrder { get; set; }

    public bool IsActive { get; set; }
}

public class CategoryFilterParams
{
    public string? Name { get; set; }
    public int? ParentId { get; set; }
    public bool? IsActive { get; set; }
    public bool IncludeChildren { get; set; } = false;
}