namespace HappyFurnitureBE.Application.DTOs.News;

public class NewsDto
{
    public int Id { get; set; }
    public string TitleVi { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? MetaTitleVi { get; set; }
    public string? MetaTitleEn { get; set; }
    public string? MetaDescriptionVi { get; set; }
    public string? MetaDescriptionEn { get; set; }
    public string? ImageUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? ExcerptVi { get; set; }
    public string? ExcerptEn { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class NewsListDto
{
    public int Id { get; set; }
    public string TitleVi { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? ExcerptVi { get; set; }
    public string? ExcerptEn { get; set; }
    public int SortOrder { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ContentBlockDto
{
    public int Id { get; set; }
    public int NewsId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? TitleVi { get; set; }
    public string? TitleEn { get; set; }
    public string? ContentVi { get; set; }
    public string? ContentEn { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAltVi { get; set; }
    public string? ImageAltEn { get; set; }
    public int SortOrder { get; set; }
    public bool IsFullWidth { get; set; }
    /// <summary>Bố cục ảnh: full | left | right</summary>
    public string? ImagePosition { get; set; }
}

public class NewsDetailDto
{
    public int Id { get; set; }
    public string TitleVi { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? MetaTitleVi { get; set; }
    public string? MetaTitleEn { get; set; }
    public string? MetaDescriptionVi { get; set; }
    public string? MetaDescriptionEn { get; set; }
    public string? ImageUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? ExcerptVi { get; set; }
    public string? ExcerptEn { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public string Type { get; set; } = string.Empty;
    public List<ContentBlockDto> ContentBlocks { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class NewsSectionResponse
{
    public List<NewsListDto> News { get; set; } = new();
    public List<NewsListDto> CompanyActivities { get; set; } = new();
}

public class NewsResponse
{
    public List<NewsDto> News { get; set; } = new();
    public List<NewsDto> CompanyActivities { get; set; } = new();
}

public class CreateNewsRequest
{
    public string TitleVi { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? MetaTitleVi { get; set; }
    public string? MetaTitleEn { get; set; }
    public string? MetaDescriptionVi { get; set; }
    public string? MetaDescriptionEn { get; set; }
    public string? ImageUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? ExcerptVi { get; set; }
    public string? ExcerptEn { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
    public string Type { get; set; } = "News";
    public List<CreateContentBlockRequest> ContentBlocks { get; set; } = new();
}

public class UpdateNewsRequest
{
    public string TitleVi { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? MetaTitleVi { get; set; }
    public string? MetaTitleEn { get; set; }
    public string? MetaDescriptionVi { get; set; }
    public string? MetaDescriptionEn { get; set; }
    public string? ImageUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? ExcerptVi { get; set; }
    public string? ExcerptEn { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public string Type { get; set; } = "News";
    public List<CreateContentBlockRequest> ContentBlocks { get; set; } = new();
}

public class CreateContentBlockRequest
{
    public string Type { get; set; } = "Text";
    public string? TitleVi { get; set; }
    public string? TitleEn { get; set; }
    public string? ContentVi { get; set; }
    public string? ContentEn { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAltVi { get; set; }
    public string? ImageAltEn { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsFullWidth { get; set; } = false;
    /// <summary>Bố cục ảnh: full | left | right</summary>
    public string? ImagePosition { get; set; }
}

public class NewsFilterParams
{
    public string? Title { get; set; }
    public string? Type { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortOrder { get; set; } = "desc";
}
