using HappyFurnitureBE.Domain.Entities;

namespace HappyFurnitureBE.Domain.Interfaces;

public interface IContactRepository : IRepository<Contact>
{
    Task<IEnumerable<Contact>> GetAllWithFilterAsync(bool? isRead);
}
