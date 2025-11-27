using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Models.Entities;

namespace EasyHouse.Simulations.Domain.Services;

public interface IClientCommandService
{
    Task<Client?> Handle(CreateClientCommand command);
    Task<Client?> Update(Guid id, UpdateClientCommand command); 
    Task<bool> Delete(Guid id);
}