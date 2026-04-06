using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using HappyFurnitureBE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HappyFurnitureBE.Infrastructure.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetBySlugAsync(string slug)
    {
        return await _dbSet
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.ProductMaterials)
                .ThenInclude(pm => pm.Material)
            .Include(p => p.ProductVariants)
            .Include(p => p.ProductImages)
            .Include(p => p.Assembly)
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        return await _dbSet
            .Where(p => p.IsFeatured && p.IsActive)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.ProductMaterials)
                .ThenInclude(pm => pm.Material)
            .Include(p => p.ProductVariants)
            .Include(p => p.ProductImages)
            .Include(p => p.Assembly)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.ProductMaterials)
                .ThenInclude(pm => pm.Material)
            .Include(p => p.ProductVariants)
            .Include(p => p.ProductImages)
            .Include(p => p.Assembly)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Where(p => p.IsActive && p.ProductCategories.Any(pc => pc.CategoryId == categoryId))
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.ProductMaterials)
                .ThenInclude(pm => pm.Material)
            .Include(p => p.ProductVariants)
            .Include(p => p.ProductImages)
            .Include(p => p.Assembly)
            .ToListAsync();
    }

    public async Task<Product?> GetProductWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.ProductMaterials)
                .ThenInclude(pm => pm.Material)
            .Include(p => p.ProductVariants)
            .Include(p => p.ProductImages)
            .Include(p => p.Assembly)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> SlugExistsAsync(string slug)
    {
        return await _dbSet.AnyAsync(p => p.Slug == slug);
    }

    // Product Images
    public async Task<ProductImage?> GetProductImageByIdAsync(int id)
    {
        return await _context.ProductImages.FirstOrDefaultAsync(pi => pi.Id == id);
    }

    public async Task<ProductImage> AddProductImageAsync(ProductImage productImage)
    {
        _context.ProductImages.Add(productImage);
        await _context.SaveChangesAsync();
        return productImage;
    }

    public async Task UpdateProductImageAsync(ProductImage productImage)
    {
        _context.ProductImages.Update(productImage);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductImageAsync(int id)
    {
        var productImage = await GetProductImageByIdAsync(id);
        if (productImage != null)
        {
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnsetPrimaryImagesAsync(int productId)
    {
        var primaryImages = await _context.ProductImages
            .Where(pi => pi.ProductId == productId && pi.IsPrimary)
            .ToListAsync();

        foreach (var image in primaryImages)
        {
            image.IsPrimary = false;
        }

        await _context.SaveChangesAsync();
    }

    public async Task SetPrimaryImageAsync(int imageId)
    {
        var image = await GetProductImageByIdAsync(imageId);
        if (image != null)
        {
            await UnsetPrimaryImagesAsync(image.ProductId);
            image.IsPrimary = true;
            await _context.SaveChangesAsync();
        }
    }

    // Product Variants
    public async Task<ProductVariant?> GetProductVariantByIdAsync(int id)
    {
        return await _context.ProductVariants.FirstOrDefaultAsync(pv => pv.Id == id);
    }

    public async Task<ProductVariant> AddProductVariantAsync(ProductVariant productVariant)
    {
        _context.ProductVariants.Add(productVariant);
        await _context.SaveChangesAsync();
        return productVariant;
    }

    public async Task UpdateProductVariantAsync(ProductVariant productVariant)
    {
        _context.ProductVariants.Update(productVariant);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductVariantAsync(int id)
    {
        var productVariant = await GetProductVariantByIdAsync(id);
        if (productVariant != null)
        {
            _context.ProductVariants.Remove(productVariant);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ProductVariant>> GetActiveProductVariantsAsync(int productId)
    {
        return await _context.ProductVariants
            .Where(pv => pv.ProductId == productId && pv.IsActive)
            .ToListAsync();
    }

    // Product Categories
    public async Task<ProductCategory> AddProductCategoryAsync(ProductCategory productCategory)
    {
        _context.ProductCategories.Add(productCategory);
        await _context.SaveChangesAsync();
        return productCategory;
    }

    // Product Materials
    public async Task<ProductMaterial> AddProductMaterialAsync(ProductMaterial productMaterial)
    {
        _context.ProductMaterials.Add(productMaterial);
        await _context.SaveChangesAsync();
        return productMaterial;
    }
}