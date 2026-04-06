using HappyFurnitureBE.Domain.Entities;
using HappyFurnitureBE.Domain.Interfaces;
using HappyFurnitureBE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HappyFurnitureBE.Infrastructure.Repositories;

public class ContactRepository : BaseRepository<Contact>, IContactRepository
{
    public ContactRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Contact>> GetAllWithFilterAsync(bool? isRead)
    {
        var query = _dbSet.AsQueryable();

        if (isRead.HasValue)
            query = query.Where(c => c.IsRead == isRead.Value);

        return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
    }
}
