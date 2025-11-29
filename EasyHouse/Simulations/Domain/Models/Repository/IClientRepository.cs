using EasyHouse.Shared.Domain.Repositories;
using EasyHouse.Simulations.Domain.Models.Entities;

namespace EasyHouse.Simulations.Domain.Models.Repository;

public interface IClientRepository : IBaseRepository<Client> 
{
    Task<IEnumerable<Client>> FindAllByUserIdAsync(Guid userId);
}