using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Models.Repository;
using EasyHouse.Simulations.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyHouse.Simulations.Interfaces;

[ApiController]
[Route("api/v1/houses")]
public class HouseController : ControllerBase
{
    private readonly IHouseCommandService _commandService;
    private readonly IHouseRepository _repository;

    public HouseController(
        IHouseCommandService commandService,
        IHouseRepository repository)
    {
        _commandService = commandService;
        _repository = repository;
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateHouse([FromBody] CreateHouseCommand model)
    {
        var result = await _commandService.Create(model);
        return Ok(result);
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetHouses()
    {
        var houses = await _repository.ListAsync();
        return Ok(houses);
    }


    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetHouse(Guid id)
    {
        var house = await _repository.FindByIdAsync(id);
        if (house == null) return NotFound();

        return Ok(house);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateHouse(Guid id, [FromBody] UpdateHouseCommand command)
    {
        var result = await _commandService.Update(id, command);
        return Ok(result);
    }


    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteHouse(Guid id)
    {
        var deleted = await _commandService.Delete(id);

        if (!deleted) return NotFound();

        return NoContent();
    }
}