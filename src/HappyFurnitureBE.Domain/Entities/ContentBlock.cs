using System.ComponentModel.DataAnnotations;

namespace HappyFurnitureBE.Domain.Entities;

public enum ContentBlockType
{
    Text = 0,
    Image = 1,
    TextColumns = 2,
    ImageColumns = 3
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

    // TextColumns: cột phải
    [MaxLength(255)]
    public string? Title2Vi { get; set; }

    [MaxLength(255)]
    public string? Title2En { get; set; }

    public string? Content2Vi { get; set; }

    public string? Content2En { get; set; }

    // ImageColumns: ảnh phải
    [MaxLength(500)]
    public string? Image2Url { get; set; }

    [MaxLength(255)]
    public string? Image2AltVi { get; set; }

    [MaxLength(255)]
    public string? Image2AltEn { get; set; }
}
