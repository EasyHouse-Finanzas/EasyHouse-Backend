using EasyHouse.Shared.Infrastructure;
using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace EasyHouse.Simulations.Infrastructure.Persistence.Repositories;

public class ClientRepository : BaseRepository<Client>, IClientRepository
{
    public ClientRepository(AppDbContext context) : base(context) {}

    public async Task<IEnumerable<Client>> FindAllByUserIdAsync(Guid userId)
    {
        return await Context.Set<Client>()
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }
}