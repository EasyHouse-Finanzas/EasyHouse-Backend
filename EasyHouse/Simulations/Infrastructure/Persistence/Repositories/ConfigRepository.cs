using EasyHouse.Shared.Infrastructure;
using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Models.Repository;

namespace EasyHouse.Simulations.Infrastructure.Persistence.Repositories;

public class ConfigRepository : BaseRepository<Config>, IConfigRepository
{
    public ConfigRepository(AppDbContext context) : base(context) { }
}