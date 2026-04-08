using HappyFurnitureBE.Application.DTOs.Common;
using HappyFurnitureBE.Application.DTOs.Material;
using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyFurnitureBE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaterialsController : ControllerBase
{
    private readonly IMaterialRepository _materialRepository;
    private readonly ILogger<MaterialsController> _logger;

    public MaterialsController(IMaterialRepository materialRepository, ILogger<MaterialsController> logger)
    {
        _materialRepository = materialRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<MaterialDto>>> GetMaterials(
        [FromQuery] PaginationParams pagination,
        [FromQuery] MaterialFilterParams filter)
    {
        try
        {
            var materials = await _materialRepository.GetAllAsync();
            
            var filteredMaterials = materials.AsQueryable();
            
            if (!string.IsNullOrEmpty(filter.Name))
            {
                filteredMaterials = filteredMaterials.Where(m => 
                    m.NameVi.Contains(filter.Name, StringComparison.OrdinalIgnoreCase) ||
                    m.NameEn.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
            }
            
            if (filter.IsActive.HasValue)
            {
                filteredMaterials = filteredMaterials.Where(m => m.IsActive == filter.IsActive);
            }

            var totalCount = filteredMaterials.Count();
            
            var pagedMaterials = filteredMaterials
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(MapToMaterialDto)
                .ToList();

            var result = new PagedResult<MaterialDto>
            {
                Items = pagedMaterials,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching materials");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetActiveMaterials()
    {
        try
        {
            var materials = await _materialRepository.GetActiveMaterialsAsync();
            var materialDtos = materials.Select(MapToMaterialDto);
            return Ok(materialDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching active materials");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MaterialDto>> GetMaterial(int id)
    {
        try
        {
            var material = await _materialRepository.GetByIdAsync(id);
            if (material == null)
            {
                return NotFound(new { message = "Material not found" });
            }
            
            return Ok(MapToMaterialDto(material));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching material {MaterialId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<MaterialDto>> CreateMaterial([FromBody] CreateMaterialRequest request)
    {
        try
        {
            var material = new Material
            {
                NameVi = request.NameVi,
                NameEn = request.NameEn,
                DescriptionVi = request.DescriptionVi,
                DescriptionEn = request.DescriptionEn,
                IsActive = request.IsActive
            };

            var createdMaterial = await _materialRepository.AddAsync(material);
            var materialDto = MapToMaterialDto(createdMaterial);
            
            return CreatedAtAction(nameof(GetMaterial), new { id = createdMaterial.Id }, materialDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating material");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<MaterialDto>> UpdateMaterial(int id, [FromBody] UpdateMaterialRequest request)
    {
        try
        {
            var material = await _materialRepository.GetByIdAsync(id);
            if (material == null)
            {
                return NotFound(new { message = "Material not found" });
            }

            material.NameVi = request.NameVi;
            material.NameEn = request.NameEn;
            material.DescriptionVi = request.DescriptionVi;
            material.DescriptionEn = request.DescriptionEn;
            material.IsActive = request.IsActive;

            await _materialRepository.UpdateAsync(material);
            var materialDto = MapToMaterialDto(material);
            
            return Ok(materialDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating material {MaterialId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteMaterial(int id)
    {
        try
        {
            var material = await _materialRepository.GetByIdAsync(id);
            if (material == null)
            {
                return NotFound(new { message = "Material not found" });
            }

            await _materialRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting material {MaterialId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    private static MaterialDto MapToMaterialDto(Material material)
    {
        return new MaterialDto
        {
            Id = material.Id,
            NameVi = material.NameVi,
            NameEn = material.NameEn,
            DescriptionVi = material.DescriptionVi,
            DescriptionEn = material.DescriptionEn,
            IsActive = material.IsActive,
            CreatedAt = material.CreatedAt,
            UpdatedAt = material.UpdatedAt
        };
    }
}
