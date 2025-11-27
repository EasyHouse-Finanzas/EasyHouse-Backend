namespace EasyHouse.Simulations.Domain.Models.Comands;

public class UpdateClientCommand
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string DocumentNumber { get; set; }
    public string Occupation { get; set; }
    public decimal MonthlyIncome { get; set; }
}