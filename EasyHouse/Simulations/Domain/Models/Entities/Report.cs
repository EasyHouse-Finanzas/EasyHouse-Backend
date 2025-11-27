namespace EasyHouse.Simulations.Domain.Models.Entities;

public class Report
{
    public Guid ReportId { get; set; }
    public Guid SimulationId { get; set; }
    public Guid UserId { get; set; }

    public DateTime GeneratedDate { get; set; }
    public string Format { get; set; } // PDF, Excel, CSV
    public string ReportUrl { get; set; }
    public Simulation Simulation { get; set; }
}