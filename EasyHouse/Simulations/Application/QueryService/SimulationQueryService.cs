using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Models.Repository;
using EasyHouse.Simulations.Domain.Services;

namespace EasyHouse.Simulations.Application.QueryService;

public class SimulationQueryService : ISimulationQueryService
{
    private readonly ISimulationRepository _repository;

    public SimulationQueryService(ISimulationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Simulation?> GetDetailedSimulationByIdAsync(Guid id)
    {
        return await _repository.FindDetailedByIdAsync(id);
    }
    public async Task<IEnumerable<Simulation>> GetAllSimulationsByUserIdAsync(Guid userId)
    {
        return await _repository.FindAllByUserIdAsync(userId);
    }
}