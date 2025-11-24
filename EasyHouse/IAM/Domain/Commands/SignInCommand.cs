namespace EasyHouse.IAM.Domain.Commands;

public class SignInCommand
{
    public string Email { get; set; }
    public string Password { get; set; }
}