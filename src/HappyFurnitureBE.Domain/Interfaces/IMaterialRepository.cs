using HappyFurnitureBE.Domain.Entities;

namespace HappyFurnitureBE.Domain.Interfaces;

public interface IMaterialRepository : IRepository<Material>
{
    Task<IEnumerable<Material>> GetActiveMaterialsAsync();
    Task<Material?> GetByIdWithRelationsAsync(int id);
}
