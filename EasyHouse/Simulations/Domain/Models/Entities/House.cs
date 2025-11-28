using System.Text.Json.Serialization;

namespace EasyHouse.Simulations.Domain.Models.Entities;

public class House
{
    public Guid HouseId { get; set; }
    public string Project { get; set; } = string.Empty; // Inicializar para evitar warnings
    public string PropertyCode { get; set; } = string.Empty;
    public decimal TotalArea { get; set; }
    public decimal? BuiltArea { get; set; }
    public string Location { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    public string Currency { get; set; } = "PEN";
    
    // ✅ NUEVA PROPIEDAD OBLIGATORIA
    public Guid UserId { get; set; } 
    
    [JsonIgnore]
    public List<Simulation> Simulations { get; set; } = new();
}