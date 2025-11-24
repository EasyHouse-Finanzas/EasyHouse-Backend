using System.Text.Json.Serialization;
using EasyHouse.IAM.Domain.Entities;

namespace EasyHouse.Simulations.Domain.Models.Entities;

public class Client
{
    public Guid ClientId { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }

    public string DocumentNumber { get; set; } // DNI
    public string Occupation { get; set; }
    public decimal MonthlyIncome { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; }
    
    [JsonIgnore]
    public List<Simulation> Simulations { get; set; } = new();
}