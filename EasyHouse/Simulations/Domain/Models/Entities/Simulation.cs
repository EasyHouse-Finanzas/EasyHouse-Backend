namespace EasyHouse.Simulations.Domain.Models.Entities;

public class Simulation
{
    public Guid SimulationId { get; set; }

    public Guid ClientId { get; set; }
    public Guid HouseId { get; set; }
    public Guid ConfigId { get; set; }

    public decimal InitialQuota { get; set; }
    public int TermMonths { get; set; }
    public DateTime StartDate { get; set; }

    public decimal? FixedQuota { get; set; }
    public decimal? TCEA { get; set; }
    public decimal? VAN { get; set; }
    public decimal? AnnualIRR { get; set; }

    public decimal? LoanAmount { get; set; }
    public decimal? TotalInterests { get; set; }
    public decimal? TotalCreditCost { get; set; }
    public decimal? DisbursementCommission { get; set; }
    public decimal? InsuranceMaintenanceFees { get; set; }
    
    public List<AmortizationDetail> AmortizationSchedule { get; set; } = new();
    public Client Client { get; set; } = null!;
    public House House { get; set; } = null!;
    public Config Config { get; set; } = null!;
}