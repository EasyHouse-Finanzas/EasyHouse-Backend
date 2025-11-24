using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Models.Repository;
using EasyHouse.Simulations.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyHouse.Simulations.Interfaces;

[ApiController]
[Route("api/v1/clients")]
public class ClientController : ControllerBase
{
    private readonly IClientCommandService _commandService;
    private readonly IClientRepository _repository;

    public ClientController(
        IClientCommandService commandService,
        IClientRepository repository)
    {
        _commandService = commandService;
        _repository = repository;
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientCommand model)
    {
        var result = await _commandService.Handle(model);
        return Ok(result);
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetClients()
    {
        var clients = await _repository.ListAsync();
        return Ok(clients);
    }
}