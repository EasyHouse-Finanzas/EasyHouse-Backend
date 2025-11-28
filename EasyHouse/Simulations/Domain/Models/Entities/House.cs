using System.Text.Json.Serialization;

namespace EasyHouse.Simulations.Domain.Models.Entities;

public class House
{
    public Guid HouseId { get; set; }
    public string Project { get; set; }
    public string PropertyCode { get; set; }
    public decimal TotalArea { get; set; }
    public decimal? BuiltArea { get; set; }
    public string Location { get; set; }
    public decimal Price { get; set; }
    
    public string Currency { get; set; }
    
    [JsonIgnore]
    public List<Simulation> Simulations { get; set; } = new();
}