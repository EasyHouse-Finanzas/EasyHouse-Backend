using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Models.Entities;

namespace EasyHouse.Simulations.Domain.Services;

public interface IConfigCommandService
{
    Task<Config?> Create(CreateConfigCommand command);
    Task<Config?> Update(Guid id, UpdateConfigCommand command);
    Task<bool> Delete(Guid id);
}