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
                .ThenInclude(pv => pv.ProductVariantImages)
            .Include(p => p.ProductImages)
            .Include(p => p.Assembly)
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<Product?> GetByVariantSlugAsync(string variantSlug)
    {
        return await _dbSet
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.ProductMaterials)
                .ThenInclude(pm => pm.Material)
            .Include(p => p.ProductVariants)
                .ThenInclude(pv => pv.ProductVariantImages)
            .Include(p => p.ProductImages)
            .Include(p => p.Assembly)
            .FirstOrDefaultAsync(p => p.ProductVariants.Any(v => v.Slug == variantSlug));
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
                .ThenInclude(pv => pv.ProductVariantImages)
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
                .ThenInclude(pv => pv.ProductVariantImages)
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
                .ThenInclude(pv => pv.ProductVariantImages)
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
                .ThenInclude(pv => pv.ProductVariantImages)
            .Include(p => p.ProductImages)
            .Include(p => p.Assembly)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> SlugExistsAsync(string slug)
    {
        return await _dbSet.AnyAsync(p => p.Slug == slug);
    }

    // ─── Product Images ──────────────────────────────────────────────────────────

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

    public async Task<IEnumerable<ProductImage>> GetProductImagesAsync(int productId)
    {
        return await _context.ProductImages
            .Where(pi => pi.ProductId == productId)
            .AsNoTracking()
            .OrderBy(pi => pi.SortOrder)
            .ToListAsync();
    }

    public async Task DeleteProductImageAsync(int id)
    {
        var productImage = await _context.ProductImages
            .FirstOrDefaultAsync(pi => pi.Id == id);
        if (productImage != null)
        {
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteProductImagesAsync(int productId)
    {
        var images = await _context.ProductImages
            .Where(pi => pi.ProductId == productId)
            .ToListAsync();
        if (images.Any())
        {
            _context.ProductImages.RemoveRange(images);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnsetPrimaryProductImagesAsync(int productId)
    {
        var primaryImages = await _context.ProductImages
            .Where(pi => pi.ProductId == productId && pi.IsPrimary)
            .ToListAsync();
        foreach (var image in primaryImages)
            image.IsPrimary = false;

        await _context.SaveChangesAsync();
    }

    public async Task SetPrimaryImageAsync(int imageId)
    {
        var image = await GetProductImageByIdAsync(imageId);
        if (image != null)
        {
            await UnsetPrimaryProductImagesAsync(image.ProductId);
            image.IsPrimary = true;
            await _context.SaveChangesAsync();
        }
    }

    // ─── Product Variant Images ──────────────────────────────────────────────────

    public async Task<ProductVariantImage?> GetProductVariantImageByIdAsync(int id)
    {
        return await _context.ProductVariantImages.FirstOrDefaultAsync(pvi => pvi.Id == id);
    }

    public async Task<IEnumerable<ProductVariantImage>> GetVariantImagesAsync(int variantId)
    {
        return await _context.ProductVariantImages
            .Where(pvi => pvi.VariantId == variantId)
            .AsNoTracking()
            .OrderBy(pvi => pvi.SortOrder)
            .ToListAsync();
    }

    public async Task<ProductVariantImage> AddProductVariantImageAsync(ProductVariantImage image)
    {
        _context.ProductVariantImages.Add(image);
        await _context.SaveChangesAsync();
        return image;
    }

    public async Task UpdateProductVariantImageAsync(ProductVariantImage image)
    {
        _context.ProductVariantImages.Update(image);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductVariantImageAsync(int id)
    {
        var image = await _context.ProductVariantImages.FirstOrDefaultAsync(pvi => pvi.Id == id);
        if (image != null)
        {
            _context.ProductVariantImages.Remove(image);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteProductVariantImagesAsync(int variantId)
    {
        var images = await _context.ProductVariantImages
            .Where(pvi => pvi.VariantId == variantId)
            .ToListAsync();
        if (images.Any())
        {
            _context.ProductVariantImages.RemoveRange(images);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnsetPrimaryVariantImagesAsync(int variantId)
    {
        var primaryImages = await _context.ProductVariantImages
            .Where(pvi => pvi.VariantId == variantId && pvi.IsPrimary)
            .ToListAsync();
        foreach (var image in primaryImages)
            image.IsPrimary = false;

        await _context.SaveChangesAsync();
    }

    public async Task SetPrimaryVariantImageAsync(int imageId)
    {
        var image = await GetProductVariantImageByIdAsync(imageId);
        if (image != null)
        {
            await UnsetPrimaryVariantImagesAsync(image.VariantId);
            image.IsPrimary = true;
            await _context.SaveChangesAsync();
        }
    }

    // ─── Product Variants ────────────────────────────────────────────────────────

    public async Task<ProductVariant?> GetProductVariantByIdAsync(int id)
    {
        return await _context.ProductVariants
            .Include(pv => pv.ProductVariantImages)
            .FirstOrDefaultAsync(pv => pv.Id == id);
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
            .Include(pv => pv.ProductVariantImages)
            .ToListAsync();
    }

    // ─── Product Categories ──────────────────────────────────────────────────────

    public async Task<ProductCategory> AddProductCategoryAsync(ProductCategory productCategory)
    {
        _context.ProductCategories.Add(productCategory);
        await _context.SaveChangesAsync();
        return productCategory;
    }

    public async Task DeleteProductCategoriesAsync(int productId)
    {
        var existing = await _context.ProductCategories
            .Where(pc => pc.ProductId == productId)
            .ToListAsync();
        _context.ProductCategories.RemoveRange(existing);
        await _context.SaveChangesAsync();
    }

    // ─── Product Materials ───────────────────────────────────────────────────────

    public async Task<ProductMaterial> AddProductMaterialAsync(ProductMaterial productMaterial)
    {
        _context.ProductMaterials.Add(productMaterial);
        await _context.SaveChangesAsync();
        return productMaterial;
    }

    public async Task DeleteProductMaterialsAsync(int productId)
    {
        var existing = await _context.ProductMaterials
            .Where(pm => pm.ProductId == productId)
            .ToListAsync();
        _context.ProductMaterials.RemoveRange(existing);
        await _context.SaveChangesAsync();
    }
}