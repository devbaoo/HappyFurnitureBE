using System.ComponentModel.DataAnnotations;

namespace HappyFurnitureBE.Domain.Entities;

public class Certificate : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string NameVi { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? NameEn { get; set; }

    [MaxLength(500)]
    public string? DescriptionVi { get; set; }

    [MaxLength(500)]
    public string? DescriptionEn { get; set; }

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; } = 0;
}
