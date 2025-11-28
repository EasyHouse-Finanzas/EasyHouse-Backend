using EasyHouse.Simulations.Domain.Models.Entities;

namespace EasyHouse.Simulations.Domain.Services;

public interface ISimulationQueryService
{
    Task<Simulation?> GetDetailedSimulationByIdAsync(Guid id);
    
    // CAMBIO: Ahora pedimos el userId para filtrar
    Task<IEnumerable<Simulation>> GetAllSimulationsByUserIdAsync(Guid userId);
}