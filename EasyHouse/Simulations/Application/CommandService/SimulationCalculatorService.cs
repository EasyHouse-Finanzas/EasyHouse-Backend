using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Services;

namespace EasyHouse.Simulations.Application;

public class SimulationCalculatorService : ISimulationCalculatorService
{
    // ==========================================
    // 1. MÉTODOS AUXILIARES
    // ==========================================
    
    private decimal TnaToTea(decimal tna, string capitalizationFrequency)
    {
        double m = capitalizationFrequency == "Diaria" ? 360.0 : 12.0;
        return (decimal)(Math.Pow(1 + (double)tna / m, m) - 1);
    }

    private decimal TeaToTem(decimal tea)
    {
        return (decimal)(Math.Pow(1 + (double)tea, 1.0 / 12.0) - 1);
    }

    private decimal CalculateTep(Config config)
    {
        // 1. Sanitización de tasas 
        decimal? rawTea = config.Tea;
        decimal? rawTna = config.Tna;

        if (config.RateType == "Efectiva")
        {
            return TeaToTem(rawTea ?? 0);
        }
        else if (config.RateType == "Nominal")
        {
            string cap = config.Capitalization ?? "Mensual";
            return TeaToTem(TnaToTea(rawTna ?? 0, cap));
        }
        return 0; 
    }

    private decimal CalculateVan(decimal initialFlow, List<AmortizationDetail> schedule, decimal annualDiscountRate)
    {
        double r_monthly = 0;
        if (annualDiscountRate > -1.0M)
        {
            r_monthly = Math.Pow(1 + (double)annualDiscountRate, 1.0 / 12.0) - 1;
        }

        double van = (double)initialFlow;
        foreach (var item in schedule)
        {
            if (Math.Abs(r_monthly) < 1e-9) van -= (double)item.Payment;
            else van -= (double)item.Payment / Math.Pow(1 + r_monthly, item.Period);
        }
        return Math.Round((decimal)van, 2);
    }

    private decimal CalculateIrr(decimal initialFlow, List<AmortizationDetail> schedule)
    {
        var cashFlows = new List<double> { (double)initialFlow };
        cashFlows.AddRange(schedule.Select(d => -(double)d.Payment));

        double rate = 0.01; 
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
                fDerivative += -t * cashFlows[t] / (denominator * (1 + rate));
            }

            if (Math.Abs(fValue) < tol) break;
            if (Math.Abs(fDerivative) < 1e-12) break;

            double newRate = rate - fValue / fDerivative;
            if (newRate <= -1.0) newRate = -0.99;

            if (Math.Abs(newRate - rate) < tol) { rate = newRate; break; }
            rate = newRate;
        }
        return (decimal)(Math.Pow(1 + rate, 12) - 1);
    }

    // ==========================================
    // 2. CÁLCULO PRINCIPAL
    // ==========================================
    public void Calculate(Simulation simulation, House house, Config config)
    {
        // =================================================================================
        // PASO 0: SANITIZACIÓN DE DATOS (Intermediarios Seguros)
        // Convertimos todo a variables locales 'decimal' puras usando coalescencia nula (?? 0).
        // =================================================================================
        
        // Variables desde Config (Usamos cast a decimal? para compatibilidad total)
        decimal housingBonus = ((decimal?)config.HousingBonus) ?? 0;
        decimal monthlyMaintenance = ((decimal?)config.MonthlyMaintenance) ?? 0;
        decimal monthlyFees = ((decimal?)config.MonthlyFees) ?? 0;
        decimal lifeInsuranceRate = ((decimal?)config.LifeInsurance) ?? 0;
        decimal riskInsuranceRate = ((decimal?)config.RiskInsurance) ?? 0;
        decimal disbursementCommission = ((decimal?)config.DisbursementCommission) ?? 0;
        decimal annualDiscountRate = ((decimal?)config.AnnualDiscountRate) ?? 0;
        
        // Gracia
        int graceMonths = ((int?)config.GraceMonths) ?? 0;

        // =================================================================================
        // LÓGICA DE NEGOCIO
        // =================================================================================

        // 1. Ajuste del Capital
        decimal principal = house.Price - simulation.InitialQuota - housingBonus;
        simulation.LoanAmount = principal;

        decimal tep = CalculateTep(config);
        int n = simulation.TermMonths;

        // 2. Costos Fijos
        decimal fixedFees = monthlyMaintenance + monthlyFees;
        decimal riskInsuranceAmount = house.Price * riskInsuranceRate;

        // 3. Generación del Cronograma
        decimal remainingBalance = principal;
        
        // Validación extra de Gracia
        if (string.IsNullOrEmpty(config.GracePeriodType) || config.GracePeriodType == "Ninguno")
        {
            graceMonths = 0;
        }
        
        decimal accumulatedInterests = 0;
        decimal accumulatedInsurance = 0; 
        
        simulation.AmortizationSchedule.Clear();
        DateTime baseDate = simulation.StartDate != default ? simulation.StartDate : DateTime.Now;

        for (int period = 1; period <= n; period++)
        {
            decimal interest = Math.Round(remainingBalance * tep, 2);
            decimal lifeInsuranceAmount = Math.Round(remainingBalance * lifeInsuranceRate, 2);

            decimal amortization = 0;
            decimal quotaCapitalInterest = 0;

            if (period <= graceMonths && graceMonths > 0)
            {
                if (config.GracePeriodType == "Parcial") 
                {
                    quotaCapitalInterest = interest;
                    amortization = 0; 
                }
                else if (config.GracePeriodType == "Total") 
                {
                    quotaCapitalInterest = 0;
                    amortization = 0;
                    remainingBalance += interest; 
                }
            }
            else 
            {
                int remainingPeriods = n - period + 1;
                
                if (tep > 0) 
                {
                    double powerTerm = Math.Pow(1 + (double)tep, remainingPeriods);
                    decimal frenchQuota = remainingBalance * (tep * (decimal)powerTerm) / ((decimal)powerTerm - 1);
                    quotaCapitalInterest = frenchQuota;
                }
                else
                {
                    quotaCapitalInterest = remainingBalance / remainingPeriods;
                }
                
                amortization = quotaCapitalInterest - interest;
                remainingBalance -= amortization;
            }
            
            // Ajustes finales de saldo
            if (remainingBalance < 0 && remainingBalance > -5)
            {
                amortization += remainingBalance;
                remainingBalance = 0;
            }
            else if (period == n && remainingBalance > 0)
            {
                amortization += remainingBalance;
                quotaCapitalInterest += remainingBalance;
                remainingBalance = 0;
            }

            // Agrupación de Seguros y Gastos
            decimal totalSeguros = lifeInsuranceAmount + riskInsuranceAmount;
            decimal totalGastos = fixedFees;
            
            decimal totalPayment = quotaCapitalInterest + totalSeguros + totalGastos;

            accumulatedInterests += interest;
            accumulatedInsurance += (totalSeguros + totalGastos);

            // Detalle
            var detail = new AmortizationDetail
            {
                Id = Guid.NewGuid(),
                SimulationId = simulation.SimulationId,
                Period = period,
                Payment = Math.Round(totalPayment, 2),
                Interest = Math.Round(interest, 2),
                Amortization = Math.Round(amortization, 2),
                Balance = Math.Round(remainingBalance, 2),
                
                Seguros = Math.Round(totalSeguros, 2), 
                Gastos = Math.Round(totalGastos, 2),
                
                PaymentDate = baseDate.AddMonths(period)
            };
            
            simulation.AmortizationSchedule.Add(detail);
        }
        
        // 4. Resultados Finales
        var firstNormalQuota = simulation.AmortizationSchedule
            .FirstOrDefault(x => x.Period > graceMonths)?.Payment ?? 0;
        
        simulation.FixedQuota = firstNormalQuota;
        simulation.TotalInterests = Math.Round(accumulatedInterests, 2);
        
        simulation.DisbursementCommission = disbursementCommission;

        decimal loanAmountSafe = ((decimal?)simulation.LoanAmount) ?? 0;

        simulation.TotalCreditCost = loanAmountSafe + accumulatedInterests + accumulatedInsurance + disbursementCommission;
        
        // Ahora usamos 'loanAmountSafe'  en lugar de 'simulation.LoanAmount'
        decimal netProceeds = loanAmountSafe - disbursementCommission;

        simulation.TCEA = Math.Round(CalculateIrr(netProceeds, simulation.AmortizationSchedule) * 100, 2); 
        simulation.AnnualIRR = simulation.TCEA;

        simulation.VAN = Math.Round(CalculateVan(netProceeds, simulation.AmortizationSchedule, annualDiscountRate), 2);
    }
}