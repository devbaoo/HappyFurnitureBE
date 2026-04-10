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

    public NewsController(INewsRepository newsRepo)
    {
        _newsRepo = newsRepo;
    }

    /// <summary>Public: tra ve Events + Activities (mang phang, khong tach teamBuilding/factoryTour).</summary>
    [HttpGet("active")]
    public async Task<ActionResult<NewsResponse>> GetActiveNews()
    {
        var events = await _newsRepo.GetActiveEventsAsync();
        var activities = await _newsRepo.GetActiveActivitiesAsync();

        var response = new NewsResponse
        {
            Events = events.Select(MapToDto).ToList(),
            Activities = activities.Select(MapToDto).ToList()
        };

        return Ok(response);
    }

    /// <summary>Public: chi tiet theo slug.</summary>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<NewsDto>> GetBySlug(string slug)
    {
        var news = await _newsRepo.GetBySlugAsync(slug);
        if (news == null) return NotFound(new { message = "Not found" });
        return Ok(MapToDto(news));
    }

    /// <summary>Public: chi tiet theo id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<NewsDto>> GetById(int id)
    {
        var news = await _newsRepo.GetByIdWithDetailsAsync(id);
        if (news == null) return NotFound(new { message = "Not found" });
        return Ok(MapToDto(news));
    }

    /// <summary>Admin: danh sach phan trang.</summary>
    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<ActionResult<object>> GetAll(
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

    /// <summary>Admin: tao moi.</summary>
    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<ActionResult<NewsDto>> Create([FromBody] CreateNewsRequest req)
    {
        if (await _newsRepo.SlugExistsAsync(req.Slug))
            return BadRequest(new { message = "Slug already exists" });

        var news = new News
        {
            TitleVi = req.TitleVi,
            TitleEn = req.TitleEn,
            Slug = req.Slug,
            ContentVi = req.ContentVi,
            ContentEn = req.ContentEn,
            ImageUrl = req.ImageUrl,
            ExcerptVi = req.ExcerptVi,
            ExcerptEn = req.ExcerptEn,
            IsActive = req.IsActive,
            SortOrder = req.SortOrder,
            Type = Enum.Parse<NewsType>(req.Type),
        };

        var created = await _newsRepo.AddAsync(news);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
    }

    /// <summary>Admin: cap nhat.</summary>
    [Authorize(Roles = "admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<NewsDto>> Update(int id, [FromBody] UpdateNewsRequest req)
    {
        var existing = await _newsRepo.GetByIdWithDetailsAsync(id);
        if (existing == null) return NotFound(new { message = "Not found" });

        if (await _newsRepo.SlugExistsAsync(req.Slug, id))
            return BadRequest(new { message = "Slug already exists" });

        existing.TitleVi = req.TitleVi;
        existing.TitleEn = req.TitleEn;
        existing.Slug = req.Slug;
        existing.ContentVi = req.ContentVi;
        existing.ContentEn = req.ContentEn;
        existing.ImageUrl = req.ImageUrl;
        existing.ExcerptVi = req.ExcerptVi;
        existing.ExcerptEn = req.ExcerptEn;
        existing.IsActive = req.IsActive;
        existing.SortOrder = req.SortOrder;
        existing.Type = Enum.Parse<NewsType>(req.Type);

        await _newsRepo.UpdateAsync(existing);
        return Ok(MapToDto(existing));
    }

    /// <summary>Admin: xoa.</summary>
    [Authorize(Roles = "admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _newsRepo.GetByIdWithDetailsAsync(id);
        if (existing == null) return NotFound(new { message = "Not found" });

        await _newsRepo.DeleteAsync(id);
        return NoContent();
    }

    private static NewsDto MapToDto(News n)
    {
        return new NewsDto
        {
            Id = n.Id,
            TitleVi = n.TitleVi,
            TitleEn = n.TitleEn,
            Slug = n.Slug,
            ContentVi = n.ContentVi,
            ContentEn = n.ContentEn,
            ImageUrl = n.ImageUrl,
            ExcerptVi = n.ExcerptVi,
            ExcerptEn = n.ExcerptEn,
            IsActive = n.IsActive,
            SortOrder = n.SortOrder,
            Type = n.Type.ToString(),
            CreatedAt = n.CreatedAt,
            UpdatedAt = n.UpdatedAt
        };
    }
}
