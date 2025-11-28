using System.Security.Claims; // Necesario para leer el token
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
    
    // POST: api/v1/clients
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientCommand model)
    {
        var result = await _commandService.Handle(model);
        return Ok(result);
    }

    // GET: api/v1/clients (CORREGIDO: FILTRA POR USUARIO)
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetClients()
    {
        // 1. Extraer ID del usuario
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        // 2. Llamar al método filtrado del repositorio
        var clients = await _repository.FindAllByUserIdAsync(userId);
        return Ok(clients);
    }
    
    // GET por ID: api/v1/clients/{id}
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetClient(Guid id)
    {
        var client = await _repository.FindByIdAsync(id);
        
        // Seguridad extra: Verificar que el cliente pertenezca al usuario
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (client != null && client.UserId.ToString() != userIdString)
             return Forbid(); // O NotFound() para ocultarlo

        if (client == null) 
            return NotFound();

        return Ok(client);
    }

    // ... (PUT y DELETE quedan igual, aunque idealmente también deberían validar el UserId) ...
    
    // PUT (Actualizar): api/v1/clients/{id}
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateClient(Guid id, [FromBody] UpdateClientCommand command)
    {
        var result = await _commandService.Update(id, command);
        if (result == null) return NotFound(); 
        return Ok(result); 
    }

    // DELETE: api/v1/clients/{id}
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteClient(Guid id)
    {
        var deleted = await _commandService.Delete(id);
        if (!deleted) return NotFound(); 
        return NoContent(); 
    }
}