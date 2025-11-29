using System.Text.Json.Serialization;

namespace EasyHouse.Simulations.Domain.Models.Entities;

public class AmortizationDetail
{
    public Guid Id { get; set; } 
    
    public Guid SimulationId { get; set; } 
    public int Period { get; set; } 
    
    public decimal Payment { get; set; }      // Cuota Total a Pagar
    public decimal Interest { get; set; }     // Interés
    public decimal Amortization { get; set; } // Amortización del Capital
    public decimal Balance { get; set; }      // Saldo Deudor
    
    // --- NUEVAS PROPIEDADES NECESARIAS PARA TU FRONTEND ---
    public decimal Seguros { get; set; }      // Suma de Desgravamen + Riesgo
    public decimal Gastos { get; set; }       // Suma de Portes + Mantenimiento
    // ------------------------------------------------------

    public DateTime PaymentDate { get; set; }
    
    [JsonIgnore] 
    public Simulation Simulation { get; set; } = null!;
}