using HappyFurnitureBE.Application.DTOs.Certificate;
using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyFurnitureBE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CertificatesController : ControllerBase
{
    private readonly ICertificateRepository _certificateRepo;

    public CertificatesController(ICertificateRepository certificateRepo)
    {
        _certificateRepo = certificateRepo;
    }

    /// <summary>
    /// Public: Lấy danh sách Certificate đang hoạt động (cho Client)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CertificateListDto>>> GetActiveCertificates()
    {
        var certificates = await _certificateRepo.GetActiveCertificatesAsync();
        return Ok(certificates.Select(MapToListDto).ToList());
    }

    /// <summary>
    /// Public: Lấy chi tiết Certificate theo id
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CertificateDto>> GetById(int id)
    {
        var certificate = await _certificateRepo.GetByIdAsync(id);
        if (certificate == null) return NotFound(new { message = "Certificate not found" });
        return Ok(MapToDto(certificate));
    }

    /// <summary>
    /// Admin: Danh sách phân trang tất cả Certificate
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpGet("admin/all")]
    public async Task<ActionResult<object>> GetAllAdmin(
        [FromQuery] string? name,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var all = await _certificateRepo.GetAllWithFilterAsync(name, isActive);
        var list = all.ToList();
        var total = list.Count;
        var items = list.Skip((page - 1) * pageSize).Take(pageSize).Select(MapToDto).ToList();

        return Ok(new { items, total, page, pageSize });
    }

    /// <summary>
    /// Admin: Tạo mới Certificate
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<ActionResult<CertificateDto>> Create([FromBody] CreateCertificateRequest req)
    {
        var certificate = new Certificate
        {
            NameVi = req.NameVi,
            NameEn = req.NameEn,
            DescriptionVi = req.DescriptionVi,
            DescriptionEn = req.DescriptionEn,
            LogoUrl = req.LogoUrl,
            IsActive = req.IsActive,
            SortOrder = req.SortOrder,
        };

        var created = await _certificateRepo.AddAsync(certificate);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
    }

    /// <summary>
    /// Admin: Cập nhật Certificate
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CertificateDto>> Update(int id, [FromBody] UpdateCertificateRequest req)
    {
        var existing = await _certificateRepo.GetByIdAsync(id);
        if (existing == null) return NotFound(new { message = "Certificate not found" });

        existing.NameVi = req.NameVi;
        existing.NameEn = req.NameEn;
        existing.DescriptionVi = req.DescriptionVi;
        existing.DescriptionEn = req.DescriptionEn;
        existing.LogoUrl = req.LogoUrl;
        existing.IsActive = req.IsActive;
        existing.SortOrder = req.SortOrder;

        await _certificateRepo.UpdateAsync(existing);
        return Ok(MapToDto(existing));
    }

    /// <summary>
    /// Admin: Xóa Certificate
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _certificateRepo.GetByIdAsync(id);
        if (existing == null) return NotFound(new { message = "Certificate not found" });

        await _certificateRepo.DeleteAsync(id);
        return NoContent();
    }

    private static CertificateDto MapToDto(Certificate c)
    {
        return new CertificateDto
        {
            Id = c.Id,
            NameVi = c.NameVi,
            NameEn = c.NameEn,
            DescriptionVi = c.DescriptionVi,
            DescriptionEn = c.DescriptionEn,
            LogoUrl = c.LogoUrl,
            IsActive = c.IsActive,
            SortOrder = c.SortOrder,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };
    }

    private static CertificateListDto MapToListDto(Certificate c)
    {
        return new CertificateListDto
        {
            Id = c.Id,
            NameVi = c.NameVi,
            NameEn = c.NameEn,
            DescriptionVi = c.DescriptionVi,
            DescriptionEn = c.DescriptionEn,
            LogoUrl = c.LogoUrl,
            IsActive = c.IsActive,
            SortOrder = c.SortOrder,
            CreatedAt = c.CreatedAt
        };
    }
}
