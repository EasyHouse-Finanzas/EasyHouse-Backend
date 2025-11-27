using EasyHouse.Shared.Domain.Repositories;
using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Models.Repository;
using EasyHouse.Simulations.Domain.Services;
using FluentValidation;

namespace EasyHouse.Simulations.Application;

public class ClientCommandService : IClientCommandService
{
    private readonly IClientRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateClientCommand> _validator;

    public ClientCommandService(
        IClientRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateClientCommand> validator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Client?> Handle(CreateClientCommand command)
    {
        var validation = await _validator.ValidateAsync(command);

        if (!validation.IsValid)
            throw new FluentValidation.ValidationException(validation.Errors);

        var client = new Client
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            BirthDate = command.BirthDate,
            DocumentNumber = command.DocumentNumber,
            Occupation = command.Occupation,
            MonthlyIncome = command.MonthlyIncome,
            UserId = command.UserId
        };

        await _repository.AddAsync(client);
        await _unitOfWork.CompleteAsync();

        return client;
    }
    
    public async Task<Client?> Update(Guid id, UpdateClientCommand command)
    {
        var client = await _repository.FindByIdAsync(id);
        
        if (client == null) 
            return null; 
        
        client.FirstName = command.FirstName;
        client.LastName = command.LastName;
        client.BirthDate = command.BirthDate;
        client.DocumentNumber = command.DocumentNumber;
        client.Occupation = command.Occupation;
        client.MonthlyIncome = command.MonthlyIncome;

        _repository.Update(client); 
        await _unitOfWork.CompleteAsync();

        return client;
    }

    public async Task<bool> Delete(Guid id)
    {
        var client = await _repository.FindByIdAsync(id);
        
        if (client == null) 
            return false; 

        _repository.Remove(client); 
        await _unitOfWork.CompleteAsync();
        
        return true;
    }
}