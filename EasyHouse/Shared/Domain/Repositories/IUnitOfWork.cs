namespace EasyHouse.Shared.Domain.Repositories;

public interface IUnitOfWork
{
    Task CompleteAsync();
}