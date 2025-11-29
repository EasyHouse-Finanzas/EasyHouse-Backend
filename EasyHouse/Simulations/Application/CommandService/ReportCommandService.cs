using EasyHouse.Shared.Domain.Repositories;
using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Models.Repository;
using EasyHouse.Simulations.Domain.Services;

namespace EasyHouse.Simulations.Application;

public class ReportCommandService : IReportCommandService
{
    private readonly IReportRepository _reportRepository;
    private readonly ISimulationRepository _simulationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReportCommandService(
        IReportRepository reportRepository,
        ISimulationRepository simulationRepository,
        IUnitOfWork unitOfWork)
    {
        _reportRepository = reportRepository;
        _simulationRepository = simulationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Report?> Generate(Guid simulationId, GenerateReportCommand command)
    {
        var simulation = await _simulationRepository.FindDetailedByIdAsync(simulationId);

        if (simulation == null)
            throw new Exception("Simulación no encontrada.");
        
        string generatedFileName = $"Reporte_{simulation.SimulationId}_{DateTime.Now:yyyyMMddHHmmss}.{command.Format.ToLower()}";
        string reportUrl = $"https://easyhouse.cloudstorage.com/reports/{generatedFileName}";
        var report = new Report
        {
            ReportId = Guid.NewGuid(),
            SimulationId = simulationId,
            UserId = command.UserId,
            GeneratedDate = DateTime.UtcNow,
            Format = command.Format,
            ReportUrl = reportUrl 
        };

        await _reportRepository.AddAsync(report);
        await _unitOfWork.CompleteAsync();

        return report;
    }
}