using EasyHouse.Shared.Domain.Repositories;
using EasyHouse.Simulations.Domain.Models.Entities;

namespace EasyHouse.Simulations.Domain.Models.Repository;

public interface ISimulationRepository : IBaseRepository<Simulation>
{
    Task<Simulation?> FindDetailedByIdAsync(Guid id);
    

    Task<IEnumerable<Simulation>> FindAllByUserIdAsync(Guid userId); 
    
    Task<House?> GetHouseByIdAsync(Guid id);
    Task<Client?> GetClientByIdAsync(Guid id);
    Task<Config?> GetConfigByIdAsync(Guid id);
}