using HappyFurnitureBE.Application.DTOs.News;
using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyFurnitureBE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly INewsRepository _newsRepo;
    private readonly IContentBlockRepository _contentBlockRepo;

    public NewsController(INewsRepository newsRepo, IContentBlockRepository contentBlockRepo)
    {
        _newsRepo = newsRepo;
        _contentBlockRepo = contentBlockRepo;
    }

    /// <summary>
    /// Public: Lấy danh sách News & Company Activities (phân cách riêng)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<NewsSectionResponse>> GetAllActive()
    {
        var news = await _newsRepo.GetActiveNewsAsync();
        var activities = await _newsRepo.GetActiveCompanyActivitiesAsync();

        var response = new NewsSectionResponse
        {
            News = news.Select(MapToListDto).ToList(),
            CompanyActivities = activities.Select(MapToListDto).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Public: Lấy danh sách News & Company Activities (phân cách riêng) - alias cho /api/News
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<NewsSectionResponse>> GetActiveNews()
    {
        var news = await _newsRepo.GetActiveNewsAsync();
        var activities = await _newsRepo.GetActiveCompanyActivitiesAsync();

        var response = new NewsSectionResponse
        {
            News = news.Select(MapToListDto).ToList(),
            CompanyActivities = activities.Select(MapToListDto).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Public: Lấy danh sách News (tin tức)
    /// </summary>
    [HttpGet("news")]
    public async Task<ActionResult<object>> GetNews(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var all = await _newsRepo.GetActiveNewsAsync();
        var list = all.ToList();
        var total = list.Count;
        var items = list.Skip((page - 1) * pageSize).Take(pageSize).Select(MapToListDto).ToList();

        return Ok(new { items, total, page, pageSize });
    }

    /// <summary>
    /// Public: Lấy danh sách Company Activities (hoạt động công ty)
    /// </summary>
    [HttpGet("company-activities")]
    public async Task<ActionResult<object>> GetCompanyActivities(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var all = await _newsRepo.GetActiveCompanyActivitiesAsync();
        var list = all.ToList();
        var total = list.Count;
        var items = list.Skip((page - 1) * pageSize).Take(pageSize).Select(MapToListDto).ToList();

        return Ok(new { items, total, page, pageSize });
    }

    /// <summary>
    /// Public: Chi tiết News/CompanyActivity theo slug (kèm content blocks)
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<NewsDetailDto>> GetBySlug(string slug)
    {
        var news = await _newsRepo.GetBySlugAsync(slug);
        if (news == null) return NotFound(new { message = "Not found" });
        return Ok(await MapToDetailDto(news.Id));
    }

    /// <summary>
    /// Public: Chi tiết News/CompanyActivity theo id (kèm content blocks)
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<NewsDetailDto>> GetById(int id)
    {
        var news = await _newsRepo.GetByIdWithDetailsAsync(id);
        if (news == null) return NotFound(new { message = "Not found" });
        return Ok(await MapToDetailDto(id));
    }

    /// <summary>
    /// Admin: Danh sách phân trang (tất cả loại)
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpGet("admin/all")]
    public async Task<ActionResult<object>> GetAllAdmin(
        [FromQuery] string? type,
        [FromQuery] string? title,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var all = await _newsRepo.GetAllWithFilterAsync(type, title, isActive);
        var list = all.ToList();
        var total = list.Count;
        var items = list.Skip((page - 1) * pageSize).Take(pageSize).Select(MapToDto).ToList();

        return Ok(new { items, total, page, pageSize });
    }

    /// <summary>
    /// Admin: Tạo mới News hoặc Company Activity
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<ActionResult<NewsDetailDto>> Create([FromBody] CreateNewsRequest req)
    {
        if (await _newsRepo.SlugExistsAsync(req.Slug))
            return BadRequest(new { message = "Slug already exists" });

        var news = new News
        {
            TitleVi = req.TitleVi,
            TitleEn = req.TitleEn,
            Slug = req.Slug,
            MetaTitleVi = req.MetaTitleVi,
            MetaTitleEn = req.MetaTitleEn,
            MetaDescriptionVi = req.MetaDescriptionVi,
            MetaDescriptionEn = req.MetaDescriptionEn,
            ImageUrl = req.ImageUrl,
            BannerUrl = req.BannerUrl,
            ExcerptVi = req.ExcerptVi,
            ExcerptEn = req.ExcerptEn,
            IsActive = req.IsActive,
            SortOrder = req.SortOrder,
            Type = Enum.Parse<NewsType>(req.Type),
        };

        var created = await _newsRepo.AddAsync(news);

        if (req.ContentBlocks != null && req.ContentBlocks.Any())
        {
            foreach (var blockReq in req.ContentBlocks)
            {
                var block = new ContentBlock
                {
                    NewsId = created.Id,
                    Type = Enum.Parse<ContentBlockType>(blockReq.Type),
                    TitleVi = blockReq.TitleVi,
                    TitleEn = blockReq.TitleEn,
                    ContentVi = blockReq.ContentVi,
                    ContentEn = blockReq.ContentEn,
                    ImageUrl = blockReq.ImageUrl,
                    ImageAltVi = blockReq.ImageAltVi,
                    ImageAltEn = blockReq.ImageAltEn,
                    SortOrder = blockReq.SortOrder,
                    IsFullWidth = blockReq.IsFullWidth,
                    ImagePosition = blockReq.ImagePosition
                };
                await _contentBlockRepo.AddAsync(block);
            }
        }

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, await MapToDetailDto(created.Id));
    }

    /// <summary>
    /// Admin: Cập nhật News hoặc Company Activity
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<NewsDetailDto>> Update(int id, [FromBody] UpdateNewsRequest req)
    {
        var existing = await _newsRepo.GetByIdWithDetailsAsync(id);
        if (existing == null) return NotFound(new { message = "Not found" });

        if (await _newsRepo.SlugExistsAsync(req.Slug, id))
            return BadRequest(new { message = "Slug already exists" });

        existing.TitleVi = req.TitleVi;
        existing.TitleEn = req.TitleEn;
        existing.Slug = req.Slug;
        existing.MetaTitleVi = req.MetaTitleVi;
        existing.MetaTitleEn = req.MetaTitleEn;
        existing.MetaDescriptionVi = req.MetaDescriptionVi;
        existing.MetaDescriptionEn = req.MetaDescriptionEn;
        existing.ImageUrl = req.ImageUrl;
        existing.BannerUrl = req.BannerUrl;
        existing.ExcerptVi = req.ExcerptVi;
        existing.ExcerptEn = req.ExcerptEn;
        existing.IsActive = req.IsActive;
        existing.SortOrder = req.SortOrder;
        existing.Type = Enum.Parse<NewsType>(req.Type);

        await _newsRepo.UpdateAsync(existing);

        if (req.ContentBlocks != null)
        {
            await _contentBlockRepo.DeleteByNewsIdAsync(id);

            foreach (var blockReq in req.ContentBlocks)
            {
                var block = new ContentBlock
                {
                    NewsId = id,
                    Type = Enum.Parse<ContentBlockType>(blockReq.Type),
                    TitleVi = blockReq.TitleVi,
                    TitleEn = blockReq.TitleEn,
                    ContentVi = blockReq.ContentVi,
                    ContentEn = blockReq.ContentEn,
                    ImageUrl = blockReq.ImageUrl,
                    ImageAltVi = blockReq.ImageAltVi,
                    ImageAltEn = blockReq.ImageAltEn,
                    SortOrder = blockReq.SortOrder,
                    IsFullWidth = blockReq.IsFullWidth,
                    ImagePosition = blockReq.ImagePosition
                };
                await _contentBlockRepo.AddAsync(block);
            }
        }

        return Ok(await MapToDetailDto(id));
    }

    /// <summary>
    /// Admin: Xóa News hoặc Company Activity (kèm content blocks)
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _newsRepo.GetByIdWithDetailsAsync(id);
        if (existing == null) return NotFound(new { message = "Not found" });

        await _contentBlockRepo.DeleteByNewsIdAsync(id);
        await _newsRepo.DeleteAsync(id);
        return NoContent();
    }

    private async Task<NewsDetailDto> MapToDetailDto(int newsId)
    {
        var news = await _newsRepo.GetByIdWithDetailsAsync(newsId);
        var blocks = await _contentBlockRepo.GetByNewsIdOrderedAsync(newsId);

        return new NewsDetailDto
        {
            Id = news!.Id,
            TitleVi = news.TitleVi,
            TitleEn = news.TitleEn,
            Slug = news.Slug,
            MetaTitleVi = news.MetaTitleVi,
            MetaTitleEn = news.MetaTitleEn,
            MetaDescriptionVi = news.MetaDescriptionVi,
            MetaDescriptionEn = news.MetaDescriptionEn,
            ImageUrl = news.ImageUrl,
            BannerUrl = news.BannerUrl,
            ExcerptVi = news.ExcerptVi,
            ExcerptEn = news.ExcerptEn,
            IsActive = news.IsActive,
            SortOrder = news.SortOrder,
            Type = news.Type.ToString(),
            ContentBlocks = blocks.Select(MapContentBlockToDto).ToList(),
            CreatedAt = news.CreatedAt,
            UpdatedAt = news.UpdatedAt
        };
    }

    private static NewsDto MapToDto(News n)
    {
        return new NewsDto
        {
            Id = n.Id,
            TitleVi = n.TitleVi,
            TitleEn = n.TitleEn,
            Slug = n.Slug,
            MetaTitleVi = n.MetaTitleVi,
            MetaTitleEn = n.MetaTitleEn,
            MetaDescriptionVi = n.MetaDescriptionVi,
            MetaDescriptionEn = n.MetaDescriptionEn,
            ImageUrl = n.ImageUrl,
            BannerUrl = n.BannerUrl,
            ExcerptVi = n.ExcerptVi,
            ExcerptEn = n.ExcerptEn,
            IsActive = n.IsActive,
            SortOrder = n.SortOrder,
            Type = n.Type.ToString(),
            CreatedAt = n.CreatedAt,
            UpdatedAt = n.UpdatedAt
        };
    }

    private static NewsListDto MapToListDto(News n)
    {
        return new NewsListDto
        {
            Id = n.Id,
            TitleVi = n.TitleVi,
            TitleEn = n.TitleEn,
            Slug = n.Slug,
            ImageUrl = n.ImageUrl,
            ExcerptVi = n.ExcerptVi,
            ExcerptEn = n.ExcerptEn,
            SortOrder = n.SortOrder,
            Type = n.Type.ToString(),
            CreatedAt = n.CreatedAt
        };
    }

    private static ContentBlockDto MapContentBlockToDto(ContentBlock cb)
    {
        // Khi imagePosition chưa được set (data cũ), fallback an toàn:
        // - Có giá trị hợp lệ ("full"/"left"/"right") → dùng nguyên
        // - null/rỗng + isFullWidth=true  → "full"
        // - null/rỗng + isFullWidth=false → "full" (an toàn, không đoán left/right)
        var validPositions = new[] { "full", "left", "right" };
        var imagePosition = (!string.IsNullOrWhiteSpace(cb.ImagePosition) && validPositions.Contains(cb.ImagePosition))
            ? cb.ImagePosition
            : (cb.IsFullWidth ? "full" : "full");

        return new ContentBlockDto
        {
            Id = cb.Id,
            NewsId = cb.NewsId,
            Type = cb.Type.ToString(),
            TitleVi = cb.TitleVi,
            TitleEn = cb.TitleEn,
            ContentVi = cb.ContentVi,
            ContentEn = cb.ContentEn,
            ImageUrl = cb.ImageUrl,
            ImageAltVi = cb.ImageAltVi,
            ImageAltEn = cb.ImageAltEn,
            SortOrder = cb.SortOrder,
            IsFullWidth = cb.IsFullWidth,
            ImagePosition = imagePosition
        };
    }
}
