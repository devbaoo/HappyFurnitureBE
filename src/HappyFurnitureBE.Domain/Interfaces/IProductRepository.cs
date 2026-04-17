using HappyFurnitureBE.Domain.Entities;

namespace HappyFurnitureBE.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetBySlugAsync(string slug);
    Task<Product?> GetByVariantSlugAsync(string variantSlug);
    Task<IEnumerable<Product>> GetFeaturedProductsAsync();
    Task<IEnumerable<Product>> GetActiveProductsAsync();
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<Product?> GetProductWithDetailsAsync(int id);
    Task<bool> SlugExistsAsync(string slug);
    
    // Product Images
    Task<ProductImage?> GetProductImageByIdAsync(int id);
    Task<IEnumerable<ProductImage>> GetProductImagesAsync(int productId);
    Task<ProductImage> AddProductImageAsync(ProductImage productImage);
    Task UpdateProductImageAsync(ProductImage productImage);
    Task DeleteProductImageAsync(int id);
    Task DeleteProductImagesAsync(int productId);
    Task UnsetPrimaryProductImagesAsync(int productId);
    Task SetPrimaryImageAsync(int imageId);
    
    // Product Variant Images
    Task<ProductVariantImage?> GetProductVariantImageByIdAsync(int id);
    Task<IEnumerable<ProductVariantImage>> GetVariantImagesAsync(int variantId);
    Task<ProductVariantImage> AddProductVariantImageAsync(ProductVariantImage image);
    Task UpdateProductVariantImageAsync(ProductVariantImage image);
    Task DeleteProductVariantImageAsync(int id);
    Task DeleteProductVariantImagesAsync(int variantId);
    Task UnsetPrimaryVariantImagesAsync(int variantId);
    Task SetPrimaryVariantImageAsync(int imageId);

    // Product Variants
    Task<ProductVariant?> GetProductVariantByIdAsync(int id);
    Task<ProductVariant> AddProductVariantAsync(ProductVariant productVariant);
    Task UpdateProductVariantAsync(ProductVariant productVariant);
    Task DeleteProductVariantAsync(int id);
    Task<IEnumerable<ProductVariant>> GetActiveProductVariantsAsync(int productId);
    
    // Product Categories
    Task<ProductCategory> AddProductCategoryAsync(ProductCategory productCategory);
    Task DeleteProductCategoriesAsync(int productId);

    // Product Materials
    Task<ProductMaterial> AddProductMaterialAsync(ProductMaterial productMaterial);
    Task DeleteProductMaterialsAsync(int productId);
}