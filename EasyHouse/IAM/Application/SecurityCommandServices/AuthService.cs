using EasyHouse.IAM.Domain.Commands;
using EasyHouse.IAM.Domain.Entities;
using EasyHouse.IAM.Domain.Repositories;
using EasyHouse.IAM.Domain.Results;
using EasyHouse.IAM.Domain.Services;
using EasyHouse.Shared.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using EasyHouse.IAM.Infrastructure;

namespace EasyHouse.IAM.Application.SecurityCommandServices;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ITokenService _token;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RecaptchaValidationService _recaptchaService;

    public AuthService(
        IUserRepository users, 
        ITokenService token, 
        IUnitOfWork unitOfWork,
        RecaptchaValidationService recaptchaService) 
    {
        _users = users;
        _token = token;
        _unitOfWork = unitOfWork;
        _recaptchaService = recaptchaService; 
    }

    public async Task<User> SignUpAsync(SignUpCommand command)
    {
        var existing = await _users.FindByEmailAsync(command.Email);
        if (existing != null)
            throw new Exception("El correo ya está registrado.");

        var user = new User
        {
            UserId = Guid.NewGuid(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Number = command.Number,
            Email = command.Email,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        var hasher = new PasswordHasher<User>();
        user.Password = hasher.HashPassword(user, command.Password);

        await _users.AddAsync(user);
        await _unitOfWork.CompleteAsync();

        return user;
    }

    public async Task<AuthResult> SignInAsync(SignInCommand command)
    {
        var captchaValido = await _recaptchaService.ValidateTokenAsync(command.RecaptchaToken);
        if (!captchaValido)
        {
            throw new Exception("Captcha inválido o expirado. Por favor recarga la página.");
        }
        
        var user = await _users.FindByEmailAsync(command.Email);
        if (user == null)
            throw new Exception("Usuario no encontrado.");

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.Password, command.Password);

        if (result == PasswordVerificationResult.Failed)
            throw new Exception("Contraseña incorrecta.");

        var token = _token.GenerateToken(user);

        return new AuthResult
        {
            Token = token,
            Email = user.Email,
            Id = user.UserId,
            FirstName = user.FirstName, 
            LastName = user.LastName
        };
    }
}