namespace EasyHouse.IAM.Domain.Commands;

public class SignUpCommand
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Number { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}