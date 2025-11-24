using System.Text.Json.Serialization;

namespace EasyHouse.Simulations.Domain.Models.Entities;

public class AmortizationDetail
{
    public Guid Id { get; set; } 
    
    public Guid SimulationId { get; set; } 
    public int Period { get; set; } 
    
    public decimal Payment { get; set; } 
    public decimal Interest { get; set; } 
    public decimal Amortization { get; set; }
    public decimal Balance { get; set; }
    
    [JsonIgnore] 
    public Simulation Simulation { get; set; } = null!;
}