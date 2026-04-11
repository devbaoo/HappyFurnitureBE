using System.ComponentModel.DataAnnotations;

namespace HappyFurnitureBE.Domain.Entities;

public enum ContentBlockType
{
    Text = 0,
    Image = 1
}

public class ContentBlock : BaseEntity
{
    public int NewsId { get; set; }
    
    public News? News { get; set; }

    public ContentBlockType Type { get; set; } = ContentBlockType.Text;

    [MaxLength(255)]
    public string TitleVi { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? TitleEn { get; set; }

    public string? ContentVi { get; set; }

    public string? ContentEn { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(255)]
    public string? ImageAltVi { get; set; }

    [MaxLength(255)]
    public string? ImageAltEn { get; set; }

    public int SortOrder { get; set; } = 0;

    public bool IsFullWidth { get; set; } = false;

    /// <summary>Bố cục ảnh: full | left | right. Chỉ dùng khi Type = Image.</summary>
    [MaxLength(10)]
    public string? ImagePosition { get; set; }
}
