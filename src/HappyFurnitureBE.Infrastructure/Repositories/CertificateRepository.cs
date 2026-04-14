using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using HappyFurnitureBE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HappyFurnitureBE.Infrastructure.Repositories;

public class CertificateRepository : BaseRepository<Certificate>, ICertificateRepository
{
    public CertificateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Certificate>> GetActiveCertificatesAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Certificate>> GetAllWithFilterAsync(string? name, bool? isActive)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(c =>
                c.NameVi.Contains(name) ||
                (c.NameEn != null && c.NameEn.Contains(name)));

        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        return await query.OrderBy(c => c.SortOrder).ThenByDescending(c => c.CreatedAt).ToListAsync();
    }

    public async Task<bool> SlugExistsAsync(string name, int? excludeId = null)
    {
        var query = _dbSet.Where(c => c.NameVi == name || c.NameEn == name);
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);
        return await query.AnyAsync();
    }
}
