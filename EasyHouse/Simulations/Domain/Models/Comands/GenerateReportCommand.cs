namespace EasyHouse.Simulations.Domain.Models.Comands;

public class GenerateReportCommand
{
    public Guid UserId { get; set; }
    public string Format { get; set; }
}