using HappyFurnitureBE.Application.DTOs.Category;
using HappyFurnitureBE.Application.DTOs.Common;
using HappyFurnitureBE.Application.Interfaces;
using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyFurnitureBE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryRepository categoryRepository, ICloudinaryService cloudinaryService, ILogger<CategoriesController> logger)
    {
        _categoryRepository = categoryRepository;
        _cloudinaryService = cloudinaryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CategoryDto>>> GetCategories(
        [FromQuery] PaginationParams pagination,
        [FromQuery] CategoryFilterParams filter)
    {
        try
        {
            var categories = await _categoryRepository.GetAllWithRelationsAsync();
            
            // Apply filters
            var filteredCategories = categories.AsQueryable();
            
            if (!string.IsNullOrEmpty(filter.Name))
            {
                filteredCategories = filteredCategories.Where(c => 
                    c.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
            }
            
            if (filter.ParentId.HasValue)
            {
                filteredCategories = filteredCategories.Where(c => c.ParentId == filter.ParentId);
            }
            
            if (filter.IsActive.HasValue)
            {
                filteredCategories = filteredCategories.Where(c => c.IsActive == filter.IsActive);
            }

            var totalCount = filteredCategories.Count();
            
            var pagedCategories = filteredCategories
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(c => MapToCategoryDto(c, true))
                .ToList();

            var result = new PagedResult<CategoryDto>
            {
                Items = pagedCategories,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching categories");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("root")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetRootCategories()
    {
        try
        {
            var categories = await _categoryRepository.GetRootCategoriesAsync();
            // Sort root categories by SortOrder (null values go last), then by Id
            var categoryDtos = categories
                .OrderBy(c => c.SortOrder ?? int.MaxValue)
                .ThenBy(c => c.Id)
                .Select(c => MapToCategoryDto(c, false));
            return Ok(categoryDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching root categories");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        try
        {
            var category = await _categoryRepository.GetByIdWithRelationsAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }
            
            return Ok(MapToCategoryDto(category, true));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching category {CategoryId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        try
        {
            // Validate parent category exists if provided
            if (request.ParentId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentId.Value);
                if (parentCategory == null)
                {
                    return BadRequest(new { message = "Parent category not found" });
                }
            }

            var category = new Category
            {
                Name = request.Name,
                ImageUrl = request.ImageUrl,
                ParentId = request.ParentId,
                IsActive = request.IsActive,
                // SortOrder chỉ áp dụng cho root category (không có ParentId)
                SortOrder = request.ParentId == null ? request.SortOrder : null
            };

            var createdCategory = await _categoryRepository.AddAsync(category);
            var categoryDto = MapToCategoryDto(createdCategory, false);
            
            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, categoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating category");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            // Validate parent category exists if provided
            if (request.ParentId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentId.Value);
                if (parentCategory == null)
                {
                    return BadRequest(new { message = "Parent category not found" });
                }
                
                // Prevent circular reference
                if (request.ParentId == id)
                {
                    return BadRequest(new { message = "Category cannot be its own parent" });
                }
            }

            category.Name = request.Name;
            category.ImageUrl = request.ImageUrl;
            category.ParentId = request.ParentId;
            category.IsActive = request.IsActive;
            // SortOrder chỉ áp dụng cho root category (không có ParentId)
            category.SortOrder = request.ParentId == null ? request.SortOrder : null;

            await _categoryRepository.UpdateAsync(category);
            var categoryDto = MapToCategoryDto(category, false);
            
            return Ok(categoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating category {CategoryId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            // Check if category has children
            var children = await _categoryRepository.GetChildCategoriesAsync(id);
            if (children.Any())
            {
                return BadRequest(new { message = "Cannot delete category with child categories" });
            }

            await _categoryRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting category {CategoryId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Tạo category với ảnh - dùng multipart/form-data.
    /// Field: name (required), parentId (optional, số), sortOrder (optional, số thứ tự - chỉ dùng cho root category), isActive (optional, true/false), image (optional, file)
    /// </summary>
    [HttpPost("with-image")]
    [Authorize(Roles = "admin")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CategoryDto>> CreateCategoryWithImage([FromForm] string name, [FromForm] int? parentId, [FromForm] int? sortOrder = null, [FromForm] bool isActive = true, [FromForm] IFormFile? image = null)
    {
        try
        {
            // Validate parent category exists if provided
            if (parentId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(parentId.Value);
                if (parentCategory == null)
                {
                    return BadRequest(new { message = "Parent category not found" });
                }
            }

            // Upload image to Cloudinary if provided
            string? imageUrl = null;
            if (image != null)
            {
                try
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(image, "categories");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading category image to Cloudinary");
                    return BadRequest(new { message = "Error uploading image: " + ex.Message });
                }
            }

            var category = new Category
            {
                Name = name,
                ImageUrl = imageUrl,
                ParentId = parentId,
                IsActive = isActive,
                // SortOrder chỉ áp dụng cho root category (không có ParentId)
                SortOrder = parentId == null ? sortOrder : null,
                CreatedAt = DateTime.UtcNow
            };

            var createdCategory = await _categoryRepository.AddAsync(category);
            var categoryDto = MapToCategoryDto(createdCategory, false);
            
            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, categoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating category with image");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    private static CategoryDto MapToCategoryDto(Category category, bool includeRelations = false)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            ImageUrl = category.ImageUrl,
            ParentId = category.ParentId,
            // SortOrder chỉ trả về với root category (ParentId == null)
            SortOrder = category.ParentId == null ? category.SortOrder : null,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
            Parent = includeRelations && category.Parent != null 
                ? MapToCategoryDto(category.Parent, false) 
                : null,
            Children = includeRelations && category.Children != null 
                ? category.Children.Select(c => MapToCategoryDto(c, false)).ToList() 
                : new List<CategoryDto>()
        };
    }
}