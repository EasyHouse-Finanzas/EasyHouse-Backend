namespace EasyHouse.Simulations.Domain.Models.Comands;

public class UpdateHouseCommand
{
    public string Project { get; set; }
    public string PropertyCode { get; set; }
    public decimal TotalArea { get; set; }
    public decimal BuiltArea { get; set; }
    public string Location { get; set; }
    public decimal Price { get; set; }
    
    public string Currency { get; set; }
}