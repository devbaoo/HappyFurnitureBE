namespace HappyFurnitureBE.Application.DTOs.CompanyInfo;

public class CompanyInfoDto
{
    public int Id { get; set; }
    public string NameVi { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? Email { get; set; }
    public string? PhoneVi { get; set; }
    public string? PhoneEn { get; set; }
    public string? FaxVi { get; set; }
    public string? FaxEn { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CompanyInfoListDto
{
    public int Id { get; set; }
    public string NameVi { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? Email { get; set; }
    public string? PhoneVi { get; set; }
    public string? PhoneEn { get; set; }
    public string? FaxVi { get; set; }
    public string? FaxEn { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCompanyInfoRequest
{
    public string NameVi { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? Email { get; set; }
    public string? PhoneVi { get; set; }
    public string? PhoneEn { get; set; }
    public string? FaxVi { get; set; }
    public string? FaxEn { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

public class UpdateCompanyInfoRequest
{
    public string NameVi { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? Email { get; set; }
    public string? PhoneVi { get; set; }
    public string? PhoneEn { get; set; }
    public string? FaxVi { get; set; }
    public string? FaxEn { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}
