using EasyHouse.Shared.Infrastructure;
using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace EasyHouse.Simulations.Infrastructure.Persistence.Repositories;

public class HouseRepository : BaseRepository<House>, IHouseRepository
{
    public HouseRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<House>> FindAllByUserIdAsync(Guid userId)
    {
        return await Context.Set<House>()
            // Asegúrate de tener la propiedad UserId en tu entidad House.cs
            // Si no existe, agrégala y haz la migración, o comenta esta línea temporalmente.
            // .Where(h => h.UserId == userId) 
            .ToListAsync();
    }
}