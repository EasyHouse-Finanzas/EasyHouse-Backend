using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyHouse.Simulations.Interfaces;

[ApiController]
[Route("api/v1/simulations")]
public class SimulationController : ControllerBase
{
    private readonly ISimulationCommandService _commandService;

    public SimulationController(ISimulationCommandService commandService)
    {
        _commandService = commandService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSimulation(CreateSimulationCommand command)
    {
        var result = await _commandService.Handle(command);
        return Ok(result);
    }
}