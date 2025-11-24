using EasyHouse.Simulations.Domain.Models.Comands;
using FluentValidation;

namespace EasyHouse.Simulations.Domain.Models.Validators;

public class CreateSimulationCommandValidator : AbstractValidator<CreateSimulationCommand>
{
    public CreateSimulationCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("ClientId es obligatorio.");

        RuleFor(x => x.HouseId)
            .NotEmpty().WithMessage("HouseId es obligatorio.");

        RuleFor(x => x.ConfigId)
            .NotEmpty().WithMessage("ConfigId es obligatorio.");

        RuleFor(x => x.InitialQuota)
            .GreaterThan(0).WithMessage("La cuota inicial debe ser mayor a 0.");

        RuleFor(x => x.TermMonths)
            .GreaterThan(0).WithMessage("El plazo debe ser mayor a 0.");
    }
}