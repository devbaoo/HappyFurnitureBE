using HappyFurnitureBE.Domain.Entities;

namespace HappyFurnitureBE.Domain.Interfaces;

public interface IAssemblyRepository : IRepository<Assembly>
{
    Task<IEnumerable<Assembly>> GetActiveAssembliesAsync();
}
