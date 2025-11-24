using EasyHouse.IAM.Domain.Entities;
using EasyHouse.IAM.Domain.Repositories;
using EasyHouse.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EasyHouse.IAM.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> FindByEmailAsync(string email)
        => await _context.Set<User>().FirstOrDefaultAsync(x => x.Email == email);

    public async Task AddAsync(User user)
        => await _context.Set<User>().AddAsync(user);
}