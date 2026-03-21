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
public class ProductImagesController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly ILogger<ProductImagesController> _logger;

    public ProductImagesController(
        IProductRepository productRepository,
        ICloudinaryService cloudinaryService,
        ILogger<ProductImagesController> logger)
    {
        _productRepository = productRepository;
        _cloudinaryService = cloudinaryService;
        _logger = logger;
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<PagedResult<ProductImageDto>>> GetProductImages(
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

            var productWithImages = await _productRepository.GetProductWithDetailsAsync(productId);
            var images = productWithImages?.ProductImages ?? new List<ProductImage>();

            var totalCount = images.Count;
            var pagedImages = images
                .OrderBy(pi => pi.SortOrder)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(MapToProductImageDto)
                .ToList();

            var result = new PagedResult<ProductImageDto>
            {
                Items = pagedImages,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching product images for product {ProductId}", productId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductImageDto>> GetProductImage(int id)
    {
        try
        {
            var image = await _productRepository.GetProductImageByIdAsync(id);
            if (image == null)
            {
                return NotFound(new { message = "Product image not found" });
            }
            
            return Ok(MapToProductImageDto(image));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching product image {ImageId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ProductImageDto>> CreateProductImage([FromBody] CreateProductImageRequest request)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                return BadRequest(new { message = "Product not found" });
            }

            // If this is set as primary, unset other primary images
            if (request.IsPrimary)
            {
                await _productRepository.UnsetPrimaryImagesAsync(request.ProductId);
            }

            var productImage = new ProductImage
            {
                ProductId = request.ProductId,
                ImageUrl = request.ImageUrl,
                AltText = request.AltText,
                IsPrimary = request.IsPrimary,
                SortOrder = request.SortOrder
            };

            var createdImage = await _productRepository.AddProductImageAsync(productImage);
            var imageDto = MapToProductImageDto(createdImage);
            
            return CreatedAtAction(nameof(GetProductImage), new { id = createdImage.Id }, imageDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product image");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Tạo product image với ảnh upload - multipart/form-data.
    /// </summary>
    [HttpPost("with-image")]
    [Authorize(Roles = "admin")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<ProductImageDto>> CreateProductImageWithImage(
        [FromForm] int productId,
        [FromForm] IFormFile image,
        [FromForm] bool isPrimary = false,
        [FromForm] int sortOrder = 1,
        [FromForm] string? altText = null)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return BadRequest(new { message = "Product not found" });
            }

            if (image == null || image.Length == 0)
            {
                return BadRequest(new { message = "Image file is required" });
            }

            string imageUrl;
            try
            {
                imageUrl = await _cloudinaryService.UploadImageAsync(image, "product-images");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading product image to Cloudinary");
                return BadRequest(new { message = "Error uploading image: " + ex.Message });
            }

            if (isPrimary)
            {
                await _productRepository.UnsetPrimaryImagesAsync(productId);
            }

            var productImage = new ProductImage
            {
                ProductId = productId,
                ImageUrl = imageUrl,
                AltText = altText,
                IsPrimary = isPrimary,
                SortOrder = sortOrder
            };

            var createdImage = await _productRepository.AddProductImageAsync(productImage);
            return CreatedAtAction(nameof(GetProductImage), new { id = createdImage.Id }, MapToProductImageDto(createdImage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product image with upload");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ProductImageDto>> UpdateProductImage(int id, [FromBody] UpdateProductImageRequest request)
    {
        try
        {
            var productImage = await _productRepository.GetProductImageByIdAsync(id);
            if (productImage == null)
            {
                return NotFound(new { message = "Product image not found" });
            }

            // If this is set as primary, unset other primary images for the same product
            if (request.IsPrimary && !productImage.IsPrimary)
            {
                await _productRepository.UnsetPrimaryImagesAsync(productImage.ProductId);
            }

            productImage.ImageUrl = request.ImageUrl;
            productImage.AltText = request.AltText;
            productImage.IsPrimary = request.IsPrimary;
            productImage.SortOrder = request.SortOrder;

            await _productRepository.UpdateProductImageAsync(productImage);
            var imageDto = MapToProductImageDto(productImage);
            
            return Ok(imageDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating product image {ImageId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteProductImage(int id)
    {
        try
        {
            var productImage = await _productRepository.GetProductImageByIdAsync(id);
            if (productImage == null)
            {
                return NotFound(new { message = "Product image not found" });
            }

            await _productRepository.DeleteProductImageAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting product image {ImageId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("{id}/set-primary")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> SetPrimaryImage(int id)
    {
        try
        {
            var productImage = await _productRepository.GetProductImageByIdAsync(id);
            if (productImage == null)
            {
                return NotFound(new { message = "Product image not found" });
            }

            await _productRepository.SetPrimaryImageAsync(id);
            return Ok(new { message = "Primary image set successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while setting primary image {ImageId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    private static ProductImageDto MapToProductImageDto(ProductImage productImage)
    {
        return new ProductImageDto
        {
            Id = productImage.Id,
            ProductId = productImage.ProductId,
            ImageUrl = productImage.ImageUrl,
            AltText = productImage.AltText,
            IsPrimary = productImage.IsPrimary,
            SortOrder = productImage.SortOrder,
            CreatedAt = productImage.CreatedAt,
            UpdatedAt = productImage.UpdatedAt
        };
    }
}