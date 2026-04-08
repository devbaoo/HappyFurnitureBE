using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using HappyFurnitureBE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HappyFurnitureBE.Infrastructure.Repositories;

public class AssemblyRepository : BaseRepository<Assembly>, IAssemblyRepository
{
    public AssemblyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Assembly>> GetActiveAssembliesAsync()
    {
        return await _dbSet
            .Where(a => a.IsActive)
            .OrderBy(a => a.NameVi)
            .ToListAsync();
    }
}
