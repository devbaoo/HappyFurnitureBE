using HappyFurnitureBE.Domain.Entities;

namespace HappyFurnitureBE.Domain.Interfaces;

public interface IContentBlockRepository : IRepository<ContentBlock>
{
    Task<IEnumerable<ContentBlock>> GetByNewsIdAsync(int newsId);
    Task<IEnumerable<ContentBlock>> GetByNewsIdOrderedAsync(int newsId);
    Task DeleteByNewsIdAsync(int newsId);
}
