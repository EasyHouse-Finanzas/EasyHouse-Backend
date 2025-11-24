using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Services;

namespace EasyHouse.Simulations.Application;

public class SimulationCalculatorService : ISimulationCalculatorService
{
    // Método para calcular la Tasa Efectiva Periódica (TEP/TEM)
    private decimal CalculateTep(Config config)
    {
        // Asumimos periodo de pago Mensual para TEP (TEM)
        const int periodsPerYear = 12;

        if (config.RateType == "Efectiva" && config.Tea.HasValue)
        {
            // Convertir TEA a TEM: TEM = (1 + TEA)^(1/12) - 1
            return (decimal)(Math.Pow(1 + (double)config.Tea.Value, 1.0 / periodsPerYear) - 1);
        }
        else if (config.RateType == "Nominal" && config.Tna.HasValue && config.Capitalization == "Mensual")
        {
            // Convertir TNA (con capitalización mensual, m=12) a TEM: TEM = TNA / 12
            return config.Tna.Value / periodsPerYear;
        }
        return 0; 
    }

    public void Calculate(Simulation simulation, House house, Config config)
    {
        // CÁLCULOS INICIALES Y AJUSTE DE CAPITAL 

        // P: Préstamo (Capital a financiar) = Precio - Cuota Inicial - Bono (si aplica)
        decimal principal = house.Price - simulation.InitialQuota;
        
        // Aplicar Bono Techo Propio (si existe y si aplica. Asumimos que se resta del principal)
        if (config.HousingBonus.HasValue && config.HousingBonus.Value > 0)
        {
             principal -= config.HousingBonus.Value;
        }
        
        simulation.LoanAmount = principal;

        // TEP (TEM): Tasa Efectiva Mensual (basada en la conversión)
        decimal tep = CalculateTep(config);
        
        // n: Número total de periodos (meses)
        int n = simulation.TermMonths;

        // S: Costos adicionales mensuales (Seguros + Portes + Mantenimiento)
        decimal monthlyCost = config.MonthlyMaintenance +
                              config.MonthlyFees +
                              config.LifeInsurance +
                              config.RiskInsurance;
        
        //CÁLCULO DE LA CUOTA FIJA (C) - Método Francés ---

        decimal cuotaSoloCapitalInteres = 0;
        if (tep > 0)
        {
            // FÓRMULA DEL MÉTODO FRANCÉS (solo Capital + Interés)
            double powerTerm = Math.Pow(1 + (double)tep, n);
            cuotaSoloCapitalInteres = principal * (tep * (decimal)powerTerm) /
                                      ((decimal)powerTerm - 1);
        } else {
             // Caso hipotético si la tasa fuera 0% (solo amortización)
             cuotaSoloCapitalInteres = principal / n;
        }

        // Cuota Fija (Total) = C + S
        simulation.FixedQuota = Math.Round(cuotaSoloCapitalInteres + monthlyCost, 2);
        
        // GENERACIÓN DEL CRONOGRAMA DE PAGOS 

        decimal remainingBalance = principal;
        int graceMonths = config.GracePeriodType != "Ninguno" ? config.GraceMonths : 0;
        decimal totalInterests = 0;
        
        simulation.AmortizationSchedule.Clear();

        for (int period = 1; period <= n; period++)
        {
            decimal interest = Math.Round(remainingBalance * tep, 2);
            decimal amortization = 0;
            decimal cuotaPeriodo = 0;
            
            // Lógica de Periodo de Gracia
            if (period <= graceMonths && graceMonths > 0)
            {
                if (config.GracePeriodType == "Parcial") // Paga solo Intereses
                {
                    cuotaPeriodo = interest + monthlyCost;
                    amortization = 0; 
                    // El capital no varía, los intereses se pagan
                }
                else if (config.GracePeriodType == "Total") // Total: Intereses se capitalizan
                {
                    cuotaPeriodo = monthlyCost; // Solo paga los costos adicionales
                    amortization = 0;
                    remainingBalance += interest; // Se suma el interés al capital
                }
            }
            else // Fuera del período de gracia (Pago normal por Método Francés)
            {
                // La cuota total es la fija (C + S)
                cuotaPeriodo = simulation.FixedQuota.Value; 
                amortization = Math.Round(cuotaSoloCapitalInteres - interest, 2);
                remainingBalance -= amortization;
            }
            
            totalInterests += interest; // Sumamos intereses pagados o capitalizados
            
            // Ajuste final para asegurar que el último saldo sea cero
            if (period == n)
            {
                // Si hay un pequeño error de redondeo, se ajusta en la última amortización
                amortization += remainingBalance; 
                remainingBalance = 0;
            }

            simulation.AmortizationSchedule.Add(new AmortizationDetail
            {
                Id = Guid.NewGuid(),
                SimulationId = simulation.SimulationId,
                Period = period,
                Payment = Math.Round(cuotaPeriodo, 2),
                Interest = interest,
                Amortization = Math.Round(amortization, 2),
                Balance = Math.Round(remainingBalance < 0 ? 0 : remainingBalance, 2)
            });
        }
        
        //  CÁLCULOS FINALES

        // Intereses Totales (Sumados durante el cronograma)
        simulation.TotalInterests = Math.Round(totalInterests, 2);

        // TCEA (Tasa de Costo Efectivo Anual)
        simulation.TCEA = (decimal)(Math.Pow(1 + (double)tep, 12) - 1); 

        // Costo Total del Crédito
        simulation.DisbursementCommission = config.DisbursementCommission;
        simulation.InsuranceMaintenanceFees = monthlyCost; // Costo mensual
        
        simulation.TotalCreditCost = simulation.TotalInterests.Value +
                                     config.DisbursementCommission +
                                     (monthlyCost * n); // Suma de todos los costos mensuales

        // VAN y TIR 
        simulation.VAN = 0.00M; 
        simulation.AnnualIRR = 0.00M; 
    }
}