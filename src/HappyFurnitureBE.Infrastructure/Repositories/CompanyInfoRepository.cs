using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using HappyFurnitureBE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HappyFurnitureBE.Infrastructure.Repositories;

public class CompanyInfoRepository : BaseRepository<CompanyInfo>, ICompanyInfoRepository
{
    public CompanyInfoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CompanyInfo>> GetActiveAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CompanyInfo>> GetAllWithFilterAsync(string? name, bool? isActive)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(c =>
                c.NameVi.Contains(name) ||
                (c.NameEn != null && c.NameEn.Contains(name)));

        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
    }
}
