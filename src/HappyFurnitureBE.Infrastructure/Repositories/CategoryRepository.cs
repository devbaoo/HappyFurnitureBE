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
            .ThenInclude(child => child.Children)
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

    public async Task<IEnumerable<Category>> GetAllWithRelationsAsync()
    {
        return await _dbSet
            .Include(c => c.Parent)
            .Include(c => c.Children)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdWithRelationsAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Parent)
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}