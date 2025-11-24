using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Services;

namespace EasyHouse.Simulations.Application;

public class SimulationCalculatorService : ISimulationCalculatorService
{
    public void Calculate(Simulation simulation, House house, Config config)
    {
        // 1. Monto del préstamo
        simulation.LoanAmount = house.Price - simulation.InitialQuota;

        // 2. Tasa mensual
        decimal tea = config.Tea ?? 0;
        decimal i = (decimal)Math.Pow(1 + (double)tea, 1.0 / 12.0) - 1;

        // 3. Cuota fija (sistema francés)
        int n = simulation.TermMonths;
        decimal P = simulation.LoanAmount ?? 0;

        decimal cuota = P * (i * (decimal)Math.Pow(1 + (double)i, n)) /
                        ((decimal)Math.Pow(1 + (double)i, n) - 1);

        simulation.FixedQuota = Math.Round(cuota, 2);

        // 4. Intereses totales
        simulation.TotalInterests = (cuota * n) - P;

        // 5. TCEA (simplificada)
        simulation.TCEA = (decimal)(Math.Pow(1 + (double)i, 12) - 1);

        // 6. Comisiones + seguros + portes
        simulation.DisbursementCommission = config.DisbursementCommission;
        simulation.InsuranceMaintenanceFees =
            config.MonthlyMaintenance +
            config.MonthlyFees +
            config.LifeInsurance +
            config.RiskInsurance;

        // 7. Costo total del crédito
        simulation.TotalCreditCost = simulation.TotalInterests +
                                     simulation.DisbursementCommission +
                                     simulation.InsuranceMaintenanceFees * n;
    }
}