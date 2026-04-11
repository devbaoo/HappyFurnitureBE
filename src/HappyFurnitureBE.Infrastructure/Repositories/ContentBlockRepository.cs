using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using HappyFurnitureBE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HappyFurnitureBE.Infrastructure.Repositories;

public class ContentBlockRepository : BaseRepository<ContentBlock>, IContentBlockRepository
{
    public ContentBlockRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ContentBlock>> GetByNewsIdAsync(int newsId)
    {
        return await _dbSet
            .Where(cb => cb.NewsId == newsId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ContentBlock>> GetByNewsIdOrderedAsync(int newsId)
    {
        return await _dbSet
            .Where(cb => cb.NewsId == newsId)
            .OrderBy(cb => cb.SortOrder)
            .ToListAsync();
    }

    public async Task DeleteByNewsIdAsync(int newsId)
    {
        var blocks = await _dbSet.Where(cb => cb.NewsId == newsId).ToListAsync();
        _dbSet.RemoveRange(blocks);
        await _context.SaveChangesAsync();
    }
}
