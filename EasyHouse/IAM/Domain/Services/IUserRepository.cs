using EasyHouse.IAM.Domain.Entities;

namespace EasyHouse.IAM.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email);
    Task AddAsync(User user);
}