using EasyHouse.Shared.Domain.Repositories;
using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Models.Repository;
using EasyHouse.Simulations.Domain.Services;
using FluentValidation;

namespace EasyHouse.Simulations.Application;

public class SimulationCommandService : ISimulationCommandService
{
    private readonly ISimulationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateSimulationCommand> _validator;
    private readonly ISimulationCalculatorService _calculator;

    public SimulationCommandService(
        ISimulationRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateSimulationCommand> validator,
        ISimulationCalculatorService calculator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _calculator = calculator;
    }

    public async Task<Simulation?> Handle(CreateSimulationCommand command)
    {
        var validation = await _validator.ValidateAsync(command);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        var client = await _repository.GetClientByIdAsync(command.ClientId);
        var house  = await _repository.GetHouseByIdAsync(command.HouseId);
        var config = await _repository.GetConfigByIdAsync(command.ConfigId);

        if (client == null || house == null || config == null)
            throw new Exception("Cliente, Casa o Config no encontrados.");

        var simulation = new Simulation
        {
            SimulationId = Guid.NewGuid(),
            ClientId = command.ClientId,
            HouseId = command.HouseId,
            ConfigId = command.ConfigId,
            InitialQuota = command.InitialQuota,
            TermMonths = command.TermMonths,
            StartDate = command.StartDate
        };
        
        _calculator.Calculate(simulation, house, config);

        await _repository.AddAsync(simulation);
        await _unitOfWork.CompleteAsync();

        return simulation;
    }
}