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
    // Asumo que el ExchangeRateService fue omitido y la tasa viene en el command.
    // private readonly IExchangeRateService _exchangeRateService; 

    public SimulationCommandService(
        ISimulationRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateSimulationCommand> validator,
        ISimulationCalculatorService calculator
        /*, IExchangeRateService exchangeRateService */)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _calculator = calculator;
        // _exchangeRateService = exchangeRateService;
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

        // --- 1. LÓGICA DE CONVERSIÓN DE MONEDA ---
        decimal finalHousePrice = house.Price;
        
        // Si la moneda de la casa (origen) es diferente a la moneda de la config (destino)
        if (house.Currency != config.Currency && command.ExchangeRate > 0) 
        {
            // Se realiza la conversión: Precio Final = Precio Original * Tasa de Cambio (T)
            finalHousePrice = house.Price * command.ExchangeRate;
        }
        
        var simulation = new Simulation
        {
            SimulationId = Guid.NewGuid(),
            ClientId = command.ClientId,
            HouseId = command.HouseId,
            ConfigId = config.ConfigId,
            InitialQuota = command.InitialQuota,
            TermMonths = command.TermMonths,
            StartDate = command.StartDate
        };
        

        var houseDataForCalculation = new House 
        { 
            Price = finalHousePrice,
            Currency = config.Currency,
            HouseId = house.HouseId,
            Project = house.Project,
            PropertyCode = house.PropertyCode,
            TotalArea = house.TotalArea,
            BuiltArea = house.BuiltArea,
            Location = house.Location
        };
        
        _calculator.Calculate(simulation, houseDataForCalculation, config); 

        await _repository.AddAsync(simulation);
        await _unitOfWork.CompleteAsync();

        return simulation;
    }
}