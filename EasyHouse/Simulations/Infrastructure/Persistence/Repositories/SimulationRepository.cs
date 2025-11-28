using EasyHouse.Shared.Infrastructure;
using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace EasyHouse.Simulations.Infrastructure.Persistence.Repositories;

public class SimulationRepository : BaseRepository<Simulation>, ISimulationRepository
{
    public SimulationRepository(AppDbContext context) : base(context) { }

    public async Task<Simulation?> FindDetailedByIdAsync(Guid id)
    {
        return await Context.Set<Simulation>()
            .Include(s => s.Client)
            .Include(s => s.House)
            .Include(s => s.Config)
            .FirstOrDefaultAsync(s => s.SimulationId == id);
    }


    public async Task<IEnumerable<Simulation>> FindAllAsync()
    {
        return await Context.Set<Simulation>()

            .Include(s => s.Client) 
            .Include(s => s.House)
            .Include(s => s.Config)
            .ToListAsync();
    }

    public async Task<Client?> GetClientByIdAsync(Guid id)
    {
        return await Context.Set<Client>().FirstOrDefaultAsync(c => c.ClientId == id);
    }

    public async Task<House?> GetHouseByIdAsync(Guid id)
    {
        return await Context.Set<House>().FirstOrDefaultAsync(h => h.HouseId == id);
    }

    public async Task<Config?> GetConfigByIdAsync(Guid id)
    {
        return await Context.Set<Config>().FirstOrDefaultAsync(c => c.ConfigId == id);
    }
}