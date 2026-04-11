using System.ComponentModel.DataAnnotations;

namespace HappyFurnitureBE.Domain.Entities;

public enum NewsType
{
    News = 0,
    CompanyActivity = 1
}

public class News : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string TitleVi { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? TitleEn { get; set; }

    [Required]
    [MaxLength(255)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? MetaTitleVi { get; set; }

    [MaxLength(255)]
    public string? MetaTitleEn { get; set; }

    [MaxLength(500)]
    public string? MetaDescriptionVi { get; set; }

    [MaxLength(500)]
    public string? MetaDescriptionEn { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(500)]
    public string? BannerUrl { get; set; }

    public string? ExcerptVi { get; set; }

    public string? ExcerptEn { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; } = 0;

    public NewsType Type { get; set; } = NewsType.News;

    public ICollection<ContentBlock> ContentBlocks { get; set; } = new List<ContentBlock>();
}
