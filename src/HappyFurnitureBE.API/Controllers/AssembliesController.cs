using HappyFurnitureBE.Application.DTOs.Assembly;
using HappyFurnitureBE.Application.DTOs.Common;
using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyFurnitureBE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssembliesController : ControllerBase
{
    private readonly IAssemblyRepository _assemblyRepository;
    private readonly ILogger<AssembliesController> _logger;

    public AssembliesController(IAssemblyRepository assemblyRepository, ILogger<AssembliesController> logger)
    {
        _assemblyRepository = assemblyRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<AssemblyDto>>> GetAssemblies(
        [FromQuery] PaginationParams pagination,
        [FromQuery] AssemblyFilterParams filter)
    {
        try
        {
            var assemblies = await _assemblyRepository.GetAllAsync();
            var filtered = assemblies.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                filtered = filtered.Where(a =>
                    a.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.IsActive.HasValue)
            {
                filtered = filtered.Where(a => a.IsActive == filter.IsActive);
            }

            var totalCount = filtered.Count();

            var paged = filtered
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(MapToDto)
                .ToList();

            return Ok(new PagedResult<AssemblyDto>
            {
                Items = paged,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching assemblies");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<AssemblyDto>>> GetActiveAssemblies()
    {
        try
        {
            var assemblies = await _assemblyRepository.GetActiveAssembliesAsync();
            return Ok(assemblies.Select(MapToDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching active assemblies");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AssemblyDto>> GetAssembly(int id)
    {
        try
        {
            var assembly = await _assemblyRepository.GetByIdAsync(id);
            if (assembly == null)
                return NotFound(new { message = "Assembly not found" });

            return Ok(MapToDto(assembly));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching assembly {Id}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<AssemblyDto>> CreateAssembly([FromBody] CreateAssemblyRequest request)
    {
        try
        {
            var assembly = new Assembly
            {
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                IsActive = request.IsActive
            };

            var created = await _assemblyRepository.AddAsync(assembly);
            return CreatedAtAction(nameof(GetAssembly), new { id = created.Id }, MapToDto(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating assembly");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<AssemblyDto>> UpdateAssembly(int id, [FromBody] UpdateAssemblyRequest request)
    {
        try
        {
            var assembly = await _assemblyRepository.GetByIdAsync(id);
            if (assembly == null)
                return NotFound(new { message = "Assembly not found" });

            assembly.Name = request.Name;
            assembly.Code = request.Code;
            assembly.Description = request.Description;
            assembly.IsActive = request.IsActive;

            await _assemblyRepository.UpdateAsync(assembly);
            return Ok(MapToDto(assembly));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assembly {Id}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteAssembly(int id)
    {
        try
        {
            var assembly = await _assemblyRepository.GetByIdAsync(id);
            if (assembly == null)
                return NotFound(new { message = "Assembly not found" });

            await _assemblyRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting assembly {Id}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    private static AssemblyDto MapToDto(Assembly assembly) => new()
    {
        Id = assembly.Id,
        Name = assembly.Name,
        Code = assembly.Code,
        Description = assembly.Description,
        IsActive = assembly.IsActive,
        CreatedAt = assembly.CreatedAt,
        UpdatedAt = assembly.UpdatedAt
    };
}
