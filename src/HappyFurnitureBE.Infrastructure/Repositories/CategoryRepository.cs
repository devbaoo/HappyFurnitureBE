using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using HappyFurnitureBE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HappyFurnitureBE.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
    {
        return await _dbSet
            .Where(c => c.ParentId == null)
            .Include(c => c.Children)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId)
    {
        return await _dbSet
            .Where(c => c.ParentId == parentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .ToListAsync();
    }
}