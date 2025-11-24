namespace EasyHouse.Simulations.Domain.Models.Comands;

public record CreateSimulationCommand(
    Guid ClientId,
    Guid HouseId,
    Guid ConfigId,
    decimal InitialQuota,
    int TermMonths,
    DateTime StartDate
);