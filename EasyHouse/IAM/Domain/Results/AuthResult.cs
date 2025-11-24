namespace EasyHouse.IAM.Domain.Results;

public class AuthResult
{
    public string Token { get; set; }
    public Guid Id { get; set; }
    public string Email { get; set; }
}