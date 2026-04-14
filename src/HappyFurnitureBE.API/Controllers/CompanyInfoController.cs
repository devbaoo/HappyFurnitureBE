using HappyFurnitureBE.Application.DTOs.CompanyInfo;
using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyFurnitureBE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyInfoController : ControllerBase
{
    private readonly ICompanyInfoRepository _repo;

    public CompanyInfoController(ICompanyInfoRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Public: Lấy danh sách CompanyInfo đang hoạt động (cho Client)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CompanyInfoListDto>>> GetActive()
    {
        var items = await _repo.GetActiveAsync();
        return Ok(items.Select(MapToListDto).ToList());
    }

    /// <summary>
    /// Public: Lấy chi tiết theo id
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CompanyInfoDto>> GetById(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return NotFound(new { message = "Not found" });
        return Ok(MapToDto(item));
    }

    /// <summary>
    /// Admin: Danh sách phân trang
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpGet("admin/all")]
    public async Task<ActionResult<object>> GetAllAdmin(
        [FromQuery] string? name,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var all = await _repo.GetAllWithFilterAsync(name, isActive);
        var list = all.ToList();
        var total = list.Count;
        var items = list.Skip((page - 1) * pageSize).Take(pageSize).Select(MapToDto).ToList();

        return Ok(new { items, total, page, pageSize });
    }

    /// <summary>
    /// Admin: Tạo mới
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<ActionResult<CompanyInfoDto>> Create([FromBody] CreateCompanyInfoRequest req)
    {
        var entity = new CompanyInfo
        {
            NameVi = req.NameVi,
            NameEn = req.NameEn,
            Email = req.Email,
            PhoneVi = req.PhoneVi,
            PhoneEn = req.PhoneEn,
            FaxVi = req.FaxVi,
            FaxEn = req.FaxEn,
            IsActive = req.IsActive
        };

        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
    }

    /// <summary>
    /// Admin: Cập nhật
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CompanyInfoDto>> Update(int id, [FromBody] UpdateCompanyInfoRequest req)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound(new { message = "Not found" });

        existing.NameVi = req.NameVi;
        existing.NameEn = req.NameEn;
        existing.Email = req.Email;
        existing.PhoneVi = req.PhoneVi;
        existing.PhoneEn = req.PhoneEn;
        existing.FaxVi = req.FaxVi;
        existing.FaxEn = req.FaxEn;
        existing.IsActive = req.IsActive;

        await _repo.UpdateAsync(existing);
        return Ok(MapToDto(existing));
    }

    /// <summary>
    /// Admin: Xóa
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound(new { message = "Not found" });

        await _repo.DeleteAsync(id);
        return NoContent();
    }

    private static CompanyInfoDto MapToDto(CompanyInfo c)
    {
        return new CompanyInfoDto
        {
            Id = c.Id,
            NameVi = c.NameVi,
            NameEn = c.NameEn,
            Email = c.Email,
            PhoneVi = c.PhoneVi,
            PhoneEn = c.PhoneEn,
            FaxVi = c.FaxVi,
            FaxEn = c.FaxEn,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };
    }

    private static CompanyInfoListDto MapToListDto(CompanyInfo c)
    {
        return new CompanyInfoListDto
        {
            Id = c.Id,
            NameVi = c.NameVi,
            NameEn = c.NameEn,
            Email = c.Email,
            PhoneVi = c.PhoneVi,
            PhoneEn = c.PhoneEn,
            FaxVi = c.FaxVi,
            FaxEn = c.FaxEn,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        };
    }
}
