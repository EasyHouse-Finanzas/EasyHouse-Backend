namespace EasyHouse.Simulations.Domain.Models.Entities;

public class Config
{
    public Guid ConfigId { get; set; }

    public string Currency { get; set; }
    public string RateType { get; set; }

    public decimal? Tea { get; set; }
    public decimal? Tna { get; set; }
    public string? Capitalization { get; set; }

    public string GracePeriodType { get; set; }
    public int GraceMonths { get; set; }

    public decimal? HousingBonus { get; set; }
    public decimal DisbursementCommission { get; set; }
    public decimal MonthlyMaintenance { get; set; }
    public decimal MonthlyFees { get; set; }
    public decimal Itf { get; set; }
    public decimal LifeInsurance { get; set; }
    public decimal RiskInsurance { get; set; }

    public List<Simulation> Simulations { get; set; } = new();
}