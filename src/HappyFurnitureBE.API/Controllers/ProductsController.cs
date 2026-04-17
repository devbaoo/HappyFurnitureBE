using HappyFurnitureBE.Application.DTOs.Assembly;
using HappyFurnitureBE.Application.DTOs.Category;
using HappyFurnitureBE.Application.DTOs.Common;
using HappyFurnitureBE.Application.DTOs.Material;
using HappyFurnitureBE.Application.DTOs.Product;
using HappyFurnitureBE.Application.Interfaces;
using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace HappyFurnitureBE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMaterialRepository _materialRepository;
    private readonly IAssemblyRepository _assemblyRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMaterialRepository materialRepository,
        IAssemblyRepository assemblyRepository,
        ICloudinaryService cloudinaryService,
        ILogger<ProductsController> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _materialRepository = materialRepository;
        _assemblyRepository = assemblyRepository;
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

            if (filter.CategoryId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => 
                    p.ProductCategories.Any(pc => pc.CategoryId == filter.CategoryId));
            }

            if (filter.MaterialId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => 
                    p.ProductMaterials.Any(pm => pm.MaterialId == filter.MaterialId));
            }

            if (filter.IsFeatured.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.IsFeatured == filter.IsFeatured);
            }

            if (filter.IsActive.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.IsActive == filter.IsActive);
            }

            if (filter.AssemblyId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.AssemblyId == filter.AssemblyId);
            }

            // Apply sorting
            filteredProducts = filter.SortBy?.ToLower() switch
            {
                "name" => filter.SortOrder?.ToLower() == "desc" 
                    ? filteredProducts.OrderByDescending(p => p.Name)
                    : filteredProducts.OrderBy(p => p.Name),
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

    /// <summary>
    /// Resolve full slug có thể chứa variant slug ở cuối.
    /// VD: "ban-phong-khach-2-mau-trang" → product slug "ban-phong-khach-2", variantSlug "mau-trang"
    /// </summary>
    [HttpGet("resolve-slug/{**fullSlug}")]
    public async Task<ActionResult> ResolveSlug(string fullSlug)
    {
        try
        {
            // 1. Exact product slug match
            var product = await _productRepository.GetBySlugAsync(fullSlug);
            if (product != null)
                return Ok(new ResolveSlugResponse
                {
                    Product = MapToProductDto(product),
                    VariantSlug = null
                });

            // 2. Variant slug direct match
            var productByVariant = await _productRepository.GetByVariantSlugAsync(fullSlug);
            if (productByVariant != null)
                return Ok(new ResolveSlugResponse
                {
                    Product = MapToProductDto(productByVariant),
                    VariantSlug = fullSlug
                });

            // 3. Progressive strip fallback (legacy slug format)
            var parts = fullSlug.Split('-');
            for (int i = parts.Length - 1; i > 0; i--)
            {
                var candidateSlug = string.Join("-", parts.Take(i));
                var candidateVariantSlug = string.Join("-", parts.Skip(i));
                var candidate = await _productRepository.GetBySlugAsync(candidateSlug);
                if (candidate != null)
                    return Ok(new ResolveSlugResponse
                    {
                        Product = MapToProductDto(candidate),
                        VariantSlug = candidateVariantSlug
                    });
            }

            return NotFound(new { message = "Product not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving slug {FullSlug}", fullSlug);
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

            // Validate materials exist
            foreach (var materialId in request.MaterialIds)
            {
                var material = await _materialRepository.GetByIdAsync(materialId);
                if (material == null)
                {
                    return BadRequest(new { message = $"Material with ID {materialId} not found" });
                }
            }

            // Validate assembly exists
            if (request.AssemblyId.HasValue)
            {
                var assembly = await _assemblyRepository.GetByIdAsync(request.AssemblyId.Value);
                if (assembly == null)
                    return BadRequest(new { message = $"Assembly with ID {request.AssemblyId} not found" });
            }

            var product = new Product
            {
                Name = request.Name,
                NameEn = request.NameEn,
                Slug = request.Slug,
                Description = request.Description,
                DescriptionEn = request.DescriptionEn,
                DimensionsHeight = request.DimensionsHeight,
                DimensionsWidth = request.DimensionsWidth,
                DimensionsDepth = request.DimensionsDepth,
                DimensionUnit = request.DimensionUnit,
                Detail = request.Detail,
                DetailEn = request.DetailEn,
                DeliveryInfo = request.DeliveryInfo,
                DeliveryInfoEn = request.DeliveryInfoEn,
                Weight = request.Weight,
                DeliveryHeight = request.DeliveryHeight,
                DeliveryWidth = request.DeliveryWidth,
                DeliveryDepth = request.DeliveryDepth,
                IsFeatured = request.IsFeatured,
                IsActive = request.IsActive,
                AssemblyId = request.AssemblyId
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

            // Add product materials
            if (request.MaterialIds.Any())
            {
                foreach (var materialId in request.MaterialIds)
                {
                    await _productRepository.AddProductMaterialAsync(new ProductMaterial
                    {
                        ProductId = createdProduct.Id,
                        MaterialId = materialId
                    });
                }
            }

            // Add product images if provided
            string? firstImageUrl = null;
            if (request.ImageUrls.Any())
            {
                for (int i = 0; i < request.ImageUrls.Count; i++)
                {
                    var productImage = new ProductImage
                    {
                        ProductId = createdProduct.Id,
                        ImageUrl = request.ImageUrls[i],
                        IsPrimary = i == 0,
                        SortOrder = i + 1
                    };
                    if (i == 0) firstImageUrl = request.ImageUrls[i];
                    await _productRepository.AddProductImageAsync(productImage);
                }
            }

            // Auto-create default variant
            var defaultVariant = new ProductVariant
            {
                ProductId = createdProduct.Id,
                ColorName = !string.IsNullOrWhiteSpace(request.DefaultVariantColorName)
                    ? request.DefaultVariantColorName : "Mặc định",
                ColorNameEn = !string.IsNullOrWhiteSpace(request.DefaultVariantColorNameEn)
                    ? request.DefaultVariantColorNameEn : "Default",
                ColorCode = !string.IsNullOrWhiteSpace(request.DefaultVariantColorCode)
                    ? request.DefaultVariantColorCode : "#FFFFFF",
                Slug = ResolveDefaultVariantSlug(request.DefaultVariantSlug, createdProduct.Slug),
                ImageUrl = !string.IsNullOrWhiteSpace(request.DefaultVariantImageUrl)
                    ? request.DefaultVariantImageUrl : firstImageUrl,
                IsActive = true
            };
            await _productRepository.AddProductVariantAsync(defaultVariant);

            var createdProductWithDetails = await _productRepository.GetProductWithDetailsAsync(createdProduct.Id);
            var productDto = MapToProductDto(createdProductWithDetails ?? createdProduct);
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

            // Validate materials exist
            foreach (var materialId in request.MaterialIds)
            {
                var material = await _materialRepository.GetByIdAsync(materialId);
                if (material == null)
                {
                    return BadRequest(new { message = $"Material with ID {materialId} not found" });
                }
            }

            // Validate assembly exists
            if (request.AssemblyId.HasValue)
            {
                var assembly = await _assemblyRepository.GetByIdAsync(request.AssemblyId.Value);
                if (assembly == null)
                    return BadRequest(new { message = $"Assembly with ID {request.AssemblyId} not found" });
            }

            // Update product properties
            product.Name = request.Name;
            product.NameEn = request.NameEn;
            product.Slug = request.Slug;
            product.Description = request.Description;
            product.DescriptionEn = request.DescriptionEn;
            product.DimensionsHeight = request.DimensionsHeight;
            product.DimensionsWidth = request.DimensionsWidth;
            product.DimensionsDepth = request.DimensionsDepth;
            product.DimensionUnit = request.DimensionUnit;
            product.Detail = request.Detail;
            product.DetailEn = request.DetailEn;
            product.DeliveryInfo = request.DeliveryInfo;
            product.DeliveryInfoEn = request.DeliveryInfoEn;
            product.Weight = request.Weight;
            product.DeliveryHeight = request.DeliveryHeight;
            product.DeliveryWidth = request.DeliveryWidth;
            product.DeliveryDepth = request.DeliveryDepth;
            product.IsFeatured = request.IsFeatured;
            product.IsActive = request.IsActive;
            product.AssemblyId = request.AssemblyId;

            await _productRepository.UpdateAsync(product);

            // Update categories: remove all existing then re-add
            await _productRepository.DeleteProductCategoriesAsync(id);
            foreach (var categoryId in request.CategoryIds)
            {
                await _productRepository.AddProductCategoryAsync(new ProductCategory
                {
                    ProductId = id,
                    CategoryId = categoryId
                });
            }

            // Update materials: remove all existing then re-add
            await _productRepository.DeleteProductMaterialsAsync(id);
            foreach (var materialId in request.MaterialIds)
            {
                await _productRepository.AddProductMaterialAsync(new ProductMaterial
                {
                    ProductId = id,
                    MaterialId = materialId
                });
            }

            if (request.ImageUrls != null)
            {
                // Xóa tất cả ảnh product rồi thêm mới
                await _productRepository.DeleteProductImagesAsync(id);

                var normalizedImageUrls = request.ImageUrls
                    .Where(url => !string.IsNullOrWhiteSpace(url))
                    .Select(url => url.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                for (int i = 0; i < normalizedImageUrls.Count; i++)
                {
                    await _productRepository.AddProductImageAsync(new ProductImage
                    {
                        ProductId = id,
                        ImageUrl = normalizedImageUrls[i],
                        IsPrimary = i == 0,
                        SortOrder = i + 1
                    });
                }
            }

            var updatedProductWithDetails = await _productRepository.GetProductWithDetailsAsync(product.Id);
            var productDto = MapToProductDto(updatedProductWithDetails ?? product);
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
        [FromForm] string? nameEn = null,
        [FromForm] string slug = "",
        [FromForm] string? description = null,
        [FromForm] string? descriptionEn = null,
        [FromForm] string? dimensionsHeight = null,
        [FromForm] string? dimensionsWidth = null,
        [FromForm] string? dimensionsDepth = null,
        [FromForm] string dimensionUnit = "cm",
        [FromForm] string? detail = null,
        [FromForm] string? detailEn = null,
        [FromForm] string? deliveryInfo = null,
        [FromForm] string? deliveryInfoEn = null,
        [FromForm] string? weight = null,
        [FromForm] string? deliveryHeight = null,
        [FromForm] string? deliveryWidth = null,
        [FromForm] string? deliveryDepth = null,
        [FromForm] bool isFeatured = false,
        [FromForm] bool isActive = true,
        [FromForm] int? assemblyId = null,
        [FromForm] string? categoryIds = null,
        [FromForm] List<IFormFile>? images = null,
        [FromForm] string? defaultVariantColorName = "Mặc định",
        [FromForm] string? defaultVariantColorNameEn = "Default",
        [FromForm] string? defaultVariantColorCode = "#FFFFFF",
        [FromForm] string? defaultVariantSlug = null,
        [FromForm] string? defaultVariantImageUrl = null,
        [FromForm] IFormFile? defaultVariantImage = null)
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

            // Parse material IDs from string (comma-separated) if provided
            var materialIdList = new List<int>();
            // Note: materialIds parameter would need to be added to the method signature if needed

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
                NameEn = nameEn,
                Slug = slug,
                Description = description,
                DescriptionEn = descriptionEn,
                DimensionsHeight = dimensionsHeight,
                DimensionsWidth = dimensionsWidth,
                DimensionsDepth = dimensionsDepth,
                DimensionUnit = dimensionUnit,
                Detail = detail,
                DetailEn = detailEn,
                DeliveryInfo = deliveryInfo,
                DeliveryInfoEn = deliveryInfoEn,
                Weight = weight,
                DeliveryHeight = deliveryHeight,
                DeliveryWidth = deliveryWidth,
                DeliveryDepth = deliveryDepth,
                IsFeatured = isFeatured,
                IsActive = isActive,
                AssemblyId = assemblyId,
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
            string? firstUploadedImageUrl = null;
            if (imageUrls.Any())
            {
                for (int i = 0; i < imageUrls.Count; i++)
                {
                    var productImage = new ProductImage
                    {
                        ProductId = createdProduct.Id,
                        ImageUrl = imageUrls[i],
                        IsPrimary = i == 0,
                        SortOrder = i + 1
                    };
                    if (i == 0) firstUploadedImageUrl = imageUrls[i];
                    await _productRepository.AddProductImageAsync(productImage);
                }
            }

            // Upload ảnh riêng cho default variant (nếu có)
            string? variantImageUrl = null;
            if (defaultVariantImage != null)
            {
                try
                {
                    variantImageUrl = await _cloudinaryService.UploadImageAsync(defaultVariantImage, "product-variants");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not upload default variant image, fallback to first product image");
                }
            }

            // Auto-create default variant
            var defaultVariant = new ProductVariant
            {
                ProductId = createdProduct.Id,
                ColorName = !string.IsNullOrWhiteSpace(defaultVariantColorName)
                    ? defaultVariantColorName : "Mặc định",
                ColorNameEn = !string.IsNullOrWhiteSpace(defaultVariantColorNameEn)
                    ? defaultVariantColorNameEn : "Default",
                ColorCode = !string.IsNullOrWhiteSpace(defaultVariantColorCode)
                    ? defaultVariantColorCode : "#FFFFFF",
                Slug = ResolveDefaultVariantSlug(defaultVariantSlug, slug),
                ImageUrl = variantImageUrl
                    ?? (!string.IsNullOrWhiteSpace(defaultVariantImageUrl) ? defaultVariantImageUrl : null)
                    ?? firstUploadedImageUrl,
                IsActive = true
            };
            await _productRepository.AddProductVariantAsync(defaultVariant);

            var createdProductWithDetails = await _productRepository.GetProductWithDetailsAsync(createdProduct.Id);
            var productDto = MapToProductDto(createdProductWithDetails ?? createdProduct);
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
        static List<ProductVariantImageDto> MapVariantImages(ProductVariant pv)
        {
            var mapped = pv.ProductVariantImages?.Select(pvi => new ProductVariantImageDto
            {
                Id = pvi.Id,
                VariantId = pvi.VariantId,
                ImageUrl = pvi.ImageUrl,
                AltText = pvi.AltText,
                IsPrimary = pvi.IsPrimary,
                SortOrder = pvi.SortOrder,
                CreatedAt = pvi.CreatedAt,
                UpdatedAt = pvi.UpdatedAt
            }).ToList() ?? new List<ProductVariantImageDto>();

            if (mapped.Count == 0 && !string.IsNullOrWhiteSpace(pv.ImageUrl))
            {
                mapped.Add(new ProductVariantImageDto
                {
                    Id = 0,
                    VariantId = pv.Id,
                    ImageUrl = pv.ImageUrl,
                    AltText = pv.ColorName,
                    IsPrimary = true,
                    SortOrder = 0,
                    CreatedAt = pv.CreatedAt,
                    UpdatedAt = pv.UpdatedAt
                });
            }

            return mapped;
        }

        static string? NormalizeVariantSlugForClient(string? variantSlug, string productSlug)
        {
            if (string.IsNullOrWhiteSpace(variantSlug)) return null;

            var normalized = variantSlug.Trim();
            if (normalized.Equals(productSlug, StringComparison.OrdinalIgnoreCase)) return null;
            if (normalized.Equals($"{productSlug}-default", StringComparison.OrdinalIgnoreCase)) return null;
            return normalized;
        }

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            NameEn = product.NameEn,
            Slug = product.Slug,
            Description = product.Description,
            DescriptionEn = product.DescriptionEn,
            DimensionsHeight = product.DimensionsHeight,
            DimensionsWidth = product.DimensionsWidth,
            DimensionsDepth = product.DimensionsDepth,
            DimensionUnit = product.DimensionUnit,
            Detail = product.Detail,
            DetailEn = product.DetailEn,
            DeliveryInfo = product.DeliveryInfo,
            DeliveryInfoEn = product.DeliveryInfoEn,
            Weight = product.Weight,
            DeliveryHeight = product.DeliveryHeight,
            DeliveryWidth = product.DeliveryWidth,
            DeliveryDepth = product.DeliveryDepth,
            IsFeatured = product.IsFeatured,
            IsActive = product.IsActive,
            AssemblyId = product.AssemblyId,
            Assembly = product.Assembly == null ? null : new AssemblyDto
            {
                Id = product.Assembly.Id,
                NameVi = product.Assembly.NameVi,
                NameEn = product.Assembly.NameEn,
                Code = product.Assembly.Code,
                DescriptionVi = product.Assembly.DescriptionVi,
                DescriptionEn = product.Assembly.DescriptionEn,
                IsActive = product.Assembly.IsActive,
                CreatedAt = product.Assembly.CreatedAt,
                UpdatedAt = product.Assembly.UpdatedAt
            },
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            Categories = product.ProductCategories?
                .Where(pc => pc.Category != null)
                .Select(pc => new CategoryDto
            {
                Id = pc.Category.Id,
                Name = pc.Category.Name,
                NameEn = pc.Category.NameEn,
                Description = pc.Category.Description,
                DescriptionEn = pc.Category.DescriptionEn,
                ImageUrl = pc.Category.ImageUrl,
                ParentId = pc.Category.ParentId,
                IsActive = pc.Category.IsActive,
                CreatedAt = pc.Category.CreatedAt,
                UpdatedAt = pc.Category.UpdatedAt
            }).ToList() ?? new List<CategoryDto>(),
            Materials = product.ProductMaterials?
                .Where(pm => pm.Material != null)
                .Select(pm => new MaterialDto
            {
                Id = pm.Material.Id,
                NameVi = pm.Material.NameVi,
                NameEn = pm.Material.NameEn,
                DescriptionVi = pm.Material.DescriptionVi,
                DescriptionEn = pm.Material.DescriptionEn,
                IsActive = pm.Material.IsActive,
                CreatedAt = pm.Material.CreatedAt,
                UpdatedAt = pm.Material.UpdatedAt
            }).ToList() ?? new List<MaterialDto>(),
            Variants = product.ProductVariants?.Select(pv => new ProductVariantDto
            {
                Id = pv.Id,
                ProductId = pv.ProductId,
                ColorName = pv.ColorName,
                ColorNameEn = pv.ColorNameEn,
                Slug = NormalizeVariantSlugForClient(pv.Slug, product.Slug),
                ColorCode = pv.ColorCode,
                ImageUrl = pv.ImageUrl,
                IsActive = pv.IsActive,
                CreatedAt = pv.CreatedAt,
                UpdatedAt = pv.UpdatedAt,
                Images = MapVariantImages(pv)
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

    private static string? ResolveDefaultVariantSlug(string? requestedSlug, string productSlug)
    {
        if (string.IsNullOrWhiteSpace(requestedSlug)) return null;

        var normalized = requestedSlug.Trim();
        if (normalized.Equals(productSlug, StringComparison.OrdinalIgnoreCase)) return null;
        if (normalized.Equals($"{productSlug}-default", StringComparison.OrdinalIgnoreCase)) return null;
        return normalized;
    }

    private sealed class ResolveSlugResponse
    {
        public required ProductDto Product { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? VariantSlug { get; set; }
    }
}
