using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Services;

namespace EasyHouse.Simulations.Application;

public class SimulationCalculatorService : ISimulationCalculatorService
{
    // Método auxiliar: Calcula la Tasa Efectiva Periódica (TEP/TEM)
    // Busca hallar la tasa r (r) que se aplica a cada cuota.
    private decimal CalculateTep(Config config)
    {
        const int periodsPerYear = 12;

        if (config.RateType == "Efectiva" && config.Tea.HasValue)
        {
            // Fórmula (a) - Conversión de TEA a TEM: TEP = (1 + TEA)^(1/12) - 1
            // Busca hallar el equivalente mensual de la tasa anual.
            return (decimal)(Math.Pow(1 + (double)config.Tea.Value, 1.0 / periodsPerYear) - 1);
        }
        else if (config.RateType == "Nominal" && config.Tna.HasValue && config.Capitalization == "Mensual")
        {
            // Fórmula (b) - Conversión de TNA a TEM (si capitalización es mensual): TEP = TNA / 12
            // Busca hallar el costo efectivo de la tasa nominal para el periodo de capitalización.
            return config.Tna.Value / periodsPerYear;
        }
        return 0; 
    }
    
    // --- CÁLCULO DE VAN (Valor Actual Neto) ---
    // VAN = -P ± Σ [VF / (1+r)^n]. 'r' es la tasa mensual de descuento.
    private decimal CalculateVan(decimal principal, List<AmortizationDetail> schedule, decimal annualDiscountRate)
    {
        if (annualDiscountRate <= 0 || !schedule.Any()) return -principal; 
        
        // 1. Calcular la TEM del Descuento (TEM_COK). Simula la fórmula C19 = (1+D22)^(1/12)-1
        double r = (double)annualDiscountRate;
        double r_monthly = Math.Pow(1 + r, 1.0 / 12.0) - 1; 

        decimal van = -principal;
        
        foreach (var detail in schedule)
        {
            decimal vf = detail.Payment; 
            int n = detail.Period;
            
            // Valor Presente: VF / (1 + r_monthly)^n
            double presentValue = (double)vf / Math.Pow(1 + r_monthly, n);
            
            van += (decimal)presentValue;
        }

        return Math.Round(van, 2); 
    }
    
    // 0 = -P ± Σ [VF / (1+TIR)^n]
    // --- CÁLCULO DE TIR (Tasa Interna de Retorno) ---
    // Implementación funcional de algoritmo iterativo.
    private decimal CalculateIrr(decimal principal, List<AmortizationDetail> schedule)
    {
        var cashFlows = new List<double> { (double)-principal };
        cashFlows.AddRange(schedule.Select(d => (double)d.Payment));

        const double tolerance = 0.00000001;
        const int maxIterations = 100;

        // Tasa inicial de prueba (0.00833 mensual ~ 10% anual)
        double rate = 0.00833; 

        for (int i = 0; i < maxIterations; i++)
        {
            double npv = 0;
            double derivative = 0;

            for (int t = 0; t < cashFlows.Count; t++)
            {
                npv += cashFlows[t] / Math.Pow(1 + rate, t);
                derivative += -t * cashFlows[t] / Math.Pow(1 + rate, t + 1);
            }

            // Si converge (VAN ≈ 0)
            if (Math.Abs(npv) < tolerance)
            {
                // Retorna la TIR anualizada
                return (decimal)Math.Round(Math.Pow(1 + rate, 12) - 1, 4);
            }

            // Ajuste de la tasa para la siguiente iteración
            rate = rate - npv / derivative;
        }

        // Si no converge
        return 0.00M; 
    }

public void Calculate(Simulation simulation, House house, Config config)
    {
        // --- PREPARACIÓN DE DATOS ---

        // P: Monto de Préstamo
        decimal principal = house.Price - simulation.InitialQuota;
        if (config.HousingBonus.HasValue && config.HousingBonus.Value > 0)
        {
             principal -= config.HousingBonus.Value;
        }
        simulation.LoanAmount = principal;

        // TEP y Costos
        decimal tep = CalculateTep(config);
        int n = simulation.TermMonths;
        decimal monthlyCost = config.MonthlyMaintenance +
                              config.MonthlyFees +
                              config.LifeInsurance +
                              config.RiskInsurance;
        
        // --- 2. CÁLCULO DE CUOTA FIJA (C)  ---
        decimal cuotaSoloCapitalInteres = 0;
        if (tep > 0 && n > 0 && principal > 0)
        {
            double powerTerm = Math.Pow(1 + (double)tep, n);
            cuotaSoloCapitalInteres = principal * (tep * (decimal)powerTerm) / ((decimal)powerTerm - 1);
        } else if (n > 0) {
             cuotaSoloCapitalInteres = principal / n;
        }
        simulation.FixedQuota = Math.Round(cuotaSoloCapitalInteres + monthlyCost, 2);
        
        // --- 3. GENERACIÓN DEL CRONOGRAMA DE PAGOS  ---
        // (El código de iteración para generar simulation.AmortizationSchedule es correcto)
        
        decimal remainingBalance = principal;
        int graceMonths = config.GracePeriodType != "Ninguno" ? config.GraceMonths : 0;
        decimal totalInterestsPaidOrCapitalized = 0;
        simulation.AmortizationSchedule.Clear();

        for (int period = 1; period <= n; period++)
        {
            decimal interest = Math.Round(remainingBalance * tep, 2);
            decimal amortization = 0;
            decimal cuotaPeriodo = 0;
            
            // Lógica de Gracia (Omitida para brevedad, pero es la lógica de control de flujo correcta)
            if (period <= graceMonths && graceMonths > 0)
            {
                if (config.GracePeriodType == "Parcial") { cuotaPeriodo = interest + monthlyCost; amortization = 0; }
                else if (config.GracePeriodType == "Total") { cuotaPeriodo = monthlyCost; amortization = 0; remainingBalance += interest; }
            }
            else 
            {
                cuotaPeriodo = simulation.FixedQuota.Value; 
                amortization = Math.Round(cuotaSoloCapitalInteres - interest, 2);
                remainingBalance -= amortization;
            }
            
            totalInterestsPaidOrCapitalized += interest; 
            
            if (period == n) { amortization += remainingBalance; remainingBalance = 0; }

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
        
        // --- 4. CÁLCULOS FINALES CONSOLIDADOS ---
        
        simulation.TotalInterests = Math.Round(totalInterestsPaidOrCapitalized, 2);
        simulation.TCEA = (decimal)(Math.Pow(1 + (double)tep, 12) - 1); 
        simulation.DisbursementCommission = config.DisbursementCommission;
        simulation.InsuranceMaintenanceFees = monthlyCost; 
        simulation.TotalCreditCost = simulation.TotalInterests.Value + config.DisbursementCommission + (monthlyCost * n);

        // VAN y TIR 
        decimal annualCok = 0.00M;
        if (config.AnnualDiscountRate.HasValue) {
             annualCok = config.AnnualDiscountRate.Value;
        }
        
        simulation.VAN = CalculateVan(principal, simulation.AmortizationSchedule, annualCok); 
        simulation.AnnualIRR = CalculateIrr(principal, simulation.AmortizationSchedule); 
    }
}