using EasyHouse.Shared.Domain.Repositories;
using EasyHouse.Simulations.Domain.Models.Entities;

namespace EasyHouse.Simulations.Domain.Models.Repository;

public interface IHouseRepository : IBaseRepository<House> 
{
    Task<IEnumerable<House>> FindAllByUserIdAsync(Guid userId);
}