using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyHouse.Simulations.Interfaces;

[ApiController]
[Route("api/v1/simulations")]
public class SimulationController : ControllerBase
{
    private readonly ISimulationCommandService _commandService;
    private readonly ISimulationQueryService _queryService; 

    public SimulationController(
        ISimulationCommandService commandService,
        ISimulationQueryService queryService) 
    {
        _commandService = commandService;
        _queryService = queryService;
    }

    // POST: api/v1/simulations 
    [HttpPost]
    public async Task<IActionResult> CreateSimulation([FromBody] CreateSimulationCommand command)
    {
        var result = await _commandService.Handle(command);
        
        return CreatedAtAction(nameof(GetSimulation), new { id = result.SimulationId }, result);
    }

    // GET: api/v1/simulations/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSimulation(Guid id)
    {
        var simulation = await _queryService.GetDetailedSimulationByIdAsync(id);

        if (simulation == null)
        {
            return NotFound(); 
        }
        return Ok(simulation); 
    }
}