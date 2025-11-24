namespace EasyHouse.IAM.Domain.Services;
using EasyHouse.IAM.Domain.Entities;

public interface ITokenService
{
    string GenerateToken(User user);
}