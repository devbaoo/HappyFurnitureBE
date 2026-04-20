using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
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

    // ═══════════════════════════════════════════════════════════════════════════
    // Product Variant CRUD
    // ═══════════════════════════════════════════════════════════════════════════

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
                .Select(variant => MapToProductVariantDto(variant, product.Slug))
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
            
            var product = await _productRepository.GetByIdAsync(variant.ProductId);
            return Ok(MapToProductVariantDto(variant, product?.Slug));
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
                Slug = NormalizeVariantSlug(request.Slug, request.ColorName),
                ColorNameEn = request.ColorNameEn,
                ColorCode = request.ColorCode,
                ImageUrl = request.ImageUrl,
                IsActive = request.IsActive
            };

            var createdVariant = await _productRepository.AddProductVariantAsync(productVariant);
            var variantDto = MapToProductVariantDto(createdVariant, product.Slug);

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

            var product = await _productRepository.GetByIdAsync(productVariant.ProductId);
            productVariant.ColorName = request.ColorName;
            productVariant.Slug = string.IsNullOrWhiteSpace(request.Slug)
                ? productVariant.Slug
                : NormalizeVariantSlug(request.Slug, null);
            productVariant.ColorNameEn = request.ColorNameEn;
            productVariant.ColorCode = request.ColorCode;
            productVariant.ImageUrl = request.ImageUrl;
            productVariant.IsActive = request.IsActive;

            await _productRepository.UpdateProductVariantAsync(productVariant);
            var variantDto = MapToProductVariantDto(productVariant, product?.Slug);
            
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
            var variantDtos = variants.Select(variant => MapToProductVariantDto(variant, product.Slug));
            
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
    public async Task<ActionResult<ProductVariantDto>> CreateProductVariantWithImage([FromForm] CreateProductVariantWithImageRequest request, [FromForm] IFormFile? image = null)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId);
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
                ProductId = request.ProductId,
                ColorName = request.ColorName,
                ColorNameEn = request.ColorNameEn,
                Slug = NormalizeVariantSlug(request.Slug, request.ColorName),
                ColorCode = request.ColorCode,
                ImageUrl = imageUrl,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            var createdVariant = await _productRepository.AddProductVariantAsync(productVariant);
            var variantDto = MapToProductVariantDto(createdVariant, product.Slug);
            
            return CreatedAtAction(nameof(GetProductVariant), new { id = createdVariant.Id }, variantDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product variant with image");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Product Variant Images CRUD
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>Lấy bộ ảnh của một biến thể</summary>
    [HttpGet("{variantId}/images")]
    public async Task<ActionResult<IEnumerable<ProductVariantImageDto>>> GetVariantImages(int variantId)
    {
        try
        {
            var variant = await _productRepository.GetProductVariantByIdAsync(variantId);
            if (variant == null)
                return NotFound(new { message = "Product variant not found" });

            var images = await _productRepository.GetVariantImagesAsync(variantId);
            return Ok(images.Select(MapToVariantImageDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching variant images for variant {VariantId}", variantId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>Thêm ảnh cho biến thể (JSON)</summary>
    [HttpPost("{variantId}/images")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ProductVariantImageDto>> CreateVariantImage(
        int variantId, [FromBody] CreateProductVariantImageRequest request)
    {
        try
        {
            var variant = await _productRepository.GetProductVariantByIdAsync(variantId);
            if (variant == null)
                return BadRequest(new { message = "Product variant not found" });

            // Override variantId from route
            if (request.IsPrimary)
            {
                await _productRepository.UnsetPrimaryVariantImagesAsync(variantId);
            }

            var image = new ProductVariantImage
            {
                VariantId = variantId,
                ImageUrl = request.ImageUrl,
                AltText = request.AltText,
                IsPrimary = request.IsPrimary,
                SortOrder = request.SortOrder
            };

            var created = await _productRepository.AddProductVariantImageAsync(image);
            return CreatedAtAction(nameof(GetVariantImages), new { variantId }, MapToVariantImageDto(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating variant image for variant {VariantId}", variantId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>Thêm ảnh cho biến thể bằng file upload</summary>
    [HttpPost("{variantId}/images/with-image")]
    [Authorize(Roles = "admin")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<ProductVariantImageDto>> CreateVariantImageWithUpload(
        int variantId,
        [FromForm] IFormFile image,
        [FromForm] bool isPrimary = false,
        [FromForm] int sortOrder = 1,
        [FromForm] string? altText = null)
    {
        try
        {
            var variant = await _productRepository.GetProductVariantByIdAsync(variantId);
            if (variant == null)
                return BadRequest(new { message = "Product variant not found" });

            if (image == null || image.Length == 0)
                return BadRequest(new { message = "Image file is required" });

            string imageUrl;
            try
            {
                imageUrl = await _cloudinaryService.UploadImageAsync(image, "product-variant-images");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading variant image to Cloudinary");
                return BadRequest(new { message = "Error uploading image: " + ex.Message });
            }

            if (isPrimary)
            {
                await _productRepository.UnsetPrimaryVariantImagesAsync(variantId);
            }

            var variantImage = new ProductVariantImage
            {
                VariantId = variantId,
                ImageUrl = imageUrl,
                AltText = altText,
                IsPrimary = isPrimary,
                SortOrder = sortOrder
            };

            var created = await _productRepository.AddProductVariantImageAsync(variantImage);
            return CreatedAtAction(nameof(GetVariantImages), new { variantId }, MapToVariantImageDto(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating variant image with upload for variant {VariantId}", variantId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>Cập nhật ảnh biến thể bằng file upload (multipart)</summary>
    [HttpPut("images/{imageId}/with-image")]
    [Authorize(Roles = "admin")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<ProductVariantImageDto>> UpdateVariantImageWithUpload(
        int imageId,
        [FromForm] IFormFile? image = null,
        [FromForm] string? imageUrl = null,
        [FromForm] bool isPrimary = false,
        [FromForm] int sortOrder = 1,
        [FromForm] string? altText = null)
    {
        try
        {
            var existingImage = await _productRepository.GetProductVariantImageByIdAsync(imageId);
            if (existingImage == null)
                return NotFound(new { message = "Variant image not found" });

            if (isPrimary && !existingImage.IsPrimary)
            {
                await _productRepository.UnsetPrimaryVariantImagesAsync(existingImage.VariantId);
            }

            // Upload new image to Cloudinary if provided; otherwise use imageUrl from form
            if (image != null && image.Length > 0)
            {
                try
                {
                    existingImage.ImageUrl = await _cloudinaryService.UploadImageAsync(image, "product-variant-images");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading variant image to Cloudinary for update");
                    return BadRequest(new { message = "Error uploading image: " + ex.Message });
                }
            }
            else if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                existingImage.ImageUrl = imageUrl;
            }

            existingImage.AltText = altText;
            existingImage.IsPrimary = isPrimary;
            existingImage.SortOrder = sortOrder;

            await _productRepository.UpdateProductVariantImageAsync(existingImage);
            return Ok(MapToVariantImageDto(existingImage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating variant image with upload {ImageId}", imageId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>Cập nhật ảnh biến thể</summary>
    [HttpPut("images/{imageId}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ProductVariantImageDto>> UpdateVariantImage(
        int imageId, [FromBody] UpdateProductVariantImageRequest request)
    {
        try
        {
            var image = await _productRepository.GetProductVariantImageByIdAsync(imageId);
            if (image == null)
                return NotFound(new { message = "Variant image not found" });

            if (request.IsPrimary && !image.IsPrimary)
            {
                await _productRepository.UnsetPrimaryVariantImagesAsync(image.VariantId);
            }

            image.ImageUrl = request.ImageUrl;
            image.AltText = request.AltText;
            image.IsPrimary = request.IsPrimary;
            image.SortOrder = request.SortOrder;

            await _productRepository.UpdateProductVariantImageAsync(image);
            return Ok(MapToVariantImageDto(image));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating variant image {ImageId}", imageId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>Xóa ảnh biến thể</summary>
    [HttpDelete("images/{imageId}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteVariantImage(int imageId)
    {
        try
        {
            var image = await _productRepository.GetProductVariantImageByIdAsync(imageId);
            if (image == null)
                return NotFound(new { message = "Variant image not found" });

            await _productRepository.DeleteProductVariantImageAsync(imageId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting variant image {ImageId}", imageId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>Đặt ảnh chính cho biến thể</summary>
    [HttpPost("images/{imageId}/set-primary")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> SetPrimaryVariantImage(int imageId)
    {
        try
        {
            var image = await _productRepository.GetProductVariantImageByIdAsync(imageId);
            if (image == null)
                return NotFound(new { message = "Variant image not found" });

            await _productRepository.SetPrimaryVariantImageAsync(imageId);
            return Ok(new { message = "Primary variant image set successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting primary variant image {ImageId}", imageId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Mapping helpers
    // ═══════════════════════════════════════════════════════════════════════════

    private static ProductVariantDto MapToProductVariantDto(ProductVariant productVariant, string? productSlug = null)
    {
        var mappedImages = productVariant.ProductVariantImages?.Select(MapToVariantImageDto).ToList()
            ?? new List<ProductVariantImageDto>();

        if (mappedImages.Count == 0 && !string.IsNullOrWhiteSpace(productVariant.ImageUrl))
        {
            mappedImages.Add(new ProductVariantImageDto
            {
                Id = 0,
                VariantId = productVariant.Id,
                ImageUrl = productVariant.ImageUrl,
                AltText = productVariant.ColorName,
                IsPrimary = true,
                SortOrder = 0,
                CreatedAt = productVariant.CreatedAt,
                UpdatedAt = productVariant.UpdatedAt
            });
        }

        return new ProductVariantDto
        {
            Id = productVariant.Id,
            ProductId = productVariant.ProductId,
            ColorName = productVariant.ColorName,
            ColorNameEn = productVariant.ColorNameEn,
            Slug = productVariant.Slug,
            FullSlug = productVariant.IsDefault ? productSlug : BuildVariantFullSlug(productSlug, productVariant.Slug),
            ColorCode = productVariant.ColorCode,
            ImageUrl = productVariant.ImageUrl,
            IsActive = productVariant.IsActive,
            IsDefault = productVariant.IsDefault,
            CreatedAt = productVariant.CreatedAt,
            UpdatedAt = productVariant.UpdatedAt,
            Images = mappedImages
        };
    }

    private static string? NormalizeVariantSlug(string? variantSlug, string? colorName = null)
    {
        var trimmed = variantSlug?.Trim();
        // Always auto-generate from colorName when slug is empty/missing
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            var generated = GenerateSlug(colorName);
            return string.IsNullOrWhiteSpace(generated) ? null : generated;
        }
        return trimmed;
    }

    private static string? BuildVariantFullSlug(string? productSlug, string? variantSlug)
    {
        if (string.IsNullOrWhiteSpace(variantSlug))
            return null;

        var trimmedVariantSlug = variantSlug.Trim();
        if (string.IsNullOrWhiteSpace(productSlug))
            return trimmedVariantSlug;

        var lastDash = productSlug.LastIndexOf('-');
        if (lastDash < 0)
            return trimmedVariantSlug;

        return productSlug[..lastDash] + "-" + trimmedVariantSlug;
    }

    private static string GenerateSlug(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return "";

        // Lowercase + normalize diacritics
        var normalized = value.ToLowerInvariant()
            .Replace("đ", "d")
            .Replace("Đ", "d");

        normalized = normalized.Replace("\u0111", "d");
        normalized = normalized.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        var slug = sb.ToString().Normalize(NormalizationForm.FormC);
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = slug.Trim('-');
        return slug;
    }

    private static ProductVariantImageDto MapToVariantImageDto(ProductVariantImage image)
    {
        return new ProductVariantImageDto
        {
            Id = image.Id,
            VariantId = image.VariantId,
            ImageUrl = image.ImageUrl,
            AltText = image.AltText,
            IsPrimary = image.IsPrimary,
            SortOrder = image.SortOrder,
            CreatedAt = image.CreatedAt,
            UpdatedAt = image.UpdatedAt
        };
    }
}
