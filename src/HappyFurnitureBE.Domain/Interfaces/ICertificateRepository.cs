using HappyFurnitureBE.Domain.Entities;

namespace HappyFurnitureBE.Domain.Interfaces;

public interface ICertificateRepository : IRepository<Certificate>
{
    Task<IEnumerable<Certificate>> GetActiveCertificatesAsync();
    Task<IEnumerable<Certificate>> GetAllWithFilterAsync(string? name, bool? isActive);
    Task<bool> SlugExistsAsync(string name, int? excludeId = null);
}
