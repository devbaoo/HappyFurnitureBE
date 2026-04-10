namespace HappyFurnitureBE.Application.DTOs.News;

public class NewsDto
{
    public int Id { get; set; }
    public string TitleVi { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? ContentVi { get; set; }
    public string? ContentEn { get; set; }
    public string? ImageUrl { get; set; }
    public string? ExcerptVi { get; set; }
    public string? ExcerptEn { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int? Year { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Response cho /News/active — su dung mang phang, khong phan tach teamBuilding/factoryTour.
/// </summary>
public class NewsResponse
{
    public List<NewsDto> Events { get; set; } = new();
    public List<NewsDto> Activities { get; set; } = new();
}

public class CreateNewsRequest
{
    public string TitleVi { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? ContentVi { get; set; }
    public string? ContentEn { get; set; }
    public string? ImageUrl { get; set; }
    public string? ExcerptVi { get; set; }
    public string? ExcerptEn { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
    public string Type { get; set; } = "Event";
    public string? Category { get; set; }
    public int? Year { get; set; }
}

public class UpdateNewsRequest
{
    public string TitleVi { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? ContentVi { get; set; }
    public string? ContentEn { get; set; }
    public string? ImageUrl { get; set; }
    public string? ExcerptVi { get; set; }
    public string? ExcerptEn { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public string Type { get; set; } = "Event";
    public string? Category { get; set; }
    public int? Year { get; set; }
}

public class NewsFilterParams
{
    public string? Title { get; set; }
    public string? Type { get; set; }
    public string? Category { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortOrder { get; set; } = "desc";
}
