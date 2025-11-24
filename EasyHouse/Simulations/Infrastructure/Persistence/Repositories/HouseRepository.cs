using EasyHouse.Shared.Infrastructure;
using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Models.Repository;

namespace EasyHouse.Simulations.Infrastructure.Persistence.Repositories;

public class HouseRepository : BaseRepository<House>, IHouseRepository
{
    public HouseRepository(AppDbContext context) : base(context) { }
}