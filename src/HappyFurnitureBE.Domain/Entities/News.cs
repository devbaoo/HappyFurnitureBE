using System.ComponentModel.DataAnnotations;

namespace HappyFurnitureBE.Domain.Entities;

public enum NewsType
{
    Event = 0,
    Activity = 1
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

    public string? ContentVi { get; set; }

    public string? ContentEn { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public string? ExcerptVi { get; set; }

    public string? ExcerptEn { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; } = 0;

    // Event | Activity
    public NewsType Type { get; set; } = NewsType.Event;


}
