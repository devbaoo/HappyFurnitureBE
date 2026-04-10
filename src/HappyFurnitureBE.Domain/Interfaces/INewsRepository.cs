using HappyFurnitureBE.Domain.Entities;

namespace HappyFurnitureBE.Domain.Interfaces;

public interface INewsRepository : IRepository<News>
{
    Task<News?> GetBySlugAsync(string slug);
    Task<News?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<News>> GetActiveNewsAsync();
    Task<IEnumerable<News>> GetActiveEventsAsync();
    Task<IEnumerable<News>> GetActiveActivitiesAsync();
    Task<IEnumerable<News>> GetAllWithFilterAsync(string? type, string? category, string? title, bool? isActive);
    Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
}
