using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using HappyFurnitureBE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HappyFurnitureBE.Infrastructure.Repositories;

public class MaterialRepository : BaseRepository<Material>, IMaterialRepository
{
    public MaterialRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Material>> GetActiveMaterialsAsync()
    {
        return await _dbSet
            .Where(m => m.IsActive)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<Material?> GetByIdWithRelationsAsync(int id)
    {
        return await _dbSet
            .Include(m => m.ProductMaterials)
                .ThenInclude(pm => pm.Product)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
}
