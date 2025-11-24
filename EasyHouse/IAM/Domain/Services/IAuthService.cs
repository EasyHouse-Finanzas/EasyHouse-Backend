using EasyHouse.IAM.Domain.Commands;
using EasyHouse.IAM.Domain.Entities;
using EasyHouse.IAM.Domain.Results;

namespace EasyHouse.IAM.Domain.Services;

public interface IAuthService
{
    Task<User> SignUpAsync(SignUpCommand command);
    Task<AuthResult> SignInAsync(SignInCommand command);
}