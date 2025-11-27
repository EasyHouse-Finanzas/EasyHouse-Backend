using EasyHouse.Shared.Domain.Repositories;
using EasyHouse.Simulations.Domain.Models.Comands;
using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Models.Repository;
using EasyHouse.Simulations.Domain.Services;

namespace EasyHouse.Simulations.Application;

public class ConfigCommandService : IConfigCommandService
{
    private readonly IConfigRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    // IValidator<CreateConfigCommand> 

    public ConfigCommandService(IConfigRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    // POST
    public async Task<Config?> Create(CreateConfigCommand command)
    {
        var config = new Config
        {
            ConfigId = Guid.NewGuid(),
            Currency = command.Currency,
            RateType = command.RateType,
            Tea = command.Tea,
            Tna = command.Tna,
            Capitalization = command.Capitalization,
            GracePeriodType = command.GracePeriodType,
            GraceMonths = command.GraceMonths,
            HousingBonus = command.HousingBonus,
            DisbursementCommission = command.DisbursementCommission,
            MonthlyMaintenance = command.MonthlyMaintenance,
            MonthlyFees = command.MonthlyFees,
            Itf = command.Itf,
            LifeInsurance = command.LifeInsurance,
            RiskInsurance = command.RiskInsurance,
            AnnualDiscountRate = command.AnnualDiscountRate,
        };

        await _repository.AddAsync(config);
        await _unitOfWork.CompleteAsync();
        return config;
    }

    // PUT
    public async Task<Config?> Update(Guid id, UpdateConfigCommand command)
    {
        var config = await _repository.FindByIdAsync(id);
        if (config == null) return null;
        
        config.Currency = command.Currency;
        config.RateType = command.RateType;
        config.Tea = command.Tea;
        config.Tna = command.Tna;
        config.Capitalization = command.Capitalization;
        config.GracePeriodType = command.GracePeriodType;
        config.GraceMonths = command.GraceMonths;
        config.HousingBonus = command.HousingBonus;
        config.DisbursementCommission = command.DisbursementCommission;
        config.MonthlyMaintenance = command.MonthlyMaintenance;
        config.MonthlyFees = command.MonthlyFees;
        config.Itf = command.Itf;
        config.LifeInsurance = command.LifeInsurance;
        config.RiskInsurance = command.RiskInsurance;
        config.AnnualDiscountRate = command.AnnualDiscountRate;
        _repository.Update(config);
        await _unitOfWork.CompleteAsync();
        return config;
    }
    
    public async Task<bool> Delete(Guid id)
    {
        var config = await _repository.FindByIdAsync(id);
        if (config == null) return false;

        _repository.Remove(config);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}