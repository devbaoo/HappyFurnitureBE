using HappyFurnitureBE.Domain.Entities;

namespace HappyFurnitureBE.Domain.Interfaces;

public interface ICompanyInfoRepository : IRepository<CompanyInfo>
{
    Task<IEnumerable<CompanyInfo>> GetActiveAsync();
    Task<IEnumerable<CompanyInfo>> GetAllWithFilterAsync(string? name, bool? isActive);
}
