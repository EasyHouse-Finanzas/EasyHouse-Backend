using EasyHouse.IAM.Domain.Commands;
using EasyHouse.IAM.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyHouse.IAM.Interfaces;

[ApiController]
[Route("api/v1/iam/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpCommand command)
    {
        var user = await _auth.SignUpAsync(command);
        return Ok(new { message = "User creado", user.Email });
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(SignInCommand command)
    {
        var result = await _auth.SignInAsync(command);
        return Ok(result);
    }
}