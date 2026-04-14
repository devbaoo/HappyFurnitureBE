using System.ComponentModel.DataAnnotations;

namespace HappyFurnitureBE.Domain.Entities;

public class CompanyInfo : BaseEntity
{
    [MaxLength(255)]
    public string NameVi { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? NameEn { get; set; }

    [MaxLength(255)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? PhoneVi { get; set; }

    [MaxLength(50)]
    public string? PhoneEn { get; set; }

    [MaxLength(50)]
    public string? FaxVi { get; set; }

    [MaxLength(50)]
    public string? FaxEn { get; set; }

    public int SortOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}
