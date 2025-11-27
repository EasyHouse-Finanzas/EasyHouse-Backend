using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyHouse.Simulations.Infrastructure.Persistence.Repositories;

[ApiController]
[Route("api/v1/reports")] 
public class ReportController : ControllerBase
{
    private readonly IReportCommandService _commandService;

    public ReportController(IReportCommandService commandService)
    {
        _commandService = commandService;
    }
    
    // POST: api/v1/reports/{simulationId}
    [HttpPost("{simulationId:guid}")]
    [Authorize]
    public async Task<IActionResult> GenerateReport(
        Guid simulationId, 
        [FromBody] GenerateReportCommand command)
    {
        if (command == null || string.IsNullOrWhiteSpace(command.Format))
        {
            return BadRequest(new { message = "El formato de reporte es obligatorio." });
        }
        
        try
        {
            var result = await _commandService.Generate(simulationId, command);
            
            return Created(result.ReportUrl, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al generar el reporte: {ex.Message}" });
        }
    }
}