using EasyHouse.Shared.Domain.Repositories;
using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Models.Repository;
using EasyHouse.Simulations.Domain.Services;
using FluentValidation;

namespace EasyHouse.Simulations.Application;

public class HouseCommandService : IHouseCommandService
{
    private readonly IHouseRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateHouseCommand> _validator;

    public HouseCommandService(
        IHouseRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateHouseCommand> validator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<House?> Create(CreateHouseCommand command)
    {
        var validation = await _validator.ValidateAsync(command);
        if (!validation.IsValid) throw new ValidationException(validation.Errors);

        var house = new House
        {
            HouseId = Guid.NewGuid(),
            Project = command.Proyecto,
            PropertyCode = command.CodigoInmueble,
            TotalArea = command.AreaTotal,
            BuiltArea = command.AreaTechada,
            Location = command.Ubicacion,
            Price = command.Precio
        };

        await _repository.AddAsync(house);
        await _unitOfWork.CompleteAsync();

        return house;
    }

    public async Task<House?> Update(Guid id, UpdateHouseCommand command)
    {
        var house = await _repository.FindByIdAsync(id);
        if (house == null) return null;

        house.Project = command.Project;
        house.PropertyCode = command.PropertyCode;
        house.TotalArea = command.TotalArea;
        house.BuiltArea = command.BuiltArea;
        house.Location = command.Location;
        house.Price = command.Price;

        _repository.Update(house);
        await _unitOfWork.CompleteAsync();

        return house;
    }

    public async Task<bool> Delete(Guid id)
    {
        var house = await _repository.FindByIdAsync(id);
        if (house == null) return false;

        _repository.Remove(house);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}