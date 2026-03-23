using HappyFurnitureBE.Application.DTOs.Category;
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
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ICloudinaryService cloudinaryService,
        ILogger<ProductsController> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _cloudinaryService = cloudinaryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
        [FromQuery] PaginationParams pagination,
        [FromQuery] ProductFilterParams filter)
    {
        try
        {
            var products = await _productRepository.GetActiveProductsAsync();
            var filteredProducts = products.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.Name))
            {
                filteredProducts = filteredProducts.Where(p => 
                    p.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(filter.Slug))
            {
                filteredProducts = filteredProducts.Where(p => 
                    p.Slug.Contains(filter.Slug, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.MinPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price >= filter.MinPrice);
            }

            if (filter.MaxPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price <= filter.MaxPrice);
            }

            if (filter.CategoryId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => 
                    p.ProductCategories.Any(pc => pc.CategoryId == filter.CategoryId));
            }

            if (filter.IsFeatured.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.IsFeatured == filter.IsFeatured);
            }

            if (filter.IsActive.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.IsActive == filter.IsActive);
            }

            // Apply sorting
            filteredProducts = filter.SortBy?.ToLower() switch
            {
                "name" => filter.SortOrder?.ToLower() == "desc" 
                    ? filteredProducts.OrderByDescending(p => p.Name)
                    : filteredProducts.OrderBy(p => p.Name),
                "price" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredProducts.OrderByDescending(p => p.Price)
                    : filteredProducts.OrderBy(p => p.Price),
                _ => filter.SortOrder?.ToLower() == "asc"
                    ? filteredProducts.OrderBy(p => p.CreatedAt)
                    : filteredProducts.OrderByDescending(p => p.CreatedAt)
            };

            var totalCount = filteredProducts.Count();
            
            var pagedProducts = filteredProducts
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(MapToProductDto)
                .ToList();

            var result = new PagedResult<ProductDto>
            {
                Items = pagedProducts,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("featured")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetFeaturedProducts()
    {
        try
        {
            var products = await _productRepository.GetFeaturedProductsAsync();
            var productDtos = products.Select(MapToProductDto);
            return Ok(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching featured products");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        try
        {
            var product = await _productRepository.GetProductWithDetailsAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }
            
            return Ok(MapToProductDto(product));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching product {ProductId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ProductDto>> GetProductBySlug(string slug)
    {
        try
        {
            var product = await _productRepository.GetBySlugAsync(slug);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }
            
            return Ok(MapToProductDto(product));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching product by slug {Slug}", slug);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            // Check if slug already exists
            if (await _productRepository.SlugExistsAsync(request.Slug))
            {
                return BadRequest(new { message = "Slug already exists" });
            }

            // Validate categories exist
            foreach (var categoryId in request.CategoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return BadRequest(new { message = $"Category with ID {categoryId} not found" });
                }
            }

            var product = new Product
            {
                Name = request.Name,
                Slug = request.Slug,
                Description = request.Description,
                Price = request.Price,
                OldPrice = request.OldPrice,
                DimensionsHeight = request.DimensionsHeight,
                DimensionsWidth = request.DimensionsWidth,
                DimensionsDepth = request.DimensionsDepth,
                DimensionUnit = request.DimensionUnit,
                Detail = request.Detail,
                DeliveryInfo = request.DeliveryInfo,
                Weight = request.Weight,
                IsFeatured = request.IsFeatured,
                IsActive = request.IsActive
            };

            var createdProduct = await _productRepository.AddAsync(product);

            // Add product categories
            if (request.CategoryIds.Any())
            {
                foreach (var categoryId in request.CategoryIds)
                {
                    await _productRepository.AddProductCategoryAsync(new ProductCategory
                    {
                        ProductId = createdProduct.Id,
                        CategoryId = categoryId
                    });
                }
            }

            // Add product images if provided
            if (request.ImageUrls.Any())
            {
                for (int i = 0; i < request.ImageUrls.Count; i++)
                {
                    var productImage = new ProductImage
                    {
                        ProductId = createdProduct.Id,
                        ImageUrl = request.ImageUrls[i],
                        IsPrimary = i == 0, // First image is primary
                        SortOrder = i + 1
                    };
                    
                    await _productRepository.AddProductImageAsync(productImage);
                }
            }

            var productDto = MapToProductDto(createdProduct);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            var product = await _productRepository.GetProductWithDetailsAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // Check if slug already exists for other products
            var existingProduct = await _productRepository.GetBySlugAsync(request.Slug);
            if (existingProduct != null && existingProduct.Id != id)
            {
                return BadRequest(new { message = "Slug already exists" });
            }

            // Validate categories exist
            foreach (var categoryId in request.CategoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return BadRequest(new { message = $"Category with ID {categoryId} not found" });
                }
            }

            // Update product properties
            product.Name = request.Name;
            product.Slug = request.Slug;
            product.Description = request.Description;
            product.Price = request.Price;
            product.OldPrice = request.OldPrice;
            product.DimensionsHeight = request.DimensionsHeight;
            product.DimensionsWidth = request.DimensionsWidth;
            product.DimensionsDepth = request.DimensionsDepth;
            product.DimensionUnit = request.DimensionUnit;
            product.Detail = request.Detail;
            product.DeliveryInfo = request.DeliveryInfo;
            product.Weight = request.Weight;
            product.IsFeatured = request.IsFeatured;
            product.IsActive = request.IsActive;

            await _productRepository.UpdateAsync(product);

            var productDto = MapToProductDto(product);
            return Ok(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating product {ProductId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            await _productRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting product {ProductId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProductsByCategory(
        int categoryId,
        [FromQuery] PaginationParams pagination)
    {
        try
        {
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            
            var totalCount = products.Count();
            var pagedProducts = products
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(MapToProductDto)
                .ToList();

            var result = new PagedResult<ProductDto>
            {
                Items = pagedProducts,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products by category {CategoryId}", categoryId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Tạo product với nhiều ảnh - multipart/form-data.
    /// </summary>
    [HttpPost("with-images")]
    [Authorize(Roles = "admin")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<ProductDto>> CreateProductWithImages(
        [FromForm] string name,
        [FromForm] string slug,
        [FromForm] decimal price,
        [FromForm] string? description,
        [FromForm] decimal? oldPrice,
        [FromForm] decimal? dimensionsHeight,
        [FromForm] decimal? dimensionsWidth,
        [FromForm] decimal? dimensionsDepth,
        [FromForm] string dimensionUnit = "cm",
        [FromForm] string? detail = null,
        [FromForm] string? deliveryInfo = null,
        [FromForm] decimal? weight = null,
        [FromForm] bool isFeatured = false,
        [FromForm] bool isActive = true,
        [FromForm] string? categoryIds = null,
        [FromForm] List<IFormFile>? images = null)
    {
        try
        {
            // Parse category IDs from string (comma-separated)
            var categoryIdList = new List<int>();
            if (!string.IsNullOrEmpty(categoryIds))
            {
                try
                {
                    categoryIdList = categoryIds.Split(',')
                        .Select(id => int.Parse(id.Trim()))
                        .ToList();
                }
                catch (FormatException)
                {
                    return BadRequest(new { message = "Invalid category IDs format. Use comma-separated integers." });
                }
            }

            // Check if slug already exists
            if (await _productRepository.SlugExistsAsync(slug))
            {
                return BadRequest(new { message = "Slug already exists" });
            }

            // Validate categories exist
            foreach (var categoryId in categoryIdList)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return BadRequest(new { message = $"Category with ID {categoryId} not found" });
                }
            }

            // Upload images to Cloudinary if provided
            var imageUrls = new List<string>();
            if (images != null && images.Any())
            {
                try
                {
                    imageUrls = await _cloudinaryService.UploadMultipleImagesAsync(images, "products");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading images to Cloudinary");
                    return BadRequest(new { message = "Error uploading images: " + ex.Message });
                }
            }

            var product = new Product
            {
                Name = name,
                Slug = slug,
                Description = description,
                Price = price,
                OldPrice = oldPrice,
                DimensionsHeight = dimensionsHeight,
                DimensionsWidth = dimensionsWidth,
                DimensionsDepth = dimensionsDepth,
                DimensionUnit = dimensionUnit,
                Detail = detail,
                DeliveryInfo = deliveryInfo,
                Weight = weight,
                IsFeatured = isFeatured,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow
            };

            var createdProduct = await _productRepository.AddAsync(product);

            // Add product categories
            if (categoryIdList.Any())
            {
                foreach (var categoryId in categoryIdList)
                {
                    await _productRepository.AddProductCategoryAsync(new ProductCategory
                    {
                        ProductId = createdProduct.Id,
                        CategoryId = categoryId
                    });
                }
            }

            // Add uploaded images
            if (imageUrls.Any())
            {
                for (int i = 0; i < imageUrls.Count; i++)
                {
                    var productImage = new ProductImage
                    {
                        ProductId = createdProduct.Id,
                        ImageUrl = imageUrls[i],
                        IsPrimary = i == 0, // First image is primary
                        SortOrder = i + 1
                    };
                    
                    await _productRepository.AddProductImageAsync(productImage);
                }
            }

            var productDto = MapToProductDto(createdProduct);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product with images");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    private static ProductDto MapToProductDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            Description = product.Description,
            Price = product.Price,
            OldPrice = product.OldPrice,
            DimensionsHeight = product.DimensionsHeight,
            DimensionsWidth = product.DimensionsWidth,
            DimensionsDepth = product.DimensionsDepth,
            DimensionUnit = product.DimensionUnit,
            Detail = product.Detail,
            DeliveryInfo = product.DeliveryInfo,
            Weight = product.Weight,
            IsFeatured = product.IsFeatured,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            Categories = product.ProductCategories?.Select(pc => new CategoryDto
            {
                Id = pc.Category.Id,
                Name = pc.Category.Name,
                ImageUrl = pc.Category.ImageUrl,
                ParentId = pc.Category.ParentId,
                IsActive = pc.Category.IsActive,
                CreatedAt = pc.Category.CreatedAt,
                UpdatedAt = pc.Category.UpdatedAt
            }).ToList() ?? new List<CategoryDto>(),
            Variants = product.ProductVariants?.Select(pv => new ProductVariantDto
            {
                Id = pv.Id,
                ProductId = pv.ProductId,
                ColorName = pv.ColorName,
                ColorCode = pv.ColorCode,
                ImageUrl = pv.ImageUrl,
                Price = pv.Price,
                IsActive = pv.IsActive,
                CreatedAt = pv.CreatedAt,
                UpdatedAt = pv.UpdatedAt
            }).ToList() ?? new List<ProductVariantDto>(),
            Images = product.ProductImages?.Select(pi => new ProductImageDto
            {
                Id = pi.Id,
                ProductId = pi.ProductId,
                ImageUrl = pi.ImageUrl,
                AltText = pi.AltText,
                IsPrimary = pi.IsPrimary,
                SortOrder = pi.SortOrder,
                CreatedAt = pi.CreatedAt,
                UpdatedAt = pi.UpdatedAt
            }).ToList() ?? new List<ProductImageDto>()
        };
    }
}