using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Models.Entities;

namespace EasyHouse.Simulations.Domain.Services;

public interface IHouseCommandService
{
    Task<House?> Create(CreateHouseCommand command);
    Task<House?> Update(Guid id, UpdateHouseCommand command);
    Task<bool> Delete(Guid id);
}