using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using HappyFurnitureBE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HappyFurnitureBE.Infrastructure.Repositories;

public class NewsRepository : BaseRepository<News>, INewsRepository
{
    public NewsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<News?> GetBySlugAsync(string slug)
    {
        return await _dbSet.FirstOrDefaultAsync(n => n.Slug == slug);
    }

    public async Task<News?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<IEnumerable<News>> GetActiveNewsAsync()
    {
        return await _dbSet
            .Where(n => n.IsActive)
            .OrderByDescending(n => n.Type)
            .ThenBy(n => n.SortOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<News>> GetActiveEventsAsync()
    {
        return await _dbSet
            .Where(n => n.Type == NewsType.Event && n.IsActive)
            .OrderBy(n => n.SortOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<News>> GetActiveActivitiesAsync()
    {
        return await _dbSet
            .Where(n => n.Type == NewsType.Activity && n.IsActive)
            .OrderBy(n => n.SortOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<News>> GetAllWithFilterAsync(
        string? type, string? title, bool? isActive)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(n => n.Type.ToString() == type);

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(n =>
                n.TitleVi.Contains(title) ||
                (n.TitleEn != null && n.TitleEn.Contains(title)));

        if (isActive.HasValue)
            query = query.Where(n => n.IsActive == isActive.Value);

        return await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
    }

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
    {
        var query = _dbSet.Where(n => n.Slug == slug);
        if (excludeId.HasValue)
            query = query.Where(n => n.Id != excludeId.Value);
        return await query.AnyAsync();
    }
}
