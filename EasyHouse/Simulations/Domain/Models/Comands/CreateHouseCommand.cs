namespace EasyHouse.Simulations.Domain.Models.Comands;

public class CreateHouseCommand
{
    public string Proyecto { get; set; }
    public string CodigoInmueble { get; set; }
    public decimal AreaTotal { get; set; }
    public decimal? AreaTechada { get; set; }
    public string Ubicacion { get; set; }
    public decimal Precio { get; set; }
}