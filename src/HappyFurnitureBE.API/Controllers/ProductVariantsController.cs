using HappyFurnitureBE.Application.DTOs.Common;
using HappyFurnitureBE.Application.DTOs.Product;
using HappyFurnitureBE.Application.Interfaces;
using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyFurnitureBE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductVariantsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly ILogger<ProductVariantsController> _logger;

    public ProductVariantsController(
        IProductRepository productRepository,
        ICloudinaryService cloudinaryService,
        ILogger<ProductVariantsController> logger)
    {
        _productRepository = productRepository;
        _cloudinaryService = cloudinaryService;
        _logger = logger;
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<PagedResult<ProductVariantDto>>> GetProductVariants(
        int productId,
        [FromQuery] PaginationParams pagination)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            var productWithVariants = await _productRepository.GetProductWithDetailsAsync(productId);
            var variants = productWithVariants?.ProductVariants ?? new List<ProductVariant>();

            var totalCount = variants.Count;
            var pagedVariants = variants
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(MapToProductVariantDto)
                .ToList();

            var result = new PagedResult<ProductVariantDto>
            {
                Items = pagedVariants,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching product variants for product {ProductId}", productId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductVariantDto>> GetProductVariant(int id)
    {
        try
        {
            var variant = await _productRepository.GetProductVariantByIdAsync(id);
            if (variant == null)
            {
                return NotFound(new { message = "Product variant not found" });
            }
            
            return Ok(MapToProductVariantDto(variant));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching product variant {VariantId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ProductVariantDto>> CreateProductVariant([FromBody] CreateProductVariantRequest request)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                return BadRequest(new { message = "Product not found" });
            }

            var productVariant = new ProductVariant
            {
                ProductId = request.ProductId,
                ColorName = request.ColorName,
                ColorCode = request.ColorCode,
                ImageUrl = request.ImageUrl,
                IsActive = request.IsActive
            };

            var createdVariant = await _productRepository.AddProductVariantAsync(productVariant);
            var variantDto = MapToProductVariantDto(createdVariant);
            
            return CreatedAtAction(nameof(GetProductVariant), new { id = createdVariant.Id }, variantDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product variant");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ProductVariantDto>> UpdateProductVariant(int id, [FromBody] UpdateProductVariantRequest request)
    {
        try
        {
            var productVariant = await _productRepository.GetProductVariantByIdAsync(id);
            if (productVariant == null)
            {
                return NotFound(new { message = "Product variant not found" });
            }

            productVariant.ColorName = request.ColorName;
            productVariant.ColorCode = request.ColorCode;
            productVariant.ImageUrl = request.ImageUrl;
            productVariant.IsActive = request.IsActive;

            await _productRepository.UpdateProductVariantAsync(productVariant);
            var variantDto = MapToProductVariantDto(productVariant);
            
            return Ok(variantDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating product variant {VariantId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteProductVariant(int id)
    {
        try
        {
            var productVariant = await _productRepository.GetProductVariantByIdAsync(id);
            if (productVariant == null)
            {
                return NotFound(new { message = "Product variant not found" });
            }

            await _productRepository.DeleteProductVariantAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting product variant {VariantId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("active/product/{productId}")]
    public async Task<ActionResult<IEnumerable<ProductVariantDto>>> GetActiveProductVariants(int productId)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            var variants = await _productRepository.GetActiveProductVariantsAsync(productId);
            var variantDtos = variants.Select(MapToProductVariantDto);
            
            return Ok(variantDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching active product variants for product {ProductId}", productId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Tạo product variant với ảnh - multipart/form-data.
    /// </summary>
    [HttpPost("with-image")]
    [Authorize(Roles = "admin")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<ProductVariantDto>> CreateProductVariantWithImage([FromForm] int productId, [FromForm] string? colorName, [FromForm] string? colorCode, [FromForm] IFormFile? image = null)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return BadRequest(new { message = "Product not found" });
            }

            // Upload image to Cloudinary if provided
            string? imageUrl = null;
            if (image != null)
            {
                try
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(image, "product-variants");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading product variant image to Cloudinary");
                    return BadRequest(new { message = "Error uploading image: " + ex.Message });
                }
            }

            var productVariant = new ProductVariant
            {
                ProductId = productId,
                ColorName = colorName,
                ColorCode = colorCode,
                ImageUrl = imageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdVariant = await _productRepository.AddProductVariantAsync(productVariant);
            var variantDto = MapToProductVariantDto(createdVariant);
            
            return CreatedAtAction(nameof(GetProductVariant), new { id = createdVariant.Id }, variantDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product variant with image");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    private static ProductVariantDto MapToProductVariantDto(ProductVariant productVariant)
    {
        return new ProductVariantDto
        {
            Id = productVariant.Id,
            ProductId = productVariant.ProductId,
            ColorName = productVariant.ColorName,
            ColorCode = productVariant.ColorCode,
            ImageUrl = productVariant.ImageUrl,
            IsActive = productVariant.IsActive,
            CreatedAt = productVariant.CreatedAt,
            UpdatedAt = productVariant.UpdatedAt
        };
    }
}