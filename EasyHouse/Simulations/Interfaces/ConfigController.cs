using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Models.Repository;
using EasyHouse.Simulations.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyHouse.Simulations.Interfaces;

[ApiController]
[Route("api/v1/configs")]
public class ConfigController : ControllerBase
{
    private readonly IConfigCommandService _commandService;
    private readonly IConfigRepository _repository;

    public ConfigController(
        IConfigCommandService commandService,
        IConfigRepository repository)
    {
        _commandService = commandService;
        _repository = repository;
    }
    
    // POST: api/v1/configs
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateConfig([FromBody] CreateConfigCommand model)
    {
        var result = await _commandService.Create(model);
        return Ok(result);
    }
    
    // GET: api/v1/configs
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetConfigs()
    {
        var configs = await _repository.ListAsync();
        return Ok(configs);
    }
    
    // GET: api/v1/configs/{id}
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetConfig(Guid id)
    {
        var config = await _repository.FindByIdAsync(id);
        if (config == null) return NotFound();

        return Ok(config);
    }

    // PUT: api/v1/configs/{id}
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateConfig(Guid id, [FromBody] UpdateConfigCommand command)
    {
        var result = await _commandService.Update(id, command);
        if (result == null) return NotFound();
        
        return Ok(result);
    }
}