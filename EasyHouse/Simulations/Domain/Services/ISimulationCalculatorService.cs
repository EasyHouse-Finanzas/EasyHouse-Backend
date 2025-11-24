using EasyHouse.Simulations.Domain.Models.Entities;

namespace EasyHouse.Simulations.Domain.Services;

public interface ISimulationCalculatorService
{
    void Calculate(Simulation simulation, House house, Config config);
}