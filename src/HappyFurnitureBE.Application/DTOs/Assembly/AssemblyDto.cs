using System.ComponentModel.DataAnnotations;

namespace HappyFurnitureBE.Application.DTOs.Assembly;

public class AssemblyDto
{
    public int Id { get; set; }
    public string NameVi { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? DescriptionVi { get; set; }
    public string? DescriptionEn { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateAssemblyRequest
{
    [Required(ErrorMessage = "Name (Vietnamese) is required")]
    [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string NameVi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name (English) is required")]
    [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string NameEn { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
    public string? Code { get; set; }

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? DescriptionVi { get; set; }

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? DescriptionEn { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdateAssemblyRequest
{
    [Required(ErrorMessage = "Name (Vietnamese) is required")]
    [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string NameVi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name (English) is required")]
    [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string NameEn { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
    public string? Code { get; set; }

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? DescriptionVi { get; set; }

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? DescriptionEn { get; set; }

    public bool IsActive { get; set; }
}

public class AssemblyFilterParams
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
}
