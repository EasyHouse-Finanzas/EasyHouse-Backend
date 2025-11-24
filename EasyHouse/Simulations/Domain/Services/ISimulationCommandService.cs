using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Models.Entities;

namespace EasyHouse.Simulations.Domain.Services;

public interface ISimulationCommandService
{
    Task<Simulation?> Handle(CreateSimulationCommand command);
}