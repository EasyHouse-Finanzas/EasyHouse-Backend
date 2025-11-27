namespace EasyHouse.IAM.Domain.Results;

public class AuthResult
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}