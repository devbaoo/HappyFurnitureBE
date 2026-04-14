namespace HappyFurnitureBE.Application.DTOs.Certificate;

public class CertificateDto
{
    public int Id { get; set; }
    public string NameVi { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? DescriptionVi { get; set; }
    public string? DescriptionEn { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CertificateListDto
{
    public int Id { get; set; }
    public string NameVi { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? DescriptionVi { get; set; }
    public string? DescriptionEn { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCertificateRequest
{
    public string NameVi { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? DescriptionVi { get; set; }
    public string? DescriptionEn { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}

public class UpdateCertificateRequest
{
    public string NameVi { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? DescriptionVi { get; set; }
    public string? DescriptionEn { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}
