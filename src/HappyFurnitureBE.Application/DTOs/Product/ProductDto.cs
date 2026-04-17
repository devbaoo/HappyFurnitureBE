using System.ComponentModel.DataAnnotations;
using HappyFurnitureBE.Application.DTOs.Assembly;
using HappyFurnitureBE.Application.DTOs.Category;
using HappyFurnitureBE.Application.DTOs.Material;

namespace HappyFurnitureBE.Application.DTOs.Product;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DimensionsHeight { get; set; }
    public string? DimensionsWidth { get; set; }
    public string? DimensionsDepth { get; set; }
    public string DimensionUnit { get; set; } = "cm";
    public string? Detail { get; set; }
    public string? DetailEn { get; set; }
    public string? DeliveryInfo { get; set; }
    public string? DeliveryInfoEn { get; set; }
    public string? Weight { get; set; }
    public string? DeliveryHeight { get; set; }
    public string? DeliveryWidth { get; set; }
    public string? DeliveryDepth { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }
    public int? AssemblyId { get; set; }
    public AssemblyDto? Assembly { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public List<CategoryDto> Categories { get; set; } = new();
    public List<MaterialDto> Materials { get; set; } = new();
    public List<ProductVariantDto> Variants { get; set; } = new();
    public List<ProductImageDto> Images { get; set; } = new();
}

public class CreateProductRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255, ErrorMessage = "Name (EN) cannot exceed 255 characters")]
    public string? NameEn { get; set; }

    [Required(ErrorMessage = "Slug is required")]
    [MaxLength(255, ErrorMessage = "Slug cannot exceed 255 characters")]
    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? DescriptionEn { get; set; }

    [MaxLength(50, ErrorMessage = "Height cannot exceed 50 characters")]
    public string? DimensionsHeight { get; set; }

    [MaxLength(50, ErrorMessage = "Width cannot exceed 50 characters")]
    public string? DimensionsWidth { get; set; }

    [MaxLength(50, ErrorMessage = "Depth cannot exceed 50 characters")]
    public string? DimensionsDepth { get; set; }

    [MaxLength(10, ErrorMessage = "Dimension unit cannot exceed 10 characters")]
    public string DimensionUnit { get; set; } = "cm";

    public string? Detail { get; set; }
    public string? DetailEn { get; set; }
    public string? DeliveryInfo { get; set; }
    public string? DeliveryInfoEn { get; set; }

    [MaxLength(50, ErrorMessage = "Weight cannot exceed 50 characters")]
    public string? Weight { get; set; }

    [MaxLength(50, ErrorMessage = "Delivery height cannot exceed 50 characters")]
    public string? DeliveryHeight { get; set; }

    [MaxLength(50, ErrorMessage = "Delivery width cannot exceed 50 characters")]
    public string? DeliveryWidth { get; set; }

    [MaxLength(50, ErrorMessage = "Delivery depth cannot exceed 50 characters")]
    public string? DeliveryDepth { get; set; }

    public bool IsFeatured { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int? AssemblyId { get; set; }

    public List<int> CategoryIds { get; set; } = new();
    public List<int> MaterialIds { get; set; } = new();

    // Image URLs (có thể từ upload trước đó)
    public List<string> ImageUrls { get; set; } = new();

    // Default variant (tự động tạo khi tạo product)
    [MaxLength(100)]
    public string? DefaultVariantColorName { get; set; } = "Mặc định";

    [MaxLength(100)]
    public string? DefaultVariantColorNameEn { get; set; } = "Default";

    [MaxLength(7)]
    public string? DefaultVariantColorCode { get; set; } = "#FFFFFF";

    [MaxLength(150)]
    public string? DefaultVariantSlug { get; set; }

    [MaxLength(500)]
    public string? DefaultVariantImageUrl { get; set; }
}

public class CreateProductWithImagesRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255, ErrorMessage = "Name (EN) cannot exceed 255 characters")]
    public string? NameEn { get; set; }

    [Required(ErrorMessage = "Slug is required")]
    [MaxLength(255, ErrorMessage = "Slug cannot exceed 255 characters")]
    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? DescriptionEn { get; set; }

    [MaxLength(50, ErrorMessage = "Height cannot exceed 50 characters")]
    public string? DimensionsHeight { get; set; }

    [MaxLength(50, ErrorMessage = "Width cannot exceed 50 characters")]
    public string? DimensionsWidth { get; set; }

    [MaxLength(50, ErrorMessage = "Depth cannot exceed 50 characters")]
    public string? DimensionsDepth { get; set; }

    [MaxLength(10, ErrorMessage = "Dimension unit cannot exceed 10 characters")]
    public string DimensionUnit { get; set; } = "cm";

    public string? Detail { get; set; }
    public string? DetailEn { get; set; }
    public string? DeliveryInfo { get; set; }
    public string? DeliveryInfoEn { get; set; }

    [MaxLength(50, ErrorMessage = "Weight cannot exceed 50 characters")]
    public string? Weight { get; set; }

    [MaxLength(50, ErrorMessage = "Delivery height cannot exceed 50 characters")]
    public string? DeliveryHeight { get; set; }

    [MaxLength(50, ErrorMessage = "Delivery width cannot exceed 50 characters")]
    public string? DeliveryWidth { get; set; }

    [MaxLength(50, ErrorMessage = "Delivery depth cannot exceed 50 characters")]
    public string? DeliveryDepth { get; set; }

    public bool IsFeatured { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int? AssemblyId { get; set; }

    public List<int> CategoryIds { get; set; } = new();
    public List<int> MaterialIds { get; set; } = new();

    // Default variant (tự động tạo khi tạo product)
    [MaxLength(100)]
    public string? DefaultVariantColorName { get; set; } = "Mặc định";

    [MaxLength(100)]
    public string? DefaultVariantColorNameEn { get; set; } = "Default";

    [MaxLength(7)]
    public string? DefaultVariantColorCode { get; set; } = "#FFFFFF";

    [MaxLength(150)]
    public string? DefaultVariantSlug { get; set; }

    [MaxLength(500)]
    public string? DefaultVariantImageUrl { get; set; }
}

public class UpdateProductRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255, ErrorMessage = "Name (EN) cannot exceed 255 characters")]
    public string? NameEn { get; set; }

    [Required(ErrorMessage = "Slug is required")]
    [MaxLength(255, ErrorMessage = "Slug cannot exceed 255 characters")]
    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? DescriptionEn { get; set; }

    [MaxLength(50, ErrorMessage = "Height cannot exceed 50 characters")]
    public string? DimensionsHeight { get; set; }

    [MaxLength(50, ErrorMessage = "Width cannot exceed 50 characters")]
    public string? DimensionsWidth { get; set; }

    [MaxLength(50, ErrorMessage = "Depth cannot exceed 50 characters")]
    public string? DimensionsDepth { get; set; }

    [MaxLength(10, ErrorMessage = "Dimension unit cannot exceed 10 characters")]
    public string DimensionUnit { get; set; } = "cm";

    public string? Detail { get; set; }
    public string? DetailEn { get; set; }
    public string? DeliveryInfo { get; set; }
    public string? DeliveryInfoEn { get; set; }

    [MaxLength(50, ErrorMessage = "Weight cannot exceed 50 characters")]
    public string? Weight { get; set; }

    [MaxLength(50, ErrorMessage = "Delivery height cannot exceed 50 characters")]
    public string? DeliveryHeight { get; set; }

    [MaxLength(50, ErrorMessage = "Delivery width cannot exceed 50 characters")]
    public string? DeliveryWidth { get; set; }

    [MaxLength(50, ErrorMessage = "Delivery depth cannot exceed 50 characters")]
    public string? DeliveryDepth { get; set; }

    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }
    public int? AssemblyId { get; set; }

    public List<int> CategoryIds { get; set; } = new();
    public List<int> MaterialIds { get; set; } = new();

    // Nếu truyền lên thì backend sẽ sync lại ảnh của product.
    // Nếu bỏ qua field này thì backend sẽ giữ nguyên ProductImages hiện có.
    public List<string>? ImageUrls { get; set; }
}

public class ProductFilterParams
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public int? CategoryId { get; set; }
    public int? MaterialId { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsActive { get; set; }
    public int? AssemblyId { get; set; }
    public string? SortBy { get; set; } = "CreatedAt"; // Name, CreatedAt
    public string? SortOrder { get; set; } = "desc"; // asc, desc
}

// ─── Product Image DTOs ──────────────────────────────────────────────────────

public class ProductImageDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateProductImageRequest
{
    [Required(ErrorMessage = "Product ID is required")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Image URL is required")]
    [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(255, ErrorMessage = "Alt text cannot exceed 255 characters")]
    public string? AltText { get; set; }

    public bool IsPrimary { get; set; } = false;

    public int SortOrder { get; set; } = 0;
}

public class UpdateProductImageRequest
{
    [Required(ErrorMessage = "Image URL is required")]
    [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(255, ErrorMessage = "Alt text cannot exceed 255 characters")]
    public string? AltText { get; set; }

    public bool IsPrimary { get; set; }

    public int SortOrder { get; set; }
}

// ─── Product Variant DTOs ────────────────────────────────────────────────────

public class ProductVariantDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? ColorName { get; set; }
    public string? ColorNameEn { get; set; }
    public string? Slug { get; set; }
    public string? ColorCode { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ProductVariantImageDto> Images { get; set; } = new();
}

public class CreateProductVariantRequest
{
    [Required(ErrorMessage = "Product ID is required")]
    public int ProductId { get; set; }

    [MaxLength(100, ErrorMessage = "Color name cannot exceed 100 characters")]
    public string? ColorName { get; set; }

    [MaxLength(100, ErrorMessage = "Color name (EN) cannot exceed 100 characters")]
    public string? ColorNameEn { get; set; }

    /// <summary>Tự động tạo từ ColorName nếu để trống</summary>
    [MaxLength(150, ErrorMessage = "Slug cannot exceed 150 characters")]
    public string? Slug { get; set; }

    [MaxLength(7, ErrorMessage = "Color code cannot exceed 7 characters")]
    public string? ColorCode { get; set; }

    [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;
}

public class CreateProductVariantWithImageRequest
{
    [Required(ErrorMessage = "Product ID is required")]
    public int ProductId { get; set; }

    [MaxLength(100, ErrorMessage = "Color name cannot exceed 100 characters")]
    public string? ColorName { get; set; }

    [MaxLength(100, ErrorMessage = "Color name (EN) cannot exceed 100 characters")]
    public string? ColorNameEn { get; set; }

    [MaxLength(150, ErrorMessage = "Slug cannot exceed 150 characters")]
    public string? Slug { get; set; }

    [MaxLength(7, ErrorMessage = "Color code cannot exceed 7 characters")]
    public string? ColorCode { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdateProductVariantRequest
{
    [MaxLength(100, ErrorMessage = "Color name cannot exceed 100 characters")]
    public string? ColorName { get; set; }

    [MaxLength(100, ErrorMessage = "Color name (EN) cannot exceed 100 characters")]
    public string? ColorNameEn { get; set; }

    [MaxLength(150, ErrorMessage = "Slug cannot exceed 150 characters")]
    public string? Slug { get; set; }

    [MaxLength(7, ErrorMessage = "Color code cannot exceed 7 characters")]
    public string? ColorCode { get; set; }

    [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; }
}

// ─── Product Variant Image DTOs ──────────────────────────────────────────────

public class ProductVariantImageDto
{
    public int Id { get; set; }
    public int VariantId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateProductVariantImageRequest
{
    [Required(ErrorMessage = "Variant ID is required")]
    public int VariantId { get; set; }

    [Required(ErrorMessage = "Image URL is required")]
    [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(255, ErrorMessage = "Alt text cannot exceed 255 characters")]
    public string? AltText { get; set; }

    public bool IsPrimary { get; set; } = false;

    public int SortOrder { get; set; } = 0;
}

public class UpdateProductVariantImageRequest
{
    [Required(ErrorMessage = "Image URL is required")]
    [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(255, ErrorMessage = "Alt text cannot exceed 255 characters")]
    public string? AltText { get; set; }

    public bool IsPrimary { get; set; }

    public int SortOrder { get; set; }
}
