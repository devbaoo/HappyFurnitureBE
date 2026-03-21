using HappyFurnitureBE.Domain.Entities;

namespace HappyFurnitureBE.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetBySlugAsync(string slug);
    Task<IEnumerable<Product>> GetFeaturedProductsAsync();
    Task<IEnumerable<Product>> GetActiveProductsAsync();
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<Product?> GetProductWithDetailsAsync(int id);
    Task<bool> SlugExistsAsync(string slug);
    
    // Product Images
    Task<ProductImage?> GetProductImageByIdAsync(int id);
    Task<ProductImage> AddProductImageAsync(ProductImage productImage);
    Task UpdateProductImageAsync(ProductImage productImage);
    Task DeleteProductImageAsync(int id);
    Task UnsetPrimaryImagesAsync(int productId);
    Task SetPrimaryImageAsync(int imageId);
    
    // Product Variants
    Task<ProductVariant?> GetProductVariantByIdAsync(int id);
    Task<ProductVariant> AddProductVariantAsync(ProductVariant productVariant);
    Task UpdateProductVariantAsync(ProductVariant productVariant);
    Task DeleteProductVariantAsync(int id);
    Task<IEnumerable<ProductVariant>> GetActiveProductVariantsAsync(int productId);
    
    // Product Categories
    Task<ProductCategory> AddProductCategoryAsync(ProductCategory productCategory);
}