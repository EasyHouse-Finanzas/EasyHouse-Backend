using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Models.Entities;

namespace EasyHouse.Simulations.Domain.Services;

public interface IReportCommandService
{
    Task<Report?> Generate(Guid simulationId, GenerateReportCommand command);
}