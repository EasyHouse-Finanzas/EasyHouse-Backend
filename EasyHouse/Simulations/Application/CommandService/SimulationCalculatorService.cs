using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Services;

namespace EasyHouse.Simulations.Application;

public class SimulationCalculatorService : ISimulationCalculatorService
{
    // --- 1. CONVERSIÓN DE TASAS (Mejora de Trazabilidad) ---
    
    // Convierte TNA a TEA según la capitalización (Diaria o Mensual)
    private decimal TnaToTea(decimal tna, string capitalizationFrequency)
    {
        // m: frecuencia de capitalización en un año comercial (360 días)
        // Diaria = 360, Mensual = 12
        double m = capitalizationFrequency == "Diaria" ? 360.0 : 12.0;
        
        // Fórmula: TEA = (1 + TNA/m)^m - 1
        return (decimal)(Math.Pow(1 + (double)tna / m, m) - 1);
    }

    // Convierte TEA a TEM
    private decimal TeaToTem(decimal tea)
    {
        // Fórmula: TEM = (1 + TEA)^(1/12) - 1
        return (decimal)(Math.Pow(1 + (double)tea, 1.0 / 12.0) - 1);
    }

    private decimal CalculateTep(Config config)
    {
        if (config.RateType == "Efectiva" && config.Tea.HasValue)
        {
            return TeaToTem(config.Tea.Value);
        }
        else if (config.RateType == "Nominal" && config.Tna.HasValue)
        {
            // Trazabilidad completa: TNA -> TEA -> TEM
            string cap = config.Capitalization ?? "Mensual";
            decimal tea = TnaToTea(config.Tna.Value, cap);
            return TeaToTem(tea);
        }
        return 0; 
    }

    // --- 2. CÁLCULO DE VAN (Corrección Bug r=0) ---
    private decimal CalculateVan(decimal initialFlow, List<AmortizationDetail> schedule, decimal annualDiscountRate)
    {
        // Convertimos la COK anual a mensual
        double r_monthly = 0;
        
        // Solo calculamos la tasa mensual si la anual es válida y diferente de -100%
        if (annualDiscountRate > -1.0M)
        {
            r_monthly = Math.Pow(1 + (double)annualDiscountRate, 1.0 / 12.0) - 1;
        }

        double van = (double)initialFlow;

        foreach (var item in schedule)
        {
            // Si la tasa es 0, no dividimos entre potencias, es una resta simple
            if (Math.Abs(r_monthly) < 1e-9) // r_monthly aprox 0
            {
                van -= (double)item.Payment;
            }
            else
            {
                van -= (double)item.Payment / Math.Pow(1 + r_monthly, item.Period);
            }
        }

        return Math.Round((decimal)van, 2);
    }

    // --- 3. CÁLCULO DE TIR / TCEA (Mayor Robustez) ---
    private decimal CalculateIrr(decimal initialFlow, List<AmortizationDetail> schedule)
    {
        var cashFlows = new List<double> { (double)initialFlow };
        cashFlows.AddRange(schedule.Select(d => -(double)d.Payment));

        double rate = 0.01; // Semilla inicial 1%
        const int maxIter = 100;
        const double tol = 1e-6;

        for (int i = 0; i < maxIter; i++)
        {
            double fValue = 0;
            double fDerivative = 0;

            for (int t = 0; t < cashFlows.Count; t++)
            {
                double denominator = Math.Pow(1 + rate, t);
                fValue += cashFlows[t] / denominator;
                
                // Derivada: -t * Flujo / (1+r)^(t+1)
                fDerivative += -t * cashFlows[t] / (denominator * (1 + rate));
            }

            // Si ya convergimos (VAN cercano a 0)
            if (Math.Abs(fValue) < tol) break;

            // Protección: Si la derivada es muy cercana a 0, detenemos para evitar NaN/Infinito
            if (Math.Abs(fDerivative) < 1e-12) break;

            double newRate = rate - fValue / fDerivative;

            // Protección: Evitar tasas menores a -100% que rompen Math.Pow
            if (newRate <= -1.0) newRate = -0.99;

            if (Math.Abs(newRate - rate) < tol) 
            {
                rate = newRate;
                break;
            }
            rate = newRate;
        }

        // Convertir TIR Mensual a Anual
        return (decimal)(Math.Pow(1 + rate, 12) - 1);
    }

    // --- 4. CÁLCULO PRINCIPAL ---
    public void Calculate(Simulation simulation, House house, Config config)
    {
        // 1. Ajuste del Capital
        decimal principal = house.Price - simulation.InitialQuota;
        if (config.HousingBonus.HasValue && config.HousingBonus.Value > 0)
        {
             principal -= config.HousingBonus.Value;
        }
        simulation.LoanAmount = principal;

        // 2. Variables de Tiempo y Tasa
        decimal tep = CalculateTep(config);
        int n = simulation.TermMonths;
        
        decimal monthlyCost = config.MonthlyMaintenance +
                              config.MonthlyFees +
                              config.LifeInsurance +
                              config.RiskInsurance;

        // 3. Cuota Fija Base (Referencial)
        decimal cuotaSoloCapitalInteres = 0;
        if (tep > 0 && n > 0)
        {
            double powerTerm = Math.Pow(1 + (double)tep, n);
            cuotaSoloCapitalInteres = principal * (tep * (decimal)powerTerm) / ((decimal)powerTerm - 1);
        }
        else if (n > 0)
        {
             cuotaSoloCapitalInteres = principal / n;
        }
        simulation.FixedQuota = Math.Round(cuotaSoloCapitalInteres + monthlyCost, 2);
        
        // 4. Generación del Cronograma
        decimal remainingBalance = principal;
        int graceMonths = config.GracePeriodType != "Ninguno" ? config.GraceMonths : 0;
        decimal totalInterests = 0;
        
        simulation.AmortizationSchedule.Clear();

        for (int period = 1; period <= n; period++)
        {
            decimal interest = Math.Round(remainingBalance * tep, 2);
            decimal amortization = 0;
            decimal cuotaPeriodo = 0;
            
            if (period <= graceMonths && graceMonths > 0)
            {
                if (config.GracePeriodType == "Parcial") 
                {
                    cuotaPeriodo = interest + monthlyCost;
                    amortization = 0; 
                }
                else if (config.GracePeriodType == "Total") 
                {
                    cuotaPeriodo = monthlyCost; 
                    amortization = 0;
                    remainingBalance += interest; // Capitalización
                }
            }
            else 
            {
                // Recálculo por posible cambio de saldo en gracia total
                int remainingPeriods = n - period + 1;
                double powerTerm = Math.Pow(1 + (double)tep, remainingPeriods);
                
                // Si tep es 0, evitamos división por cero en la fórmula francesa
                decimal cuotaRecalculada = 0;
                if (tep > 0) 
                {
                    cuotaRecalculada = remainingBalance * (tep * (decimal)powerTerm) / ((decimal)powerTerm - 1);
                }
                else
                {
                    cuotaRecalculada = remainingBalance / remainingPeriods;
                }
                
                cuotaPeriodo = cuotaRecalculada + monthlyCost;
                amortization = cuotaRecalculada - interest;
                remainingBalance -= amortization;
            }
            
            totalInterests += interest;
            
            // Ajuste final de precisión
            if (remainingBalance < 0 && remainingBalance > -5)
            {
                amortization += remainingBalance;
                remainingBalance = 0;
            }
            else if (period == n && remainingBalance > 0)
            {
                amortization += remainingBalance;
                cuotaPeriodo += remainingBalance;
                remainingBalance = 0;
            }

            simulation.AmortizationSchedule.Add(new AmortizationDetail
            {
                Id = Guid.NewGuid(),
                SimulationId = simulation.SimulationId,
                Period = period,
                Payment = Math.Round(cuotaPeriodo, 2),
                Interest = Math.Round(interest, 2),
                Amortization = Math.Round(amortization, 2),
                Balance = Math.Round(remainingBalance, 2)
            });
        }
        
        // 5. Asignación de Resultados
        simulation.TotalInterests = Math.Round(totalInterests, 2);
        simulation.DisbursementCommission = config.DisbursementCommission;
        simulation.InsuranceMaintenanceFees = monthlyCost;
        simulation.TotalCreditCost = simulation.TotalInterests.Value + config.DisbursementCommission + (monthlyCost * n) + principal;

        // Flujo Neto para TCEA = Préstamo Recibido - Comisiones Iniciales
        decimal netProceeds = principal - config.DisbursementCommission;

        // TCEA / TIR
        simulation.TCEA = Math.Round(CalculateIrr(netProceeds, simulation.AmortizationSchedule) * 100, 2); 
        simulation.AnnualIRR = simulation.TCEA;

        // VAN
        decimal annualCok = config.AnnualDiscountRate ?? 0;
        simulation.VAN = Math.Round(CalculateVan(netProceeds, simulation.AmortizationSchedule, annualCok), 2);
    }
}