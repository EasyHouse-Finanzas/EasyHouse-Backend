using EasyHouse.Shared.Domain.Repositories;

namespace EasyHouse.Shared.Infrastructure;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task CompleteAsync()
        => await context.SaveChangesAsync();
}